using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using NuGet;
using PackageManager4Apps.ImplemNuget.Extensions;
using Prism.Logging;

namespace PackageManager4Apps.ImplemNuget
{
    /// <summary>
    /// Implementation of a <see cref="IPackageManagingService"/> on a nuget server
    /// </summary>
    public class NugetPackageManagingService : IPackageManagingService
    {
        private readonly bool allowMultipleVersionOfPackagesInCache;
        private readonly DirectoryInfo localRepository;
        private readonly ILoggerFacade logger;
        private readonly Uri packageSourcePath;

        /// <summary>
        /// Create a new instance of <c>NugetPackageManagingService</c>
        /// </summary>
        /// <param name="packageSourcePath">Uri of the nuget server</param>
        /// <param name="localRepository"><see cref="DirectoryInfo"/> of the local nuget cache folder (folder may not exists, it will be created)</param>
        /// <param name="allowMultipleVersionOfPackagesInCache"><c>True</c>:allow to have multiple version of a package in cache; <c>False</c>:keep only the last version loaded</param>
        public NugetPackageManagingService(Uri packageSourcePath, DirectoryInfo localRepository, bool allowMultipleVersionOfPackagesInCache, ILoggerFacade logger)
        {
            if (!packageSourcePath.IsAbsoluteUri) throw new ArgumentException($"{nameof(packageSourcePath)} must be an absolute Uri");

            this.packageSourcePath = packageSourcePath;
            this.allowMultipleVersionOfPackagesInCache = allowMultipleVersionOfPackagesInCache;
            this.logger = logger;
            this.localRepository = localRepository;
        }
        
        /// <summary>
        /// See <see cref="IPackageManagingService.EnsurePackageLoaded"/>
        /// </summary>
        public void EnsurePackageLoaded(PackageMetadata packageInfo)
        {
            // Mutex sur 
            Mutex packageMutex = null;

            try
            {
                var mutexFirstCreation = false;
                var mutexName = ($"{nameof(NugetPackageManagingService)}-{localRepository.FullName}\\{packageInfo.PackageKey}-{packageInfo.Version}").Replace('\\','>');
                packageMutex = new Mutex(true, mutexName, out mutexFirstCreation);
                if (!mutexFirstCreation)
                    packageMutex.WaitOne();

                DoEnsurePackageLoaded(packageInfo);
            }
            finally
            {
                packageMutex?.ReleaseMutex();
            }
        }

        private void DoEnsurePackageLoaded(PackageMetadata packageInfo)
        {
            var packageRepository = PackageRepositoryFactory.Default.CreateRepository(packageSourcePath.AbsoluteUri);
            var packageManager = new NuGet.PackageManager(packageRepository, GetCacheFolderPath());
            var packageId = packageInfo.PackageKey;
            var packageSemanticVersion = packageInfo.ToSemanticVersion();
            var corrId = Guid.NewGuid();

            this.LogPackageOperation(packageId, packageSemanticVersion, "Package needed", corrId);

            var currentPackages = packageManager.LocalRepository.GetPackages().Where(p => p.Id == packageId).ToList();

            var installedPackage = EnsurePackageInstalled(currentPackages, packageManager, packageId, packageSemanticVersion, corrId);
            EnsureMultiplePackagesRespected(currentPackages, packageManager, packageId, packageSemanticVersion, corrId);
            EnsureMainAssemblyLoadedInAppDomain(packageManager.PathResolver, installedPackage, packageInfo, corrId);
        }

        private const string DefaultAssemblyExtensionLowerCase = ".dll";
        private const string PackageMainAssemblySubFolderPath = @"lib\";

        private void EnsureMainAssemblyLoadedInAppDomain(IPackagePathResolver pathResolver, IPackage package, PackageMetadata packageInfo, Guid correlationId)
        {
            var mainAssemblyFileName = $"{PackageMainAssemblySubFolderPath}{packageInfo.PackageKey}{DefaultAssemblyExtensionLowerCase}";
            var mainAssemblyPackageFile =
                    package
                    .GetLibFiles()
                    .FirstOrDefault(libFile => libFile.Path.Equals(mainAssemblyFileName, StringComparison.InvariantCultureIgnoreCase));

            if (mainAssemblyPackageFile == null)
            {
                this.LogPackageOperation(packageInfo.PackageKey, package.Version, $"No assembly {mainAssemblyFileName} found in {package.GetFullName()}", correlationId);
                return;
            }
            
            var mainAssemblyFullPath = Path.Combine(pathResolver.GetInstallPath(package), mainAssemblyPackageFile.Path);
            this.LogPackageOperation(packageInfo.PackageKey, package.Version, $"AppDomain.LoadAssembly:'{mainAssemblyFullPath}'", correlationId);
            AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(mainAssemblyFullPath));
        }

        private void EnsureMultiplePackagesRespected(IReadOnlyCollection<IPackage> currentPackages, NuGet.PackageManager packageManager, string packageId, SemanticVersion packageSemanticVersion, Guid corrId)
        {
            if (this.allowMultipleVersionOfPackagesInCache) return;
            const string disallowMultipleVersion = "(Multiple version in cache disabled)";

            var packagesToUninstall = currentPackages.Where(p => p.Version != packageSemanticVersion).ToList();
            if (!packagesToUninstall.Any()) return;

            foreach (var existingPackage in packagesToUninstall)
            {
                this.LogPackageOperation(existingPackage.Id, existingPackage.Version, $"Uninstall from cache {disallowMultipleVersion}", corrId);
                packageManager.UninstallPackage(existingPackage);
            }
        }

        private IPackage EnsurePackageInstalled(
            IReadOnlyCollection<IPackage> localCacheInstalledPackages, NuGet.PackageManager currentPackageManager,
            string packageId, SemanticVersion packageSemanticVersion,
            Guid corrId)
        {
            if (localCacheInstalledPackages.All(p => p.Version != packageSemanticVersion))
            {
                this.LogPackageOperation(packageId, packageSemanticVersion, "Install in local cache", corrId);
                currentPackageManager.InstallPackage(packageId, packageSemanticVersion);
            }
            else
                this.LogPackageOperation(packageId, packageSemanticVersion, "Already in cache", corrId);

            return currentPackageManager.LocalRepository.FindPackage(packageId, packageSemanticVersion);
        }

        private void LogPackageOperation(string packageId, SemanticVersion version, string operation,
            Guid correlationId, [CallerMemberName] string callerMemberName = null)
            => logger.Log(
                $"[{nameof(NugetPackageManagingService)}.{callerMemberName}(Package Id:{packageId}, Version:{version}, CorrelationId:{correlationId})] - {operation}", Category.Info, Priority.Medium);

        private string GetCacheFolderPath()
            => localRepository.FullName;
    }
}