using Microsoft.VisualStudio.TestTools.UnitTesting;
using PackageManager4Apps.Tests.PackageManagingServiceTestsResources;

namespace PackageManager4Apps.Tests
{
    [TestClass]
    public class CachedNugetPackageManagingServiceTest
    {
        public CachedNugetPackageManagingServiceTest()
        {

        }

        [TestMethod]
        public void EnsurePackageLoadedAsync_SingleVersionAtATimeInCache()
        {
            // We need to execute tests on a separated AppDomain to ensure the isolation of the assemblies loaded in appdomain
            // each execution need a clean appdomain
            var executer = new SeparatedAppDomainExecuter();

            executer.Run(typeof(SingleVersionsPackageManagingServiceTestCase), nameof(SingleVersionsPackageManagingServiceTestCase.RunPackage1Version1), null);
            executer.Run(typeof(SingleVersionsPackageManagingServiceTestCase), nameof(SingleVersionsPackageManagingServiceTestCase.RunPackage1Version2), null);
        }

        [TestMethod]
        public void EnsurePackageLoadedAsync_MultiVersionAtATimeInCache()
        {
            // We need to execute tests on a separated AppDomain to ensure the isolation of the assemblies loaded in appdomain
            // each execution need a clean appdomain
            var executer = new SeparatedAppDomainExecuter();

            executer.Run(typeof(MultiVersionsPackageManagingServiceTestCase), nameof(MultiVersionsPackageManagingServiceTestCase.RunPackage1Version1), null);
            executer.Run(typeof(MultiVersionsPackageManagingServiceTestCase), nameof(MultiVersionsPackageManagingServiceTestCase.RunPackage1Version2), null);
        }
    }
}