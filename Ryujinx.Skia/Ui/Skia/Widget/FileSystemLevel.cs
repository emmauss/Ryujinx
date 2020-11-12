using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Ryujinx.Skia.Ui.Skia.Widget
{
    public class FileSystemLevel: IDisposable
    {
        public string Path { get; private set; }
        public List<FileInfo> Files { get; set; }
        public List<DirectoryInfo> Directories { get; set; }

        public string Active { get; set; }

        public FileSystemLevel(string path)
        {
            Path = path;

            Files = new List<FileInfo>();
            Directories = new List<DirectoryInfo>();

            Active = string.Empty;
        }

        public void RefreshFiles()
        {
            if(Directory.Exists(Path))
            {
                Files.Clear();

                Files.AddRange(Directory.EnumerateFiles(Path).Select(x => new FileInfo(x)));

                Directories.AddRange(Directory.EnumerateDirectories(Path).Select(x => new DirectoryInfo(x)));

                Active = null;
            }
        }

        public void Dispose()
        {
            Files.Clear();
        }
    }
}