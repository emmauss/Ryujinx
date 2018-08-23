using Ryujinx.HLE.HOS.Ipc;
using Ryujinx.HLE.Logging;
using System.Collections.Generic;
using Ryujinx.HLE.FileSystem;

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
                { 22,   CreateSaveDataFileSystem                 },
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

        public long CreateSaveDataFileSystem(ServiceCtx Context)
        {
            long TitleId    = Context.RequestData.ReadInt64();
            long UserIdHigh = Context.RequestData.ReadInt64();
            long UserIdLow  = Context.RequestData.ReadInt64();
            long SaveId     = Context.RequestData.ReadInt64();

            SaveDataType Type = (SaveDataType)Context.RequestData.ReadByte();

            Save SaveInfo = new Save()
            {
                TitleId      = TitleId,
                UserID       = new SystemState.UserId(UserIdLow, UserIdHigh),
                SaveID       = SaveId,
                SaveDataType = Type
            };

            switch (SaveInfo.SaveDataType)
            {
                case SaveDataType.SaveData:
                    SaveInfo.SaveSpaceId = SaveSpaceId.NandUser;
                    break;
                case SaveDataType.SystemSaveData:
                    SaveInfo.SaveSpaceId = SaveSpaceId.NandSystem;
                    break;
            }

            byte[] SaveCreateStruct = Context.RequestData.ReadBytes(0x40);
            byte[] Input            = Context.RequestData.ReadBytes(0x10);

            Context.Device.FileSystem.GetGameSavesPath(SaveInfo, Context);

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
            byte id = Context.RequestData.ReadByte();
            SaveSpaceId SaveSpaceId = (SaveSpaceId)id;

            int Unknown = Context.RequestData.ReadInt32();

            long   TitleId = Context.RequestData.ReadInt64();
            byte[] UserId  = Context.RequestData.ReadBytes(0x10);
            long   SaveId  = Context.RequestData.ReadInt64();

            byte type = Context.RequestData.ReadByte();
            SaveDataType Type = (SaveDataType)type;

            Save SaveInfo = new Save()
            {
                TitleId      = TitleId,
                UserID       = new SystemState.UserId(UserId),
                SaveID       = SaveId,
                SaveDataType = Type
            };

            SaveInfo.SaveSpaceId = SaveSpaceId;

            MakeObject(Context, new IFileSystem(Context.Device.FileSystem.GetGameSavesPath(SaveInfo, Context)));
        }
    }
}