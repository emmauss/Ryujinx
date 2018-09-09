using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Ryujinx.HLE.HOS;

using static Ryujinx.HLE.HOS.ErrorCode;

namespace Ryujinx.HLE.HOS.Services.FspSrv
{
    class FileSystemProvider : IFileSystemProvider
    {
        private string BasePath;

        public FileSystemProvider(string Path)
        {
            this.BasePath = Path;
        }

        public int CreateDirectory(string Name)
        {
            string FullName = Name;

            if (Directory.Exists(FullName))
                return (int)MakeError(ErrorModule.Fs, FsErr.PathAlreadyExists);

            Directory.CreateDirectory(FullName);

            return 0;
        }

        public int CreateFile(string Name, long Size)
        {
            string FullName = Name;

            if (File.Exists(FullName))
                return (int)MakeError(ErrorModule.Fs, FsErr.PathAlreadyExists);

            using (FileStream NewFile = File.Create(FullName))
            {
                NewFile.SetLength(Size);
            }

            return 0;
        }

        public int DeleteDirectory(string Name, bool Recursive)
        {
            string DirName = Name;

            if (!Directory.Exists(DirName))
            {
                return (int)MakeError(ErrorModule.Fs, FsErr.PathDoesNotExist);
            }

            Directory.Delete(DirName, Recursive);

            return 0;
        }

        public int DeleteFile(string Name)
        {
            if (!File.Exists(Name))
            {
                return (int)MakeError(ErrorModule.Fs, FsErr.PathDoesNotExist);
            }

            else File.Delete(Name);

            return 0;
        }

        public string[] GetDirectories(string Path)
        {
            return Directory.GetDirectories(Path);
        }

        public string[] GetEntries(string Path)
        {
            string DirName = GetFullPath(Path);

            if (Directory.Exists(DirName))
            {
                return Directory.GetFileSystemEntries(DirName);
            }

            return null;
        }

        public string[] GetFiles(string Path)
        {
            return Directory.GetFiles(Path);
        }

        public long GetFreeSpace(ServiceCtx Context)
        {
            return Context.Device.FileSystem.GetDrive().AvailableFreeSpace;
        }

        public string GetFullPath(string Name)
        {
            if (Name.StartsWith("//"))
            {
                Name = Name.Substring(2);
            }
            else if (Name.StartsWith('/'))
            {
                Name = Name.Substring(1);
            }
            else
            {
                return null;
            }

            return Path.Combine(BasePath, Name);
        }

        public long GetTotalSpace(ServiceCtx Context)
        {
            return Context.Device.FileSystem.GetDrive().TotalSize;
        }

        public bool IsDirectoryExists(string Name)
        {
            return Directory.Exists(Name);
        }

        public bool IsFileExists(string Name)
        {
            return File.Exists(Name);
        }

        public int OpenDirectory(string Name, int FilterFlags,out IDirectory DirectoryInterface)
        {
            DirectoryInterface = null;

            if (Directory.Exists(Name))
            {
                DirectoryInterface = new IDirectory(Name, FilterFlags, this);
            }
            else
            {
                return (int)MakeError(ErrorModule.Fs, FsErr.PathDoesNotExist);
            }

            return 0;
        }

        public int OpenFile(string Name, out IFile FileInterface)
        {
            FileInterface = null;

            if (File.Exists(Name))
            {
                FileStream Stream = new FileStream(Name, FileMode.Open);

                FileInterface = new IFile(Stream, Name);
            }
            else
            {
                return (int)MakeError(ErrorModule.Fs, FsErr.PathDoesNotExist);
            }

            return 0;
        }

        public int RenameDirectory(string OldName, string NewName)
        {
            if (Directory.Exists(OldName))
            {
                Directory.Move(OldName, NewName);
            }
            else
            {
                return (int)MakeError(ErrorModule.Fs, FsErr.PathDoesNotExist);
            }

            return 0;
        }

        public int RenameFile(string OldName, string NewName)
        {
            if (File.Exists(OldName))
            {
                File.Move(OldName, NewName);
            }
            else
            {
                return (int)MakeError(ErrorModule.Fs, FsErr.PathDoesNotExist);
            }

            return 0;
        }
    }
}
