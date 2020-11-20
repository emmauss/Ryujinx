using LibHac;
using LibHac.Account;
using LibHac.Common;
using LibHac.Fs;
using LibHac.Fs.Shim;
using LibHac.FsSystem;
using LibHac.FsSystem.NcaUtils;
using LibHac.Ncm;
using LibHac.Ns;
using Ryujinx.Common.Configuration;
using Ryujinx.Common.Logging;
using Ryujinx.Common.Utilities;
using Ryujinx.HLE.FileSystem;
using Ryujinx.Skia.Ui;
using Ryujinx.Skia.Ui.Skia.Scene;
using Ryujinx.Skia.Ui.Skia.Widget;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using static LibHac.Fs.ApplicationSaveDataManagement;

namespace Ryujinx.Skia.App
{
    public static class ApplicationHelper
    {
        private static VirtualFileSystem _virtualFileSystem;

        public static void Initialize(VirtualFileSystem virtualFileSystem)
        {
            _virtualFileSystem = virtualFileSystem;
        }

        private static bool TryFindSaveData(string titleName, ulong titleId, BlitStruct<ApplicationControlProperty> controlHolder, SaveDataFilter filter, out ulong saveDataId)
        {
            saveDataId = default;

            var activeScene = ((IManager.Instance) as SKWindow).ActiveScene;

            MessageDialog dialog = null;

            Result result = _virtualFileSystem.FsClient.FindSaveDataWithFilter(out SaveDataInfo saveDataInfo, SaveDataSpaceId.User, ref filter);

            if (ResultFs.TargetNotFound.Includes(result))
            {
                // Savedata was not found. Ask the user if they want to create it */
                var messageDialog = new MessageDialog(activeScene,
                        "Ryujinx",
                        $"There is no savedata for {titleName} [{titleId:x16}]",
                        "Would you like to create savedata for this game?",
                        DialogButtons.Yes | DialogButtons.No);

                messageDialog.Run();

                if(messageDialog.DialogResult == "No")
                {
                    return false;
                }

                ref ApplicationControlProperty control = ref controlHolder.Value;

                if (LibHac.Utilities.IsEmpty(controlHolder.ByteSpan))
                {
                    // If the current application doesn't have a loaded control property, create a dummy one
                    // and set the savedata sizes so a user savedata will be created.
                    control = ref new BlitStruct<ApplicationControlProperty>(1).Value;

                    // The set sizes don't actually matter as long as they're non-zero because we use directory savedata.
                    control.UserAccountSaveDataSize = 0x4000;
                    control.UserAccountSaveDataJournalSize = 0x4000;

                    Logger.Warning?.Print(LogClass.Application,
                        "No control file was found for this game. Using a dummy one instead. This may cause inaccuracies in some games.");
                }

                Uid user = new Uid(1, 0);

                result = EnsureApplicationSaveData(_virtualFileSystem.FsClient, out _, new LibHac.Ncm.ApplicationId(titleId), ref control, ref user);

                if (result.IsFailure())
                {
                    dialog = new MessageDialog(activeScene,
                             "Ryujinx - Error",
                             "Ryujinx has encountered an error",
                             $"There was an error creating the specified savedata: {result.ToStringWithName()}",
                             DialogButtons.OK);

                    dialog.Run();

                    return false;
                }

                // Try to find the savedata again after creating it
                result = _virtualFileSystem.FsClient.FindSaveDataWithFilter(out saveDataInfo, SaveDataSpaceId.User, ref filter);
            }

            if (result.IsSuccess())
            {
                saveDataId = saveDataInfo.SaveDataId;

                return true;
            }

            dialog = new MessageDialog(activeScene,
                             "Ryujinx - Error",
                             "Ryujinx has encountered an error",
                             $"There was an error finding the specified savedata: {result.ToStringWithName()}",
                             DialogButtons.OK);

            dialog.Run();

            return false;
        }

        public static string GetSaveDataDirectory(ulong saveDataId)
        {
            string saveRootPath = System.IO.Path.Combine(_virtualFileSystem.GetNandPath(), $"user/save/{saveDataId:x16}");

            if (!Directory.Exists(saveRootPath))
            {
                // Inconsistent state. Create the directory
                Directory.CreateDirectory(saveRootPath);
            }

            string committedPath = System.IO.Path.Combine(saveRootPath, "0");
            string workingPath   = System.IO.Path.Combine(saveRootPath, "1");

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

        public static void OpenSaveDir(string titleName, BlitStruct<ApplicationControlProperty> controlData, ulong titleId, SaveDataFilter filter)
        {
            filter.SetProgramId(new ProgramId(titleId));

            if (!TryFindSaveData(titleName, titleId, controlData, filter, out ulong saveDataId))
            {
                return;
            }

            string saveDir = GetSaveDataDirectory(saveDataId);

            Process.Start(new ProcessStartInfo
            {
                FileName = saveDir,
                UseShellExecute = true,
                Verb = "open"
            });
        }
    }
}