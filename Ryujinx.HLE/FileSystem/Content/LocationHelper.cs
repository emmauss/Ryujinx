using System;
using System.Collections.Generic;
using System.Text;
using Ryujinx.HLE.FileSystem;
using System.IO;

using static Ryujinx.HLE.FileSystem.VirtualFileSystem;

namespace Ryujinx.HLE.FileSystem.Content
{
    internal static class LocationHelper
    {
        public static string GetRealPath(VirtualFileSystem FileSystem,string SwitchContentPath)
        {
            string BasePath = FileSystem.GetBasePath();

            switch (SwitchContentPath)
            {
                case ContentPath.SystemContent:
                    return Path.Combine(FileSystem.GetBasePath(),SystemNandPath, "Contents", "registered");
                case ContentPath.UserContent:
                    return Path.Combine(FileSystem.GetBasePath(), UserNandPath, "Contents", "registered");
                case ContentPath.SdCardContent:
                    return Path.Combine(FileSystem.GetSdCardPath(), "Nintendo", "Contents", "registered");
                case ContentPath.System:
                    return Path.Combine(BasePath, SystemNandPath);
                case ContentPath.User:
                    return Path.Combine(BasePath, UserNandPath);
                default:
                    throw new NotSupportedException($"Content Path `{SwitchContentPath}` is not supported.");
            }
        }

        public static string GetContentPath(ContentStorageId ContentStorageId)
        {
            switch (ContentStorageId)
            {
                case ContentStorageId.NandSystem:
                    return ContentPath.SystemContent;
                case ContentStorageId.NandUser:
                    return ContentPath.UserContent;
                case ContentStorageId.SdCard:
                    return ContentPath.SdCardContent;
                default:
                    throw new NotSupportedException($"Content Storage `{ContentStorageId}` is not supported.");
            }
        }

        public static StorageId GetStorageId(string ContentPathString)
        {
            switch (ContentPathString)
            {
                case ContentPath.SystemContent:
                case ContentPath.System:
                    return StorageId.NandSystem;

                case ContentPath.UserContent:
                case ContentPath.User:
                    return StorageId.NandUser;

                case ContentPath.SdCardContent:
                    return StorageId.SdCard;

                case ContentPath.Host:
                    return StorageId.Host;

                case ContentPath.GamecardApp:
                case ContentPath.GamecardContents:
                case ContentPath.GamecardUpdate:
                    return StorageId.GameCard;

                default:
                    return StorageId.None;
            }
        }
    }
}
