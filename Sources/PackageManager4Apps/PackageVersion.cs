using System;

namespace PackageManager4Apps
{
    /// <summary>
    /// Define a package loadable by the <see cref="IPackageManagingService"/>
    /// </summary>
    public class PackageMetadata
    {
        private static readonly Version MostRecentVersion = new Version(1, 0);

        /// <summary>
        /// Create a new <c>PackageMetadata</c>
        /// </summary>
        /// <param name="packageKey">Key that identify the package in the Package Manager</param>
        /// <param name="version">Version to load</param>
        public PackageMetadata(string packageKey, Version version)
            : this(packageKey, version, null)
        {

        }

        /// <summary>
        /// Create a new <c>PackageMetadata</c>
        /// </summary>
        /// <param name="packageKey">Key that identify the package in the Package Manager</param>
        /// <param name="version">Version to load</param>
        /// <param name="suffix">Package suffix used to specify a pre-released package (alpha, beta, etc.)</param>
        public PackageMetadata(string packageKey, Version version, string suffix)
        {
            this.PackageKey = packageKey;
            Version = version;
            Suffix = suffix?.Trim() ?? string.Empty;
        }

        /// <summary>
        /// Gets the key that identify the package in the Package Manager
        /// </summary>
        public string PackageKey { get; }

        /// <summary>
        /// Gets the package Version
        /// </summary>
        public Version Version { get; }

        /// <summary>
        /// Gets the suffix (if pre-release package, string.empty otherwise)
        /// </summary>
        public string Suffix { get; }
    }
}