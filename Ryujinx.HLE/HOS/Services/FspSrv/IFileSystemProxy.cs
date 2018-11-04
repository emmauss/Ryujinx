using LibHac;
using Ryujinx.HLE.FileSystem;
using Ryujinx.HLE.HOS.Ipc;
using Ryujinx.HLE.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using static Ryujinx.HLE.FileSystem.VirtualFileSystem;
using static Ryujinx.HLE.HOS.ErrorCode;
using static Ryujinx.HLE.Utilities.StringUtils;

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
                { 8,    OpenFileSystemWithId                     },
                { 11,   OpenBisFileSystem                        },
                { 18,   OpenSdCardFileSystem                     },
                { 51,   OpenSaveDataFileSystem                   },
                { 52,   OpenSaveDataFileSystemBySystemSaveDataId },
                { 200,  OpenDataStorageByCurrentProcess          },
                { 202,  OpenDataStorageByDataId                  },
                { 203,  OpenPatchDataStorageByCurrentProcess     },
                { 1005, GetGlobalAccessLogMode                   }
            };
        }

        public long SetCurrentProcess(ServiceCtx Context)
        {
            return 0;
        }

        public long OpenBisFileSystem(ServiceCtx Context)
        {
            int BisPartitionId = Context.RequestData.ReadInt32();

            string PartitionString = ReadUtf8String(Context);

            string BisPartitonPath = string.Empty;

            switch (BisPartitionId)
            {
                case 29:
                    BisPartitonPath = SafeNandPath;
                    break;
                case 30:
                case 31:
                    BisPartitonPath = SystemNandPath;
                    break;
                case 32:
                    BisPartitonPath = UserNandPath;
                    break;
                default:
                    return MakeError(ErrorModule.Fs, FsErr.InvalidInput);
            }

            string FullPath = Context.Device.FileSystem.GetFullPartitionPath(BisPartitonPath);

            FileSystemProvider FileSystemProvider = new FileSystemProvider(FullPath, Context.Device.FileSystem.GetBasePath());

            MakeObject(Context, new IFileSystem(FullPath, FileSystemProvider));

            return 0;
        }

        public long OpenFileSystemWithId(ServiceCtx Context)
        {
            FileSystemType FileSystemType = (FileSystemType)Context.RequestData.ReadInt32();

            long TitleId = Context.RequestData.ReadInt64();

            string SwitchPath = ReadUtf8String(Context);

            string FullPath = Context.Device.FileSystem.SwitchPathToSystemPath(SwitchPath);

            FileStream FileStream = new FileStream(FullPath, FileMode.Open, FileAccess.Read);

            string Extension = Path.GetExtension(FullPath);

            if (Extension == ".nca")
            {
                Nca Nca = new Nca(Context.Device.System.KeySet, FileStream, false);

                NcaSection RomfsSection = Nca.Sections.FirstOrDefault(x => x?.Type == SectionType.Romfs);

                if (RomfsSection != null)
                {
                    Stream RomfsStream = Nca.OpenSection(RomfsSection.SectionNum, false, Context.Device.System.FsIntegrityCheckLevel);

                    IFileSystem NcaFileSystem = new IFileSystem(SwitchPath, new RomFsProvider(RomfsStream));

                    MakeObject(Context, NcaFileSystem);

                    return 0;
                }
            }
            else if (Extension == ".nsp")
            {
                Pfs Nsp = new Pfs(FileStream);

                PfsFileEntry TicketFile = Nsp.Files.FirstOrDefault(x => x.Name.EndsWith(".tik"));

                if (TicketFile != null)
                {
                    Ticket Ticket = new Ticket(Nsp.OpenFile(TicketFile));

                    Context.Device.System.KeySet.TitleKeys[Ticket.RightsId] =
                        Ticket.GetTitleKey(Context.Device.System.KeySet);
                }

                IFileSystem NspFileSystem = new IFileSystem(SwitchPath, new PFsProvider(Nsp));

                MakeObject(Context, NspFileSystem);

                return 0;
            }

            return MakeError(ErrorModule.Fs, FsErr.InvalidInput);
        }

        public long OpenSdCardFileSystem(ServiceCtx Context)
        {
            string SdCardPath = Context.Device.FileSystem.GetSdCardPath();

            FileSystemProvider FileSystemProvider = new FileSystemProvider(SdCardPath, Context.Device.FileSystem.GetBasePath());

            MakeObject(Context, new IFileSystem(SdCardPath, FileSystemProvider));

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

        public long OpenDataStorageByDataId(ServiceCtx Context)
        {
            StorageId StorageId = (StorageId)Context.RequestData.ReadByte();

            byte[] Padding = Context.RequestData.ReadBytes(7);

            long TitleId = Context.RequestData.ReadInt64();

            StorageId InstalledStorage =
                Context.Device.System.ContentManager.GetInstalledStorage(TitleId, ContentType.Data, StorageId);

            if (InstalledStorage == StorageId.None)
            {
                InstalledStorage =
                    Context.Device.System.ContentManager.GetInstalledStorage(TitleId, ContentType.AocData, StorageId);
            }

            if (InstalledStorage != StorageId.None)
            {
                string InstallPath =
                    Context.Device.System.ContentManager.GetInstalledPath(TitleId, ContentType.AocData, StorageId);

                UInt128 NcaId = Context.Device.System.ContentManager.GetInstalledNcaId(TitleId, ContentType.AocData);

                if (string.IsNullOrWhiteSpace(InstallPath))
                {
                    InstallPath =
                        Context.Device.System.ContentManager.GetInstalledPath(TitleId, ContentType.Data, StorageId);
                }

                if ((NcaId.High | NcaId.Low) == 0)
                {
                    NcaId = Context.Device.System.ContentManager.GetInstalledNcaId(TitleId, ContentType.Data);
                }

                if (!string.IsNullOrWhiteSpace(InstallPath))
                {
                    string NcaPath = InstallPath;

                    if (File.Exists(NcaPath))
                    {
                        FileStream NcaStream = new FileStream(NcaPath, FileMode.Open, FileAccess.Read);

                        Nca Nca = new Nca(Context.Device.System.KeySet, NcaStream, false);

                        NcaSection RomfsSection = Nca.Sections.FirstOrDefault(x => x?.Type == SectionType.Romfs);

                        Stream RomfsStream = Nca.OpenSection(RomfsSection.SectionNum, false, Context.Device.System.FsIntegrityCheckLevel);

                        MakeObject(Context, new IStorage(RomfsStream));

                        return 0;
                    }
                    else
                        throw new FileNotFoundException($"No Nca found in Path `{NcaPath}`.");
                }
                else
                    throw new DirectoryNotFoundException($"Path for title id {TitleId:x16} on Storage {StorageId} was not found in Path {InstallPath}.");
            }

            throw new FileNotFoundException($"System archive with titleid {TitleId:x16} was not found on Storage {StorageId}. Found in {InstalledStorage}.");
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

            UInt128 UserId = new UInt128(
                Context.RequestData.ReadInt64(), 
                Context.RequestData.ReadInt64());

            long SaveId = Context.RequestData.ReadInt64();

            SaveDataType SaveDataType = (SaveDataType)Context.RequestData.ReadByte();

            SaveInfo SaveInfo = new SaveInfo(TitleId, SaveId, SaveDataType, UserId, SaveSpaceId);

            string SavePath = Context.Device.FileSystem.GetGameSavePath(SaveInfo, Context);

            FileSystemProvider FileSystemProvider = new FileSystemProvider(SavePath, Context.Device.FileSystem.GetBasePath());

            MakeObject(Context, new IFileSystem(SavePath, FileSystemProvider));
        }
    }
}