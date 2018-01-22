namespace PackageManager4Apps
{
    public interface IPackageManagingService
    {
        void EnsurePackageLoaded(PackageMetadata packageInfo);
    }
}
