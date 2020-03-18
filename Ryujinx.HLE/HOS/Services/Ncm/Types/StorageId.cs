namespace Ryujinx.HLE.HOS.Services.Ncm.Types
{
    public enum StorageId : byte
    {
        None          = 0,
        Host          = 1,
        GameCard      = 2,
        BuiltInSystem = 3,
        BuiltInUser   = 4,
        SdCard        = 5,
        Any           = 6,
    }

    public static class StorageIdExtensions
    {
        public static bool IsInstallableStorage(this StorageId storageId)
        {
            return (storageId == StorageId.BuiltInSystem) || 
                   (storageId == StorageId.BuiltInUser)   || 
                   (storageId == StorageId.SdCard)        || 
                   (storageId == StorageId.Any);
        }

        public static bool IsUniqueStorage(this StorageId storageId)
        {
            return (storageId != StorageId.None) &&
                   (storageId != StorageId.Any);
        }
    }
}