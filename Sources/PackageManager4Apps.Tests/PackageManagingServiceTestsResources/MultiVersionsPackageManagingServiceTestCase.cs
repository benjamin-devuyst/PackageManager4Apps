using System;
using PackageManager4Apps.ImplemNuget;

namespace PackageManager4Apps.Tests.PackageManagingServiceTestsResources
{
    /// <summary>
    /// Facade that allows the execution of the test 'Multi Version' inside a separated appdomain
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