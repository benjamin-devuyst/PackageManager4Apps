using System;
using System.Diagnostics;
using Prism.Logging;

namespace PackageManager4Apps.Tests
{
    internal class TestLogger : ILoggerFacade
    {
        public void Log(string message, Category category, Priority priority) 
            => Debug.WriteLine($"*****[{DateTime.Now:s}][{category}] {message}");
    }
}
