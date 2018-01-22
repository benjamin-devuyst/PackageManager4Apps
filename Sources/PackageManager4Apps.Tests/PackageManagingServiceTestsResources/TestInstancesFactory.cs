using System;
using System.Diagnostics;
using System.IO;
using PackageManager4Apps;
using PackageManager4Apps.Nuget;

namespace AppPackageManager.Tests.PackageManagingServiceTestsResources
{
    internal static class TestInstancesFactory
    {
        private const string CacheFolderName = "AppPackageCache";

        public static PackageManagingServiceTest<NugetPackageManagingService> CreateNugetPackageManagingServiceTest(bool allowMultipleVersionsInCache)
        {
            var cacheFolder = CreateCacheTestsDirectoryInfo();
            var service = TestInstancesFactory.CreateCachedPackageManagingService(
                allowMultipleVersionsInCache,
                cacheFolder);

            var result = new PackageManagingServiceTest<NugetPackageManagingService>(
                service,
                cacheFolder,
                allowMultipleVersionsInCache);

            return result;
        }

        public static IPackageManagingService CreateCachedPackageManagingService(bool allowMultipleVersionsInCache, DirectoryInfo cache)
        {
            var execFolder = new FileInfo(typeof(TestInstancesFactory).Assembly.ManifestModule.FullyQualifiedName).Directory.FullName;
            return new NugetPackageManagingService(
                new Uri(Path.Combine(execFolder, "LocalNugetNupkg"), UriKind.Absolute),
                cache,
                allowMultipleVersionsInCache,
                m => Debug.WriteLine(m));
        }

        public static DirectoryInfo CreateCacheTestsDirectoryInfo()
            => new DirectoryInfo(
                Path.Combine(
                    System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    CacheFolderName));
    }
}