using System;
using PackageManager4Apps.Nuget;

namespace AppPackageManager.Tests.PackageManagingServiceTestsResources
{
    /// <summary>
    /// Facade permettant l'exécution du test Multi version's tems cached au sein d'un appdomain séparé
    /// </summary>
    public class MultiVersionsPackageManagingServiceTestCase : MarshalByRefObject
    {
        private readonly PackageManagingServiceTest<NugetPackageManagingService> testedInstance;

        public MultiVersionsPackageManagingServiceTestCase()
        {
            testedInstance = TestInstancesFactory.CreateNugetPackageManagingServiceTest(true);
        }

        public void RunPackage1Version1()
            => testedInstance.RunPackage1Version1();

        public void RunPackage1Version2()
            => testedInstance.RunPackage1Version2();
    }
}