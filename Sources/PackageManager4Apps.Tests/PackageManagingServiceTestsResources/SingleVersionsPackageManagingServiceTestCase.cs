using System;
using PackageManager4Apps.ImplemNuget;

namespace PackageManager4Apps.Tests.PackageManagingServiceTestsResources
{
    /// <summary>
    /// /// Facade that allows the execution of the test 'Single Version' inside a separated appdomain
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