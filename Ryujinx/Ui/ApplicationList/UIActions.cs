using LibHac;
using LibHac.Account;
using LibHac.Common;
using LibHac.Fs;
using LibHac.Fs.Shim;
using LibHac.FsSystem;
using LibHac.FsSystem.NcaUtils;
using LibHac.Ncm;
using LibHac.Ns;
using Ryujinx.HLE.FileSystem;
using System;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Ryujinx.Ui
{
    public static class UIActions
    {
        private static VirtualFileSystem _virtualFileSystem;

        public static void Initialize(VirtualFileSystem virtualFileSystem)
        {
            _virtualFileSystem = virtualFileSystem;
        }

        public static UIActionResult OpenSaveDirectory(ApplicationData application)
        {
            if(!ulong.TryParse(application.TitleId, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out ulong titleId))
            {
                return UIActionResult.InvalidInput;
            }

            SaveDataFilter filter = new SaveDataFilter();

            filter.SetUserId(new UserId(1, 0));

            UIActionResult result = TryFindSaveData(titleId, filter, out ulong saveDataId, out string path);

            if(result == UIActionResult.Succcess)
            {
                Process.Start(new ProcessStartInfo()
                {
                    FileName = path,
                    UseShellExecute = true,
                    Verb = "open"
                });
            }

            return result;
        }

        private static UIActionResult TryFindSaveData(ulong titleId, SaveDataFilter filter, out ulong saveDataId, out string path)
        {
            saveDataId = default;
            path = string.Empty;

            Result result = _virtualFileSystem.FsClient.FindSaveDataWithFilter(out SaveDataInfo saveDataInfo, SaveDataSpaceId.User, ref filter);

            if (ResultFs.TargetNotFound.Includes(result))
            {
                return UIActionResult.DirectoryNotFound;
            }

            if (result.IsSuccess())
            {
                saveDataId = saveDataInfo.SaveDataId;

                path = GetSaveDataDirectory(saveDataId);

                if (Directory.Exists(path))
                {
                    return UIActionResult.Succcess;
                }
                return UIActionResult.DirectoryNotFound;
            }

            return UIActionResult.UnknownError;
        }

        private static string GetSaveDataDirectory(ulong saveDataId)
        {
            string saveRootPath = System.IO.Path.Combine(_virtualFileSystem.GetNandPath(), $"user/save/{saveDataId:x16}");

            if (!Directory.Exists(saveRootPath))
            {
                // Inconsistent state. Create the directory
                Directory.CreateDirectory(saveRootPath);
            }

            string committedPath = System.IO.Path.Combine(saveRootPath, "0");
            string workingPath = System.IO.Path.Combine(saveRootPath, "1");

            // If the committed directory exists, that path will be loaded the next time the savedata is mounted
            if (Directory.Exists(committedPath))
            {
                return committedPath;
            }

            // If the working directory exists and the committed directory doesn't,
            // the working directory will be loaded the next time the savedata is mounted
            if (!Directory.Exists(workingPath))
            {
                Directory.CreateDirectory(workingPath);
            }

            return workingPath;
        }

        public static UIActionResult OpenGameDirectory(ApplicationData application)
        {
            string gamePath = application.Path;

            if (File.Exists(gamePath))
            {
                string parentDirectory = new FileInfo(gamePath).DirectoryName;

                Process.Start(new ProcessStartInfo()
                {
                    FileName = parentDirectory,
                    UseShellExecute = true,
                    Verb = "open"
                });

                return UIActionResult.Succcess;
            }
            else return UIActionResult.FileNotFound;
        }
    }
}
