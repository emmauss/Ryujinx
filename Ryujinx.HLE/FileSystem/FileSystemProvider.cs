using Ryujinx.HLE.HOS;
using Ryujinx.HLE.HOS.Services.FspSrv;
using System;
using System.IO;

using static Ryujinx.HLE.HOS.ErrorCode;

namespace Ryujinx.HLE.FileSystem
{
    class FileSystemProvider : IFileSystemProvider
    {
        private string BasePath;
        private string RootPath;

        public FileSystemProvider(string BasePath, string RootPath)
        {
            this.BasePath = BasePath;
            this.RootPath = RootPath;

            CheckIfDecendentOfRootPath(BasePath);
        }

        public long CreateDirectory(string Name)
        {
            if (Directory.Exists(Name))
            {
                return MakeError(ErrorModule.Fs, FsErr.PathAlreadyExists);
            }

            Directory.CreateDirectory(Name);

            return 0;
        }

        public long CreateFile(string Name, long Size)
        {
            if (File.Exists(Name))
            {
                return MakeError(ErrorModule.Fs, FsErr.PathAlreadyExists);
            }

            using (FileStream NewFile = File.Create(Name))
            {
                NewFile.SetLength(Size);
            }

            return 0;
        }

        public long DeleteDirectory(string Name, bool Recursive)
        {
            string DirName = Name;

            if (!Directory.Exists(DirName))
            {
                return MakeError(ErrorModule.Fs, FsErr.PathDoesNotExist);
            }

            Directory.Delete(DirName, Recursive);

            return 0;
        }

        public long DeleteFile(string Name)
        {
            if (!File.Exists(Name))
            {
                return MakeError(ErrorModule.Fs, FsErr.PathDoesNotExist);
            }

            else
            {
                File.Delete(Name);
            }

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

            string FullPath = Path.Combine(BasePath, Name);

            CheckIfDecendentOfRootPath(FullPath);

            return FullPath;
        }

        public long GetTotalSpace(ServiceCtx Context)
        {
            return Context.Device.FileSystem.GetDrive().TotalSize;
        }

        public bool DirectoryExists(string Name)
        {
            return Directory.Exists(Name);
        }

        public bool FileExists(string Name)
        {
            return File.Exists(Name);
        }

        public long OpenDirectory(string Name, int FilterFlags,out IDirectory DirectoryInterface)
        {
            DirectoryInterface = null;

            if (Directory.Exists(Name))
            {
                DirectoryInterface = new IDirectory(Name, FilterFlags, this);

                return 0;
            }

            return MakeError(ErrorModule.Fs, FsErr.PathDoesNotExist);
        }

        public long OpenFile(string Name, out IFile FileInterface)
        {
            FileInterface = null;

            if (File.Exists(Name))
            {
                FileStream Stream = new FileStream(Name, FileMode.Open);

                FileInterface = new IFile(Stream, Name);

                return 0;
            }

            return MakeError(ErrorModule.Fs, FsErr.PathDoesNotExist);
        }

        public long RenameDirectory(string OldName, string NewName)
        {
            if (Directory.Exists(OldName))
            {
                Directory.Move(OldName, NewName);
            }
            else
            {
                return MakeError(ErrorModule.Fs, FsErr.PathDoesNotExist);
            }

            return 0;
        }

        public long RenameFile(string OldName, string NewName)
        {
            if (File.Exists(OldName))
            {
                File.Move(OldName, NewName);
            }
            else
            {
                return MakeError(ErrorModule.Fs, FsErr.PathDoesNotExist);
            }

            return 0;
        }

        public void CheckIfDecendentOfRootPath(string Path)
        {
            DirectoryInfo PathInfo = new DirectoryInfo(Path);
            DirectoryInfo RootInfo = new DirectoryInfo(RootPath);

            while (PathInfo.Parent != null)
            {
                if (PathInfo.Parent.FullName == RootInfo.FullName)
                {
                    return;
                }
                else
                {
                    PathInfo = PathInfo.Parent;
                }
            }

            throw new InvalidOperationException($"Path {BasePath} is not a child directory of {RootPath}");
        }
    }
}
