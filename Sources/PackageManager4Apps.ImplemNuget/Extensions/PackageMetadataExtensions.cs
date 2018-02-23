using NuGet;

namespace PackageManager4Apps.ImplemNuget.Extensions
{
    internal static class PackageMetadataExtensions
    {
        public static SemanticVersion ToSemanticVersion(this PackageMetadata source)
            => new SemanticVersion(source.Version, source.Suffix);
    }
}
