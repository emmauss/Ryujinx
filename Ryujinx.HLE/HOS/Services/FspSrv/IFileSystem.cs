using Ryujinx.HLE.HOS.Ipc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using static Ryujinx.HLE.HOS.ErrorCode;

namespace Ryujinx.HLE.HOS.Services.FspSrv
{
    class IFileSystem : IpcService
    {
        private Dictionary<int, ServiceProcessRequest> m_Commands;

        public override IReadOnlyDictionary<int, ServiceProcessRequest> Commands => m_Commands;

        private HashSet<string> OpenPaths;

        private string Path;

        private IFileSystemProvider Provider;

        public IFileSystem(string Path, IFileSystemProvider Provider)
        {
            m_Commands = new Dictionary<int, ServiceProcessRequest>()
            {
                { 0,  CreateFile                 },
                { 1,  DeleteFile                 },
                { 2,  CreateDirectory            },
                { 3,  DeleteDirectory            },
                { 4,  DeleteDirectoryRecursively },
                { 5,  RenameFile                 },
                { 6,  RenameDirectory            },
                { 7,  GetEntryType               },
                { 8,  OpenFile                   },
                { 9,  OpenDirectory              },
                { 10, Commit                     },
                { 11, GetFreeSpaceSize           },
                { 12, GetTotalSpaceSize          },
                { 13, CleanDirectoryRecursively  },
                //{ 14, GetFileTimeStampRaw        }
            };

            OpenPaths = new HashSet<string>();

            this.Path = Path;

            this.Provider = Provider;
        }

        public long CreateFile(ServiceCtx Context)
        {
            string Name = ReadUtf8String(Context);

            long Mode = Context.RequestData.ReadInt64();
            int  Size = Context.RequestData.ReadInt32();

            string FileName = Provider.GetFullPath(Name);

            if (FileName == null)
            {
                return MakeError(ErrorModule.Fs, FsErr.PathDoesNotExist);
            }

            if (Provider.IsFileExists(Name))
            {
                return MakeError(ErrorModule.Fs, FsErr.PathAlreadyExists);
            }

            if (IsPathAlreadyInUse(FileName))
            {
                return MakeError(ErrorModule.Fs, FsErr.PathAlreadyInUse);
            }

            return Provider.CreateFile(FileName, Size);
        }

        public long DeleteFile(ServiceCtx Context)
        {
            string Name = ReadUtf8String(Context);

            string FileName = Provider.GetFullPath(Name);

            if (!Provider.IsFileExists(FileName))
            {
                return MakeError(ErrorModule.Fs, FsErr.PathDoesNotExist);
            }

            if (IsPathAlreadyInUse(FileName))
            {
                return MakeError(ErrorModule.Fs, FsErr.PathAlreadyInUse);
            }

            return Provider.DeleteFile(FileName);
        }

        public long CreateDirectory(ServiceCtx Context)
        {
            string Name = ReadUtf8String(Context);

            string DirName = Provider.GetFullPath(Name);

            if (DirName == null)
            {
                return MakeError(ErrorModule.Fs, FsErr.PathDoesNotExist);
            }

            if (Provider.IsDirectoryExists(DirName))
            {
                return MakeError(ErrorModule.Fs, FsErr.PathAlreadyExists);
            }

            if (IsPathAlreadyInUse(DirName))
            {
                return MakeError(ErrorModule.Fs, FsErr.PathAlreadyInUse);
            }

            Provider.CreateDirectory(DirName);

            return 0;
        }

        public long DeleteDirectory(ServiceCtx Context)
        {
            return DeleteDirectory(Context, false);
        }

        public long DeleteDirectoryRecursively(ServiceCtx Context)
        {
            return DeleteDirectory(Context, true);
        }

        private long DeleteDirectory(ServiceCtx Context, bool Recursive)
        {
            string Name = ReadUtf8String(Context);

            string DirName = Provider.GetFullPath(Name);

            if (!Directory.Exists(DirName))
            {
                return MakeError(ErrorModule.Fs, FsErr.PathDoesNotExist);
            }

            if (IsPathAlreadyInUse(DirName))
            {
                return MakeError(ErrorModule.Fs, FsErr.PathAlreadyInUse);
            }

            Provider.DeleteDirectory(DirName, Recursive);

            return 0;
        }

        public long RenameFile(ServiceCtx Context)
        {
            string OldName = ReadUtf8String(Context, 0);
            string NewName = ReadUtf8String(Context, 1);

            string OldFileName = Provider.GetFullPath(OldName);
            string NewFileName = Provider.GetFullPath(NewName);

            if (Provider.IsFileExists(OldFileName))
            {
                return MakeError(ErrorModule.Fs, FsErr.PathDoesNotExist);
            }

            if (Provider.IsFileExists(NewFileName))
            {
                return MakeError(ErrorModule.Fs, FsErr.PathAlreadyExists);
            }

            if (IsPathAlreadyInUse(OldFileName))
            {
                return MakeError(ErrorModule.Fs, FsErr.PathAlreadyInUse);
            }

            return Provider.RenameFile(OldFileName, NewFileName);
        }

        public long RenameDirectory(ServiceCtx Context)
        {
            string OldName = ReadUtf8String(Context, 0);
            string NewName = ReadUtf8String(Context, 1);

            string OldDirName = Provider.GetFullPath(OldName);
            string NewDirName = Provider.GetFullPath(NewName);

            if (!Provider.IsDirectoryExists(OldDirName))
            {
                return MakeError(ErrorModule.Fs, FsErr.PathDoesNotExist);
            }

            if (!Provider.IsDirectoryExists(NewDirName))
            {
                return MakeError(ErrorModule.Fs, FsErr.PathAlreadyExists);
            }

            if (IsPathAlreadyInUse(OldDirName))
            {
                return MakeError(ErrorModule.Fs, FsErr.PathAlreadyInUse);
            }

            return Provider.RenameDirectory(OldDirName, NewDirName);
        }

        public long GetEntryType(ServiceCtx Context)
        {
            string Name = ReadUtf8String(Context);

            string FileName = Provider.GetFullPath(Name);

            if (Provider.IsFileExists(FileName))
            {
                Context.ResponseData.Write(1);
            }
            else if (Provider.IsDirectoryExists(FileName))
            {
                Context.ResponseData.Write(0);
            }
            else
            {
                Context.ResponseData.Write(0);

                return MakeError(ErrorModule.Fs, FsErr.PathDoesNotExist);
            }

            return 0;
        }

        public long OpenFile(ServiceCtx Context)
        {
            int FilterFlags = Context.RequestData.ReadInt32();

            string Name = ReadUtf8String(Context);

            string FileName = Provider.GetFullPath(Name);

            if (!Provider.IsFileExists(FileName))
            {
                return MakeError(ErrorModule.Fs, FsErr.PathDoesNotExist);
            }

            if (IsPathAlreadyInUse(FileName))
            {
                return MakeError(ErrorModule.Fs, FsErr.PathAlreadyInUse);
            }


            int Error = Provider.OpenFile(FileName, out IFile FileInterface);

            if (Error == 0)
            {
                FileInterface.Disposed += RemoveFileInUse;

                lock (OpenPaths)
                {
                    OpenPaths.Add(FileName);
                }

                MakeObject(Context, FileInterface);

                return 0;
            }

            return Error;
        }

        public long OpenDirectory(ServiceCtx Context)
        {
            int FilterFlags = Context.RequestData.ReadInt32();

            string Name = ReadUtf8String(Context);

            string DirName = Provider.GetFullPath(Name);

            if (!Provider.IsDirectoryExists(DirName))
            {
                return MakeError(ErrorModule.Fs, FsErr.PathDoesNotExist);
            }

            if (IsPathAlreadyInUse(DirName))
            {
                return MakeError(ErrorModule.Fs, FsErr.PathAlreadyInUse);
            }

            int Error = Provider.OpenDirectory(DirName, FilterFlags, out IDirectory DirInterface);

            if (Error == 0)
            {
                DirInterface.Disposed += RemoveDirectoryInUse;

                lock (OpenPaths)
                {
                    OpenPaths.Add(DirName);
                }

                MakeObject(Context, DirInterface);
            }

            return Error;
        }

        public long Commit(ServiceCtx Context)
        {
            return 0;
        }

        public long GetFreeSpaceSize(ServiceCtx Context)
        {
            string Name = ReadUtf8String(Context);

            Context.ResponseData.Write(Provider.GetFreeSpace(Context));

            return 0;
        }

        public long GetTotalSpaceSize(ServiceCtx Context)
        {
            string Name = ReadUtf8String(Context);

            Context.ResponseData.Write(Provider.GetFreeSpace(Context));

            return 0;
        }

        public long CleanDirectoryRecursively(ServiceCtx Context)
        {
            string Name = ReadUtf8String(Context);

            string DirName = Provider.GetFullPath(Name);

            if (!Provider.IsDirectoryExists(DirName))
            {
                return MakeError(ErrorModule.Fs, FsErr.PathDoesNotExist);
            }

            if (IsPathAlreadyInUse(DirName))
            {
                return MakeError(ErrorModule.Fs, FsErr.PathAlreadyInUse);
            }

            foreach (string Entry in Provider.GetEntries(DirName))
            {
                if (Provider.IsDirectoryExists(Entry))
                {
                    Provider.DeleteDirectory(Entry, true);
                }
                else if (Provider.IsFileExists(Entry))
                {
                   Provider.DeleteFile(Entry);
                }
            }

            return 0;
        }

        private bool IsPathAlreadyInUse(string Path)
        {
            lock (OpenPaths)
            {
                return OpenPaths.Contains(Path);
            }
        }

        private void RemoveFileInUse(object sender, EventArgs e)
        {
            IFile FileInterface = (IFile)sender;

            lock (OpenPaths)
            {
                FileInterface.Disposed -= RemoveFileInUse;

                OpenPaths.Remove(FileInterface.HostPath);
            }
        }

        private void RemoveDirectoryInUse(object sender, EventArgs e)
        {
            IDirectory DirInterface = (IDirectory)sender;

            lock (OpenPaths)
            {
                DirInterface.Disposed -= RemoveDirectoryInUse;

                OpenPaths.Remove(DirInterface.DirectoryPath);
            }
        }

        private string ReadUtf8String(ServiceCtx Context, int Index = 0)
        {
            long Position = Context.Request.PtrBuff[Index].Position;
            long Size     = Context.Request.PtrBuff[Index].Size;

            using (MemoryStream MS = new MemoryStream())
            {
                while (Size-- > 0)
                {
                    byte Value = Context.Memory.ReadByte(Position++);

                    if (Value == 0)
                    {
                        break;
                    }

                    MS.WriteByte(Value);
                }

                return Encoding.UTF8.GetString(MS.ToArray());
            }
        }
    }
}