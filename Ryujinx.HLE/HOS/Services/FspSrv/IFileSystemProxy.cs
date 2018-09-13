using Ryujinx.HLE.FileSystem;
using Ryujinx.HLE.HOS.Ipc;
using Ryujinx.HLE.HOS.SystemState;
using System;
using System.Collections.Generic;
using LibHac;
using System.Linq;
using Ryujinx.HLE.FileSystem.Content;
using System.IO;

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
                { 202,  OpenDataStorageByDataId                  },
                { 203,  OpenPatchDataStorageByCurrentProcess     },
                { 1005, GetGlobalAccessLogMode                   }
            };
        }

        private long OpenDataStorageByDataId(ServiceCtx Context)
        {
            StorageId StorageId = (StorageId)Context.RequestData.ReadByte();

            byte[] Padding = Context.RequestData.ReadBytes(7);

            long TitleId = Context.RequestData.ReadInt64();

            StorageId InstalledStorage = Context.Device.System.ContentManager.GetInstalledStorage(TitleId);

            if (InstalledStorage == StorageId)
            {
                string InstallPath =
                    Context.Device.System.ContentManager.GetInstalledPath(TitleId, ContentType.AocData);

                NcaId NcaId = Context.Device.System.ContentManager.GetInstalledNcaId(TitleId, ContentType.AocData);

                if (string.IsNullOrWhiteSpace(InstallPath))
                {
                    InstallPath = 
                        Context.Device.System.ContentManager.GetInstalledPath(TitleId, ContentType.Data);
                }

                if(NcaId == null)
                {
                    NcaId = Context.Device.System.ContentManager.GetInstalledNcaId(TitleId, ContentType.Data);
                }

                if (!string.IsNullOrWhiteSpace(InstallPath))
                {


                    string NcaPath = Path.Combine(InstallPath, NcaId.ToString() + ".nca", "00");

                    if (File.Exists(NcaPath))
                    {
                        FileStream NcaStream = new FileStream(NcaPath, FileMode.Open);
                        Nca Nca = new Nca(Context.Device.System.KeySet, NcaStream, true);

                        NcaSection RomfsSection = Nca.Sections.FirstOrDefault(x => x?.Type == SectionType.Romfs);

                        Stream RomfsStream = Nca.OpenSection(RomfsSection.SectionNum, false);

                        MakeObject(Context, new IStorage(RomfsStream));

                        return 0;
                    }
                    else
                        throw new FileNotFoundException($"No nca found in Path `{NcaPath}`.");
                }
                else
                    throw new DirectoryNotFoundException($"Path for title id {TitleId} on Storage {StorageId} was not found.");
            }

            throw new FileNotFoundException($"System archive with titleid {TitleId.ToString("x16")} was not found.");
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

            long TitleId = Context.RequestData.ReadInt64();

            UserId UserId = new UserId(
                Context.RequestData.ReadInt64(), 
                Context.RequestData.ReadInt64());

            long SaveId = Context.RequestData.ReadInt64();

            SaveDataType SaveDataType = (SaveDataType)Context.RequestData.ReadByte();

            SaveInfo SaveInfo = new SaveInfo(TitleId, SaveId, SaveDataType, UserId, SaveSpaceId);

            MakeObject(Context, new IFileSystem(Context.Device.FileSystem.GetGameSavePath(SaveInfo, Context)));
        }
    }
}