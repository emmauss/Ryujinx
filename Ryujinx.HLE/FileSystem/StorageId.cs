namespace Ryujinx.HLE.FileSystem
{
    internal enum StorageId : byte
    {
        None,
        Host,
        GameCard,
        NandSystem,
        NandUser,
        SdCard
    }
}
