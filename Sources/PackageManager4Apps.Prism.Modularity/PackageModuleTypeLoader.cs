using System;
using System.Text.RegularExpressions;
using PackageManager4Apps;
using Prism.Modularity;

namespace AppPackageManager.Prism.Modularity
{
    /// <summary>
    /// Implémentation d'un <see cref="T:Prism.Modularity.IModuleTypeLoader" /> Prism sur le composant <see cref="T:AppPackageManager.IPackageManagingService" />.
    /// Ceci permet d'injecter la récupération de modules d'un Package Manager dans Prism.
    /// <para>
    /// Cette stratégie ne traitera que les <see cref="T:Prism.Modularity.ModuleInfo" /> dont la ref correspond au pattern scheme://PackageName.MAJ.MIN.PATCH-SUFFIX où le scheme est fourni par ctor et 
    /// [PackageName] correspond au nom du package (Alphanumérique ASCII sans espace); [MAJ] numéro de version Majeur (entier positif); [MIN] numéro de version Mineur (entier positif); 
    /// [PATCH] numéro de patch; [SUFFIX] correspond à une chaine Alphanumérique ASCII sans espace.
    /// </para>
    /// </summary>
    public class PackageModuleTypeLoader : IModuleTypeLoader
    {
        private readonly IPackageManagingService packageManagingService;
        private readonly string scheme;

        public PackageModuleTypeLoader(IPackageManagingService packageManagingService, string scheme)
        {
            this.packageManagingService = packageManagingService;
            this.scheme = scheme;
        }

        public bool CanLoadModuleType(ModuleInfo moduleInfo)
        {
            var match = Regex.Match(moduleInfo.Ref, @"^(?<SCHEME>[a-zA-Z0-9]+)://(?<PACKAGE>[0-9A-Za-z-\._]+)(?<Version>(\.(?<MAJOR>\d+))(\.(?<MINOR>\d+))(\.(?<PATCH>\d+))(-(?<SUFFIX>[0-9A-Za-z-\._]+)){0,1})$");
            if (!match.Success) return false;
            return match.Groups["SCHEME"].Value.Equals(this.scheme, StringComparison.InvariantCultureIgnoreCase);
        }

        public void LoadModuleType(ModuleInfo moduleInfo)
        {
            var match = Regex.Match(moduleInfo.Ref, @"^(?<SCHEME>[a-zA-Z0-9]+)://(?<PACKAGE>[0-9A-Za-z-\._]+)(?<Version>(\.(?<MAJOR>\d+))(\.(?<MINOR>\d+))(\.(?<PATCH>\d+))(-(?<SUFFIX>[0-9A-Za-z-\._]+)){0,1})$");
            if (!match.Success) return;

            var packageInfo = new PackageMetadata(
                match.Groups["PACKAGE"].Value,
                new Version(
                    Convert.ToInt32(match.Groups["MAJOR"].Value),
                    Convert.ToInt32(match.Groups["MINOR"].Value),
                    Convert.ToInt32(match.Groups["PATCH"].Value)),
                match.Groups["SUFFIX"].Value);

            Exception error = null;

            ModuleDownloadProgressChanged?.Invoke(this, new ModuleDownloadProgressChangedEventArgs(moduleInfo, 1000000, 2000000));
            this.packageManagingService.EnsurePackageLoaded(packageInfo);

            ModuleDownloadProgressChanged?.Invoke(this, new ModuleDownloadProgressChangedEventArgs(moduleInfo, 2000000, 2000000));
            LoadModuleCompleted?.Invoke(this, new LoadModuleCompletedEventArgs(moduleInfo, error));
        }

        public event EventHandler<ModuleDownloadProgressChangedEventArgs> ModuleDownloadProgressChanged;

        public event EventHandler<LoadModuleCompletedEventArgs> LoadModuleCompleted;
    }
}
