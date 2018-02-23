using System;
using System.Reflection;

namespace PackageManager4Apps.Tests
{
    /// <summary>
    /// Strategy that encapsulate the execution of a method in a separated appdomain
    /// </summary>
    public class SeparatedAppDomainExecuter
    {
        public void Run(Type strategyTypeToInstanciate, string methodNameToExecute, object[] arguments)
        {
            var appDomainSetup = new AppDomainSetup { ApplicationBase = Environment.CurrentDirectory };
            var appDomain = AppDomain.CreateDomain(methodNameToExecute, null, appDomainSetup);

            try
            {
                appDomain.UnhandledException += (sender, e) => throw (Exception)e.ExceptionObject;

                var unitTest =
                    appDomain
                        .CreateInstanceAndUnwrap(
                            strategyTypeToInstanciate.Assembly.GetName().Name,
                            strategyTypeToInstanciate.FullName, arguments);

                var methodToExecute = strategyTypeToInstanciate.GetMethod(methodNameToExecute, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (methodToExecute == null)
                    throw new ArgumentException(nameof(methodNameToExecute));

                try { methodToExecute.Invoke(unitTest, arguments); }
                catch (System.Reflection.TargetInvocationException e) { throw e.InnerException; }
            }
            finally
            {
                AppDomain.Unload(appDomain);
            }
        }
    }
}