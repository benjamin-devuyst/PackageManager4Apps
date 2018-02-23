using System;
using System.Text.RegularExpressions;
using Prism.Logging;
using Prism.Modularity;

namespace PackageManager4Apps.Prism.Modularity
{
    /// <summary>
    /// Implementation of a Prism <see cref="T:Prism.Modularity.IModuleTypeLoader" /> on a <see cref="T:PackageManager4Apps.IPackageManagingService" />.
    /// This component allows to inject the IPackageManagingServices inside the Prism modularity engine.
    /// <para>
    /// This strategy will only take care of <see cref="T:Prism.Modularity.ModuleInfo" /> with a reference that match 'scheme://PackageName.MAJ.MIN.PATCH-SUFFIX' where the 'scheme' part is provided as constructor's argument 
    /// [PackageName] is package key (ASCII without space); [MAJ] Version.Major (positive int); [MIN] Version.Minor (positive int); 
    /// [PATCH] patch or revision (positive int); [SUFFIX] may not exists, used on pre-released packages (ASCII, no space).
    /// </para>
    /// See https://semver.org/ for more informations about versionning.
    /// </summary>
    public class PackageModuleTypeLoader : IModuleTypeLoader
    {
        private readonly IPackageManagingService packageManagingService;
        private readonly string scheme;
        private readonly ILoggerFacade logger;

        /// <summary>
        /// Create a new instance of <c>PackageModuleTypeLoader</c>
        /// </summary>
        /// <param name="packageManagingService">The Package Services to use behind</param>
        /// <param name="scheme">The scheme to match with when Prism use this type loader.</param>
        /// <param name="logger"></param>
        public PackageModuleTypeLoader(IPackageManagingService packageManagingService, string scheme, ILoggerFacade logger)
        {
            this.packageManagingService = packageManagingService;
            this.scheme = scheme;
            this.logger = logger;
        }

        private const string RegexExpression = @"^(?<SCHEME>[a-zA-Z0-9]+)://(?<PACKAGE>[0-9A-Za-z-\._]+)(?<Version>(\.(?<MAJOR>\d+))(\.(?<MINOR>\d+))(\.(?<PATCH>\d+))(-(?<SUFFIX>[0-9A-Za-z-\._]+)){0,1})$";

        /// <summary>
        /// Implementation of <see cref="IModuleTypeLoader.CanLoadModuleType"/>
        /// </summary>
        public bool CanLoadModuleType(ModuleInfo moduleInfo)
        {
            var match = Regex.Match(moduleInfo.Ref, RegexExpression);
            if (!match.Success) return false;
            return match.Groups["SCHEME"].Value.Equals(this.scheme, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Implementation of <see cref="IModuleTypeLoader.LoadModuleType"/>
        /// </summary>
        public void LoadModuleType(ModuleInfo moduleInfo)
        {
            var match = Regex.Match(moduleInfo.Ref, RegexExpression);
            if (!match.Success) return;

            var packageInfo = new PackageMetadata(
                match.Groups["PACKAGE"].Value,
                new Version(
                    Convert.ToInt32(match.Groups["MAJOR"].Value),
                    Convert.ToInt32(match.Groups["MINOR"].Value),
                    Convert.ToInt32(match.Groups["PATCH"].Value)),
                match.Groups["SUFFIX"].Value);

            Exception error = null;
            try
            {
                ModuleDownloadProgressChanged?.Invoke(this, new ModuleDownloadProgressChangedEventArgs(moduleInfo, 1000000, 2000000));
                this.packageManagingService.EnsurePackageLoaded(packageInfo);
            }
            catch (Exception e)
            {
                this.logger.Log($"[{nameof(PackageModuleTypeLoader)}.{nameof(LoadModuleType)}(scheme:'{scheme}', moduleRef:'{moduleInfo.Ref}')] - An error occurs during the loading of the package : {ExtractException(e)}", Category.Exception, Priority.High);
                error = e;
            }

            ModuleDownloadProgressChanged?.Invoke(this, new ModuleDownloadProgressChangedEventArgs(moduleInfo, 2000000, 2000000));
            LoadModuleCompleted?.Invoke(this, new LoadModuleCompletedEventArgs(moduleInfo, error));
        }

        /// <summary>
        /// Implementation of <see cref="IModuleTypeLoader.ModuleDownloadProgressChanged"/>
        /// </summary>
        public event EventHandler<ModuleDownloadProgressChangedEventArgs> ModuleDownloadProgressChanged;

        /// <summary>
        /// Implementation of <see cref="IModuleTypeLoader.LoadModuleCompleted"/>
        /// </summary>
        public event EventHandler<LoadModuleCompletedEventArgs> LoadModuleCompleted;

        private static string ExtractException(Exception input)
        {
            var result = $"{{exceptionType:'{input.GetType().FullName}', exceptionMessage:'{input.Message}', exceptionStackTrace:{input.StackTrace}";
            result = input.InnerException == null ? $"{result}}}" : $"{result}, innerException:\r\n     {ExtractException(input.InnerException)}}}";
            return result;
        }
    }
}
