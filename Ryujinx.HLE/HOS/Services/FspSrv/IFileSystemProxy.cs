using Ryujinx.HLE.FileSystem;
using Ryujinx.HLE.HOS.Ipc;
using Ryujinx.HLE.HOS.SystemState;
using System.Collections.Generic;

namespace Ryujinx.HLE.HOS.Services.FspSrv
{
    class IFileSystemProxy : IpcService
    {
        private Dictionary<int, ServiceProcessRequest> m_Commands;

        public override IReadOnlyDictionary<int, ServiceProcessRequest> Commands => m_Commands;

        public IFileSystemProxy()
        {
            m_Commands = new Dictionary<int, ServiceProcessRequest>()
            {
                { 1,    SetCurrentProcess                        },
                { 18,   OpenSdCardFileSystem                     },
                { 51,   OpenSaveDataFileSystem                   },
                { 52,   OpenSaveDataFileSystemBySystemSaveDataId },
                { 200,  OpenDataStorageByCurrentProcess          },
                { 203,  OpenPatchDataStorageByCurrentProcess     },
                { 1005, GetGlobalAccessLogMode                   }
            };
        }

        public long SetCurrentProcess(ServiceCtx Context)
        {
            return 0;
        }

        public long OpenSdCardFileSystem(ServiceCtx Context)
        {
            MakeObject(Context, new IFileSystem(Context.Device.FileSystem.GetSdCardPath()));

            return 0;
        }

        public long OpenSaveDataFileSystem(ServiceCtx Context)
        {
            LoadSaveDataFileSystem(Context);

            return 0;
        }

        public long OpenSaveDataFileSystemBySystemSaveDataId(ServiceCtx Context)
        {
            LoadSaveDataFileSystem(Context);

            return 0;
        }

        public long OpenDataStorageByCurrentProcess(ServiceCtx Context)
        {
            MakeObject(Context, new IStorage(Context.Device.FileSystem.RomFs));

            return 0;
        }

        public long OpenPatchDataStorageByCurrentProcess(ServiceCtx Context)
        {
            MakeObject(Context, new IStorage(Context.Device.FileSystem.RomFs));

            return 0;
        }

        public long GetGlobalAccessLogMode(ServiceCtx Context)
        {
            Context.ResponseData.Write(0);

            return 0;
        }

        public void LoadSaveDataFileSystem(ServiceCtx Context)
        {
            SaveSpaceId SaveSpaceId = (SaveSpaceId)Context.RequestData.ReadInt64();

            SaveInfo SaveInfo = new SaveInfo(Context, SaveSpaceId);

            MakeObject(Context, new IFileSystem(Context.Device.FileSystem.GetGameSavePath(SaveInfo, Context)));
        }
    }
}