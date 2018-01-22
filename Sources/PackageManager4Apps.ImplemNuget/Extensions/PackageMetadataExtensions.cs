using NuGet;

namespace PackageManager4Apps.Nuget.Extensions
{
    internal static class PackageMetadataExtensions
    {
        public static SemanticVersion ToSemanticVersion(this PackageMetadata source)
            => new SemanticVersion(source.Version, source.Suffix);
    }
}
