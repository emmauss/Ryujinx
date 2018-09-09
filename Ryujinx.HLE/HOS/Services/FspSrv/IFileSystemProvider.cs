using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Ryujinx.HLE.HOS;

namespace Ryujinx.HLE.HOS.Services.FspSrv
{
    interface IFileSystemProvider
    {
        int CreateFile(string Name, long Size);

        int CreateDirectory(string Name);

        int RenameFile(string OldName, string NewName);

        int RenameDirectory(string OldName, string NewName);

        string[] GetEntries(string Path);

        string[] GetDirectories(string Path);

        string[] GetFiles(string Path);

        int DeleteFile(string Name);

        int DeleteDirectory(string Name, bool Recursive);

        bool IsFileExists(string Name);

        bool IsDirectoryExists(string Name);

        int OpenFile(string Name, out IFile FileInterface);

        int OpenDirectory(string Name, int FilterFlags, out IDirectory DirectoryInterface);

        string GetFullPath(string Name);

        long GetFreeSpace(ServiceCtx Context);

        long GetTotalSpace(ServiceCtx Context);
        
    }
}
