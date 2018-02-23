using System;
using System.IO;
using PackageManager4Apps.ImplemNuget;

namespace PackageManager4Apps.Tests.PackageManagingServiceTestsResources
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
            return new NugetPackageManagingService(new Uri(Path.Combine(execFolder, "LocalNugetNupkg"), UriKind.Absolute), cache, allowMultipleVersionsInCache,
                new TestLogger());
        }

        public static DirectoryInfo CreateCacheTestsDirectoryInfo()
            => new DirectoryInfo(
                Path.Combine(
                    System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    CacheFolderName));
    }
}