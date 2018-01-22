using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PackageManager4Apps;

namespace AppPackageManager.Tests.PackageManagingServiceTestsResources
{
    /// <summary>
    /// Stratégie de déroulement du test de caching et loading dynamique d'assemlies contenus dans un package
    /// </summary>
    internal class PackageManagingServiceTest<TService>
        where TService : class, IPackageManagingService
    {
        public PackageManagingServiceTest(IPackageManagingService testedService, DirectoryInfo cacheFolder, bool allowMultipleVersionsInCache)
        {
            this.serviceToTest = testedService;
            this.cacheFolder = cacheFolder;
            this.allowMultipleVersionsInCache = allowMultipleVersionsInCache;
        }

        protected readonly IPackageManagingService serviceToTest;
        private readonly DirectoryInfo cacheFolder;
        private readonly bool allowMultipleVersionsInCache;

        private void DoLoadPackage(string packageName, string mainAssemblyName, IReadOnlyCollection<(Version PackageVersion, Version)> versions)
        {
            CheckLoadedAssemblyInAppDomain(mainAssemblyName, versions, false, "Assembly already loaded : IMPOSSIBLE TO EXECUTE TEST");

            foreach (var version in versions)
                serviceToTest.EnsurePackageLoaded(new PackageMetadata(packageName, version.PackageVersion));

            CheckCacheFolderState(packageName, versions.Select(v => v.PackageVersion).ToList());
            CheckLoadedAssemblyInAppDomain(mainAssemblyName, versions, true, "Assembly not in AppDomain : NOT LOADED");
        }

        private void CheckCacheFolderState(string packageName, IReadOnlyCollection<Version> packageVersionInstalled)
        {
            var folders = this.cacheFolder.GetDirectories($"{packageName}.*");

            if (!this.allowMultipleVersionsInCache)
                Assert.IsFalse(folders.Length > 1, "More than one version is in the cache folder (this may also happen on debug - debugger blocks assemblies - ... wrong positive maybe?))");
        }

        private void CheckLoadedAssemblyInAppDomain(string assemblyName, IReadOnlyCollection<(Version PackageVersion, Version AssemblyVersion)> versions, bool checkIsLoaded, string errMessage)
        {
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var version in versions)
                Assert.AreEqual(loadedAssemblies.Any(a => a.GetName().Name == assemblyName && a.GetName().Version == (version.AssemblyVersion ?? version.PackageVersion)), checkIsLoaded, $"{errMessage} - Name:{assemblyName}, PackageVersion:{version.PackageVersion}, AssemblyVersion:{version.AssemblyVersion ?? version.PackageVersion}");
        }

        public void RunPackage1Version1()
        {
            DoLoadPackage( // version de packages nuget unity pour lesquelles la version de l'assembly Unity.Container est identique
                packageName: "MApp.DemoModule",
                mainAssemblyName: "MApp.DemoModule",
                versions: new(Version, Version)[]
                {
                    (new Version(1, 0, 0, 0), new Version(1,0,0,0)),
                });
        }

        public void RunPackage1Version2()
        {
            DoLoadPackage( // version de packages nuget unity pour lesquelles la version de l'assembly Unity.Container est identique
                packageName: "MApp.DemoModule",
                mainAssemblyName: "MApp.DemoModule",
                versions: new(Version, Version)[]
                {
                    (new Version(2, 0, 0, 0), new Version(2,0,0,0)),
                });
        }
    }
}
