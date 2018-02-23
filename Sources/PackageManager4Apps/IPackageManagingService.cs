namespace PackageManager4Apps
{
    /// <summary>
    /// Abstraction of the Package Manager used behind 
    /// </summary>
    public interface IPackageManagingService
    {
        /// <summary>
        /// Method to ensure that the package is loaded and uncompressed locally and ready to be used in the AppDomain (package main assembly is loaded in AppDomain).
        /// </summary>
        /// <param name="packageInfo"></param>
        void EnsurePackageLoaded(PackageMetadata packageInfo);
    }
}
