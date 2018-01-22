using System;

namespace PackageManager4Apps
{
    public class PackageMetadata
    {
        private static readonly Version MostRecentVersion = new Version(1, 0);

        public PackageMetadata(string packageKey, Version version)
            : this(packageKey, version, null)
        {

        }

        public PackageMetadata(string packageKey, Version version, string suffix)
        {
            this.PackageKey = packageKey;
            Version = version;
            Suffix = suffix?.Trim() ?? string.Empty;
        }

        public string PackageKey { get; }

        public Version Version { get; }

        public string Suffix { get; }
    }
}