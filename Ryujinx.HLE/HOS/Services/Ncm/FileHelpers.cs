using Ryujinx.HLE.HOS.Services.Ncm.Types;
using Ryujinx.HLE.Utilities;
using System;
using System.IO;

namespace Ryujinx.HLE.HOS.Services.Ncm
{
    public static class FileHelpers
    {
        public static string MakePath(UInt128 id, StorageId storageId)
        {
            string baseDirectory = string.Empty;

            switch (storageId)
            {
                case StorageId.BuiltInSystem:
                    baseDirectory = MakeFlatDirectoryPath(id);
                    break;
                default:
                    baseDirectory = MakeSha256DirectoryPath(id);
                    break;
            }

            return $"{baseDirectory}.nca";
        }
        public static string MakePlaceHolderPath(UInt128 placeholderId, string rootPath, StorageId storageId)
        {
            return Path.Combine(rootPath, MakePath(placeholderId, storageId));
        }

        public static string MakeContentPath(UInt128 contentId, string rootPath, StorageId storageId)
        {
            return Path.Combine(rootPath, MakePath(contentId, storageId));
        }

        public static string MakeFlatDirectoryPath(UInt128 id)
        {
            return $"{id}.nca";
        }

        public static string MakeSha256DirectoryPath(UInt128 id)
        {
            string path = $"{id}.nca";

            Span<byte> hash = new Span<byte>(new byte[0x10]);

            LibHac.Crypto.Sha256.GenerateSha256Hash(id.ToBytes().AsSpan(), hash);

            BitConverter.ToString(hash.Slice(0, 8).ToArray()).Replace("-", string.Empty);

            string hashPath = $"{BitConverter.ToString(hash.Slice(0, 8).ToArray()).Replace("-", string.Empty)}/";

            hashPath += $"{BitConverter.ToString(hash.Slice(8, 8).ToArray()).Replace("-", string.Empty)}";
            
            return Path.Combine(hashPath, path);
        }
    }
}