using LibHac;
using Ryujinx.HLE.HOS;
using Ryujinx.HLE.HOS.Services.FspSrv;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using static Ryujinx.HLE.HOS.ErrorCode;

namespace Ryujinx.HLE.FileSystem
{
    class RomFileSystemProvider : IFileSystemProvider
    {
        private Romfs RomFs;

        public RomFileSystemProvider(Stream StorageStream)
        {
            RomFs = new Romfs(StorageStream);
        }

        public long CreateDirectory(string Name)
        {
            throw new NotSupportedException();
        }

        public long CreateFile(string Name, long Size)
        {
            throw new NotSupportedException();
        }

        public long DeleteDirectory(string Name, bool Recursive)
        {
            throw new NotSupportedException();
        }

        public long DeleteFile(string Name)
        {
            throw new NotSupportedException();
        }

        public string[] GetDirectories(string Path)
        {
            List<string> Directories = new List<string>();

            foreach(RomfsDir Directory in RomFs.Directories)
            {
                Directories.Add(Directory.Name);
            }

            return Directories.ToArray();
        }

        public string[] GetEntries(string Path)
        {
            List<string> Entries = new List<string>();

            foreach (RomfsDir Directory in RomFs.Directories)
            {
                Entries.Add(Directory.Name);
            }

            foreach (RomfsFile File in RomFs.Files)
            {
                Entries.Add(File.Name);
            }

            return Entries.ToArray();
        }

        public string[] GetFiles(string Path)
        {
            List<string> Files = new List<string>();

            foreach (RomfsFile File in RomFs.Files)
            {
                Files.Add(File.Name);
            }

            return Files.ToArray();
        }

        public long GetFreeSpace(ServiceCtx Context)
        {
            return 0;
        }

        public string GetFullPath(string Name)
        {
            return Name;
        }

        public long GetTotalSpace(ServiceCtx Context)
        {
            return RomFs.Files.Sum(x => x.DataLength);
        }

        public bool DirectoryExists(string Name)
        {
            return RomFs.Directories.Exists(x=>x.Name == Name);
        }

        public bool FileExists(string Name)
        {
            return RomFs.FileExists(Name);
        }

        public long OpenDirectory(string Name, int FilterFlags, out IDirectory DirectoryInterface)
        {
            DirectoryInterface = null;

            RomfsDir Directory = RomFs.Directories.Find(x => x.Name == Name);

            if (Directory != null)
            {
                DirectoryInterface = new IDirectory(Name, FilterFlags, this);
            }

            return MakeError(ErrorModule.Fs, FsErr.PathDoesNotExist);
        }

        public long OpenFile(string Name, out IFile FileInterface)
        {
            FileInterface = null;

            if (File.Exists(Name))
            {
                Stream Stream = RomFs.OpenFile(Name);

                FileInterface = new IFile(Stream, Name);

                return 0;
            }

            return MakeError(ErrorModule.Fs, FsErr.PathDoesNotExist);
        }

        public long RenameDirectory(string OldName, string NewName)
        {
            throw new NotSupportedException();
        }

        public long RenameFile(string OldName, string NewName)
        {
            throw new NotSupportedException();
        }

        public void CheckIfOutsideBasePath(string Path)
        {
            throw new NotSupportedException();
        }
    }
}
