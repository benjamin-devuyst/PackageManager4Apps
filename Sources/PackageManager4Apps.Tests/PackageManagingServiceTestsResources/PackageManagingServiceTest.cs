using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PackageManager4Apps.Tests.PackageManagingServiceTestsResources
{
    /// <summary>
    /// Strategy of the caching test and dynamic assembly loading.
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

        // Tuple Item1:PackageVersion, Item2:AssemblyVersion
        private void DoLoadPackage(string packageName, string mainAssemblyName, IReadOnlyCollection<Tuple<Version, Version>> versions)
        {
            CheckLoadedAssemblyInAppDomain(mainAssemblyName, versions, false, "Assembly already loaded : IMPOSSIBLE TO EXECUTE TEST");

            
            foreach (var version in versions)
                serviceToTest.EnsurePackageLoaded(new PackageMetadata(packageName, version.Item1));

            CheckCacheFolderState(packageName, versions.Select(v => v.Item1).ToList());
            CheckLoadedAssemblyInAppDomain(mainAssemblyName, versions, true, "Assembly not in AppDomain : NOT LOADED");
        }

        private void CheckCacheFolderState(string packageName, IReadOnlyCollection<Version> packageVersionInstalled)
        {
            var folders = this.cacheFolder.GetDirectories($"{packageName}.*");

            if (!this.allowMultipleVersionsInCache)
                Assert.IsFalse(folders.Length > 1, "More than one version is in the cache folder (this may also happen on debug - debugger blocks assemblies - ... wrong positive maybe?))");
        }

        // Tuple Item1:PackageVersion, Item2:AssemblyVersion
        private void CheckLoadedAssemblyInAppDomain(string assemblyName, IReadOnlyCollection<Tuple<Version ,Version>> versions, bool checkIsLoaded, string errMessage)
        {
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var version in versions)
                Assert.AreEqual(loadedAssemblies.Any(a => a.GetName().Name == assemblyName && a.GetName().Version == (version.Item2 ?? version.Item1)), checkIsLoaded, $"{errMessage} - Name:{assemblyName}, PackageVersion:{version.Item1}, AssemblyVersion:{version.Item2 ?? version.Item1}");
        }

        public void RunPackage1Version1()
        {
            DoLoadPackage( // package version is identical to assembly version
                packageName: "MApp.DemoModule",
                mainAssemblyName: "MApp.DemoModule",
                versions: new Tuple<Version, Version>[] // Tuple Item1:PackageVersion, Item2:AssemblyVersion
                {
                    new Tuple<Version,Version> (new Version(1, 0, 0, 0), new Version(1,0,0,0)),
                });
        }

        public void RunPackage1Version2()
        {
            DoLoadPackage( // package version is identical to assembly version
                packageName: "MApp.DemoModule",
                mainAssemblyName: "MApp.DemoModule",
                versions: new Tuple<Version, Version>[] // Tuple Item1:PackageVersion, Item2:AssemblyVersion
                {
                    new Tuple<Version,Version> (new Version(2, 0, 0, 0), new Version(2,0,0,0)),
                });
        }
    }
}
