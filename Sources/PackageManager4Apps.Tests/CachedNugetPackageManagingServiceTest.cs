using AppPackageManager.Tests.PackageManagingServiceTestsResources;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AppPackageManager.Tests
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
            // nécessité d'exécuter dans un appdomain séparé pour ne pas être influencé par les autres tests
            // car on manipule le chargement d'assemblies dans l'appdomain.
            // le test nécessite un appdomain clean pour valider les asserts.
            var executer = new SeparatedAppDomainExecuter();

            executer.Run(typeof(SingleVersionsPackageManagingServiceTestCase), nameof(SingleVersionsPackageManagingServiceTestCase.RunPackage1Version1), null);
            executer.Run(typeof(SingleVersionsPackageManagingServiceTestCase), nameof(SingleVersionsPackageManagingServiceTestCase.RunPackage1Version2), null);
        }

        [TestMethod]
        public void EnsurePackageLoadedAsync_MultiVersionAtATimeInCache()
        {
            // nécessité d'exécuter dans un appdomain séparé pour ne pas être influencé par les autres tests
            // car on manipule le chargement d'assemblies dans l'appdomain.
            // le test nécessite un appdomain clean pour valider les asserts.
            var executer = new SeparatedAppDomainExecuter();

            executer.Run(typeof(MultiVersionsPackageManagingServiceTestCase), nameof(MultiVersionsPackageManagingServiceTestCase.RunPackage1Version1), null);
            executer.Run(typeof(MultiVersionsPackageManagingServiceTestCase), nameof(MultiVersionsPackageManagingServiceTestCase.RunPackage1Version2), null);
        }
    }
}