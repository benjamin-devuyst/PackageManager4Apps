using System;
using PackageManager4Apps;
using PackageManager4Apps.Nuget;

namespace AppPackageManager.Tests.PackageManagingServiceTestsResources
{
    /// <summary>
    /// Facade permettant l'exécution du test Single version's item cached au sein d'un appdomain séparé
    /// </summary>
    public class SingleVersionsPackageManagingServiceTestCase : MarshalByRefObject
    {
        private readonly PackageManagingServiceTest<NugetPackageManagingService> testedInstance;

        public SingleVersionsPackageManagingServiceTestCase()
        {
            testedInstance = testedInstance = TestInstancesFactory.CreateNugetPackageManagingServiceTest(false);
        }

        public void RunPackage1Version1()
            => testedInstance.RunPackage1Version1();

        public void RunPackage1Version2()
            => testedInstance.RunPackage1Version2();
    }
}