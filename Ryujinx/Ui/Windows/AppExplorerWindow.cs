using Gtk;
using LibHac;
using LibHac.Common;
using LibHac.Fs;
using LibHac.Fs.Fsa;
using LibHac.FsSystem;
using LibHac.FsSystem.NcaUtils;
using Ryujinx.HLE.FileSystem;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GUI = Gtk.Builder.ObjectAttribute;

namespace Ryujinx.Ui.Windows
{
    public class AppExplorerWindow : Window, IProgressReport
    {
        private Xci _xci;
        private PartitionFileSystem _fs;
        private string _appType = "";
        private string _titleName;
        private string _titleId;

        private long _operationTotal;
        private long _operationCompleted;
        private bool _operationRunning;

#pragma warning disable CS0649, IDE0044
        [GUI] TreeView _explorerView;
        [GUI] Label _nameLabel;
        [GUI] Label _idLabel;
        [GUI] Label _sizeLabel;
        [GUI] Label _typeLabel;
        [GUI] Label _progressLabel;
        [GUI] Entry _searchEntry;
        [GUI] TreeSelection _explorerSelection;
        [GUI] Gtk.ProgressBar _progressBar;
        [GUI] TreeModelFilter _filterModel;
#pragma warning restore CS0649, IDE0044

        public string AppPath { get; }

        public VirtualFileSystem VirtualFileSystem { get; }

        public AppExplorerWindow(VirtualFileSystem virtualFileSystem, string titlePath, string titleName, string titleId) : this(new Builder("Ryujinx.Ui.Windows.AppExplorerWindow.glade"), virtualFileSystem, titlePath, titleName, titleId) { }

        private AppExplorerWindow(Builder builder, VirtualFileSystem virtualFileSystem, string appPath, string titleName, string titleId) : base(builder.GetObject("_explorerWindow").Handle)
        {
            VirtualFileSystem = virtualFileSystem;
            AppPath = appPath;
            builder.Autoconnect(this);

            _titleName = titleName;
            _titleId = titleId;

            var model = _explorerView.Model;

            _filterModel = new TreeModelFilter(model, null);

            _filterModel.VisibleFunc = new TreeModelFilterVisibleFunc(Filter);

            _explorerView.Model = _filterModel;

            _explorerView.AppendColumn("Name", new CellRendererText(), "text", 0);
            _explorerView.AppendColumn("Size", new CellRendererText(), "text", 1);
            _explorerView.AppendColumn("Path", new CellRendererText(), "text", 2);
            _explorerView.AppendColumn("Type", new CellRendererText(), "text", 3);
            _explorerView.AppendColumn("Title Id", new CellRendererText(), "text", 4);

            _explorerView.Columns[2].Visible = false;
            _explorerView.Columns[0].MinWidth = 250;

            _searchEntry.Changed += SearchEntry_Changed;

            Title = $"Ryujinx - Exploring {titleName} Contents";

            _nameLabel.Text = titleName;
            _idLabel.Text = titleId;

            _explorerView.RowExpanded += ExplorerView_RowExpanded;
            _explorerView.ButtonReleaseEvent += ExplorerView_RowClicked;

            LoadApp();
        }

        private void SearchEntry_Changed(object sender, EventArgs e)
        {
            _filterModel.Refilter();
        }

        private bool Filter(ITreeModel model, TreeIter iter)
        {
            string query = _searchEntry.Text.ToLower();

            if(string.IsNullOrWhiteSpace(query))
            {
                return true;
            }

            string name = model.GetValue(iter, 0).ToString().ToLower();
            string type = model.GetValue(iter, 3).ToString();
            string titleId = model.GetValue(iter, 4).ToString().ToLower();

            if(name == "[expand]" || type == "Section" || type == "Partition")
            {
                return true;
            }

            if(name.Contains(query) || titleId.Contains(query))
            {
                return true;
            }

            if (model.IterHasChild(iter))
            {
                return SearchChildNodes(iter);
            }

            return false;

            bool SearchChildNodes(TreeIter parentIter)
            {
                TreeIter childIter;
                model.IterChildren(out childIter, parentIter);

                string name = model.GetValue(parentIter, 0).ToString().ToLower();
                string type = model.GetValue(parentIter, 3).ToString();
                string titleId = model.GetValue(parentIter, 4).ToString().ToLower();

                if (name == "[expand]" || type == "Section" || type == "Partition")
                {
                    return true;
                }

                if (name == "[expand]")
                {
                    return true;
                }

                do
                {
                    if (name.Contains(query) || titleId.Contains(query))
                        return true;

                    if (model.IterHasChild(childIter))
                        return SearchChildNodes(childIter);

                } while (model.IterNext(ref childIter));

                return false;
            }
        }

        private void ExplorerView_RowClicked(object o, ButtonReleaseEventArgs args)
        {
            if (args.Event.Button != 3)
            {
                return;
            }

            _explorerSelection.GetSelected(out TreeIter treeIter);

            if (treeIter.UserData == IntPtr.Zero)
            {
                return;
            }

            string type = _explorerView.Model.GetValue(treeIter, 3).ToString();

            Menu menu = new Menu();

            if(type == "Section" || type == "Partition" || type == "DIR")
            {
                MenuItem extractToMenuItem = new MenuItem("Extract To ...");

                menu.Add(extractToMenuItem);

                extractToMenuItem.Activated += ExtractToMenuItem_Activated;
            }

            if (type != "DIR" && type != "Partition")
            {
                MenuItem saveToMenuItem = new MenuItem("Save To ...");

                menu.Add(saveToMenuItem);

                saveToMenuItem.Activated += SaveToMenuItem_Activated;
            }

            if (menu.Children.Length > 0)
            {
                menu.ShowAll();

                menu.PopupAtPointer(null);
            }
        }

        private string GetSaveToPath(string title)
        {
            FileChooserDialog dialog = new FileChooserDialog(title, this, FileChooserAction.SelectFolder, "Cancel", ResponseType.Cancel, "Choose", ResponseType.Accept)
            {
                SelectMultiple = false,
            };

            try
            {
                if (dialog.Run() == (int)ResponseType.Accept)
                {
                    return dialog.Filename;
                }
            }
            finally
            {
                dialog.Dispose();
            }

            return string.Empty;
        }

        private void SaveToMenuItem_Activated(object sender, EventArgs e)
        {
            if(_operationRunning)
            {
                return;
            }

            string savePath = GetSaveToPath("Save To...");

            _operationRunning = true;

            FileStream destination;

            Task.Run(() =>
            {
                _explorerSelection.GetSelected(out TreeIter treeIter);

                string path = _explorerView.Model.GetValue(treeIter, 2).ToString();

                string fileName = string.Empty;

                FileStream destination;

                try
                {
                    if (!string.IsNullOrWhiteSpace(savePath) && Directory.Exists(savePath))
                    {
                        var levels = path.Split("/", StringSplitOptions.RemoveEmptyEntries);

                        switch (_appType)
                        {
                            case "xci":
                                var partitionType = Enum.Parse<XciPartitionType>(levels[0], true);
                                if (_xci.HasPartition(partitionType))
                                {
                                    var partition = _xci.OpenPartition(partitionType);

                                    fileName = levels[1];

                                    if (fileName.ToLower().EndsWith(".nca"))
                                    {
                                        if (partition.FileExists(fileName))
                                        {
                                            var entry = partition.Files.ToList().Find(x => x.Name == fileName);

                                            var ncaStorage = partition.OpenFile(entry, OpenMode.Read).AsStorage();

                                            if (levels.Length == 2)
                                            {
                                                using (destination = File.OpenWrite(System.IO.Path.Combine(savePath, fileName)))
                                                {
                                                    ncaStorage.GetSize(out var size);

                                                    LogMessage($"Saving {entry.Name}");

                                                    ncaStorage.CopyToStream(destination, size, this);
                                                }

                                                return;
                                            }

                                            var nca = new Nca(VirtualFileSystem.KeySet, ncaStorage);

                                            var ncaSectionType = Enum.Parse<NcaSectionType>(levels[2], true);

                                            if (levels.Length == 3)
                                            {
                                                if (nca.CanOpenSection(ncaSectionType))
                                                {
                                                    var section = nca.OpenStorage(ncaSectionType, IntegrityCheckLevel.IgnoreOnInvalid);

                                                    savePath = System.IO.Path.Combine(savePath, fileName, ncaSectionType.ToString());

                                                    Directory.CreateDirectory(new FileInfo(savePath).DirectoryName);

                                                    using (destination = File.OpenWrite(savePath))
                                                    {
                                                        section.GetSize(out var size);

                                                        LogMessage($"Saving {ncaSectionType} section");

                                                        section.CopyToStream(destination, size, this);
                                                    }
                                                }
                                                return;
                                            }
                                            string relativePath = string.Join('/', levels.AsSpan().Slice(3).ToArray());

                                            TransverseNca(nca, ncaSectionType, "/" + relativePath);
                                        }
                                    }
                                    else
                                    {
                                        savePath = System.IO.Path.Combine(savePath, fileName);

                                        Directory.CreateDirectory(new FileInfo(savePath).DirectoryName);

                                        using (destination = File.OpenWrite(savePath))
                                        {
                                            if (partition.OpenFile(out var file, fileName.ToU8Span(), OpenMode.Read) == Result.Success)
                                            {
                                                file.GetSize(out var size);

                                                LogMessage($"Saving {fileName}");

                                                file.AsStorage().CopyToStream(destination, size, this);
                                            }
                                        }
                                    }
                                }
                                break;
                            case "nsp":
                                fileName = levels[0];
                                if (fileName.ToLower().EndsWith(".nca"))
                                {
                                    if (_fs.FileExists(fileName))
                                    {
                                        var entry = _fs.Files.ToList().Find(x => x.Name == fileName);

                                        var ncaStorage = _fs.OpenFile(entry, OpenMode.Read).AsStorage();

                                        if (levels.Length == 1)
                                        {
                                            using (destination = File.OpenWrite(System.IO.Path.Combine(savePath, fileName)))
                                            {
                                                ncaStorage.GetSize(out var size);

                                                LogMessage($"Saving {fileName}");

                                                ncaStorage.CopyToStream(destination, size, this);
                                            }

                                            return;
                                        }

                                        var nca = new Nca(VirtualFileSystem.KeySet, ncaStorage);

                                        if (levels.Last() != fileName)
                                        {
                                            var ncaSection = Enum.Parse<NcaSectionType>(levels[1], true);

                                            string relativePath = string.Join('/', levels.AsSpan().Slice(2).ToArray());

                                            TransverseNca(nca, ncaSection, "/" + relativePath);
                                        }
                                    }
                                }
                                else
                                {
                                    savePath = System.IO.Path.Combine(savePath, fileName);

                                    Directory.CreateDirectory(new FileInfo(savePath).DirectoryName);

                                    using (destination = File.OpenWrite(savePath))
                                    {
                                        if (_fs.OpenFile(out var file, fileName.ToU8Span(), OpenMode.Read) == Result.Success)
                                        {
                                            file.GetSize(out var size);

                                            LogMessage($"Saving {fileName}");

                                            file.AsStorage().CopyToStream(destination, size, this);
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }
                finally
                {
                    _operationRunning = false;

                    LogMessage("Save Successful");
                }
            });

            void TransverseNca(Nca nca, NcaSectionType sectionType, string relativePath)
            {
                if (nca.CanOpenSection(sectionType))
                {
                    var section = nca.OpenFileSystem(sectionType, IntegrityCheckLevel.IgnoreOnInvalid);

                    if (section != null)
                    {
                        if (section.FileExists("/" + relativePath))
                        {
                            if(section.OpenFile(out var file, relativePath.ToU8Span(), OpenMode.Read) == Result.Success)
                            {
                                savePath = System.IO.Path.Combine(savePath, new FileInfo(relativePath).Name);

                                Directory.CreateDirectory(new FileInfo(savePath).DirectoryName);

                                using (destination = File.OpenWrite(savePath))
                                {
                                    file.GetSize(out var size);

                                    LogMessage($"Saving {relativePath}");

                                    file.AsStorage().CopyToStream(destination, size, this);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ExtractToMenuItem_Activated(object sender, EventArgs e)
        {
            if(_operationRunning)
            {
                return;
            }
            _explorerSelection.GetSelected(out TreeIter treeIter);

            string path = _explorerView.Model.GetValue(treeIter, 2).ToString();

            string savePath = GetSaveToPath("Extract To...");

            string fileName = string.Empty;

            _operationRunning = true;

            Task.Run(() =>
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(savePath) && Directory.Exists(savePath))
                    {
                        var levels = path.Split("/", StringSplitOptions.RemoveEmptyEntries);

                        switch (_appType)
                        {
                            case "xci":
                                var partitionType = Enum.Parse<XciPartitionType>(levels[0], true);
                                if (_xci.HasPartition(partitionType))
                                {
                                    var partition = _xci.OpenPartition(partitionType);

                                    if (levels.Length == 1)
                                    {
                                        savePath = System.IO.Path.Combine(savePath, partitionType.ToString());

                                        Directory.CreateDirectory(savePath);

                                        LogMessage($"Extracting {partitionType}");

                                        partition.Extract(savePath, this);

                                        return;
                                    }

                                    fileName = levels[1];

                                    if (fileName.ToLower().EndsWith(".nca"))
                                    {
                                        if (partition.FileExists(fileName))
                                        {
                                            var entry = partition.Files.ToList().Find(x => x.Name == fileName);

                                            var ncaStorage = partition.OpenFile(entry, OpenMode.Read).AsStorage();

                                            var nca = new Nca(VirtualFileSystem.KeySet, ncaStorage);

                                            var ncaSectionType = Enum.Parse<NcaSectionType>(levels[2], true);

                                            if (levels.Length == 3)
                                            {
                                                if (nca.CanOpenSection(ncaSectionType))
                                                {
                                                    var section = nca.OpenFileSystem(ncaSectionType, IntegrityCheckLevel.IgnoreOnInvalid);

                                                    savePath = System.IO.Path.Combine(savePath, ncaSectionType.ToString());

                                                    Directory.CreateDirectory(savePath);

                                                    LogMessage($"Extracting {ncaSectionType}");

                                                    section.Extract(savePath, this);
                                                }
                                                return;
                                            }
                                            string relativePath = string.Join('/', levels.AsSpan().Slice(3).ToArray());

                                            TransverseNca(nca, ncaSectionType, "/" + relativePath);
                                        }
                                    }
                                }
                                break;
                            case "nsp":
                                if (levels.Length == 0)
                                {
                                    LogMessage($"Extracting {_titleName}");

                                    _fs.Extract(savePath, this);

                                    return;
                                }
                                fileName = levels[0];
                                if (fileName.ToLower().EndsWith(".nca"))
                                {
                                    if (_fs.FileExists(fileName))
                                    {
                                        var entry = _fs.Files.ToList().Find(x => x.Name == fileName);

                                        var ncaStorage = _fs.OpenFile(entry, OpenMode.Read).AsStorage();

                                        var nca = new Nca(VirtualFileSystem.KeySet, ncaStorage);

                                        if (levels.Last() != fileName)
                                        {
                                            var ncaSectionType = Enum.Parse<NcaSectionType>(levels[1], true);

                                            if (levels.Length == 2)
                                            {
                                                if (nca.CanOpenSection(ncaSectionType))
                                                {
                                                    var section = nca.OpenFileSystem(ncaSectionType, IntegrityCheckLevel.IgnoreOnInvalid);

                                                    savePath = System.IO.Path.Combine(savePath, ncaSectionType.ToString());

                                                    Directory.CreateDirectory(savePath);

                                                    LogMessage($"Extracting {ncaSectionType}");

                                                    section.Extract(savePath, this);
                                                }
                                                return;
                                            }

                                            string relativePath = string.Join('/', levels.AsSpan().Slice(2).ToArray());

                                            TransverseNca(nca, ncaSectionType, "/" + relativePath);
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }
                finally
                {
                    _operationRunning = false;

                    LogMessage("Extraction Successful");
                }
            });

            void TransverseNca(Nca nca, NcaSectionType sectionType, string relativePath)
            {
                if (nca.CanOpenSection(sectionType))
                {
                    var section = nca.OpenFileSystem(sectionType, IntegrityCheckLevel.IgnoreOnInvalid);

                    if (section != null)
                    {
                        relativePath = "/" + relativePath;

                        if (section.DirectoryExists(relativePath))
                        {
                            if (section.OpenDirectory(out var directory, relativePath.ToU8Span(), OpenDirectoryMode.All) == Result.Success)
                            {
                                savePath = System.IO.Path.Combine(savePath, new FileInfo(relativePath).Name);

                                Directory.CreateDirectory(savePath);

                                directory.GetEntryCount(out long count);

                                var entries = new DirectoryEntry[count];
                                long entriesRead = 0;
                                do
                                {
                                    directory.Read(out entriesRead, entries.AsSpan());
                                }
                                while (entriesRead == 0);

                                foreach (var directoryEntry in entries)
                                {
                                    string name = System.Text.Encoding.UTF8.GetString(directoryEntry.Name).TrimEnd('\0');

                                    string fullPath = relativePath + "/" + name;

                                    string extractionPath = System.IO.Path.Combine(savePath, name);

                                    switch (directoryEntry.Type)
                                    {
                                        case DirectoryEntryType.File:
                                            section.OpenFile(out var file, fullPath.ToU8Span(), OpenMode.Read);

                                            Directory.CreateDirectory(new FileInfo(extractionPath).DirectoryName);

                                            using (var stream = File.OpenWrite(extractionPath))
                                            {
                                                file.GetSize(out var size);

                                                LogMessage($"Saving {fullPath}");

                                                file.AsStorage().CopyToStream(stream, size, this);
                                            }
                                            break;
                                        case DirectoryEntryType.Directory:
                                            ExtractDirectory(extractionPath, fullPath);
                                            break;
                                    }
                                }
                            }
                        }

                        void ExtractDirectory(string path, string fsPath)
                        {
                            if (section.DirectoryExists(fsPath))
                            {
                                if (section.OpenDirectory(out var directory, fsPath.ToU8Span(), OpenDirectoryMode.All) == Result.Success)
                                {
                                    savePath = System.IO.Path.Combine(savePath, new FileInfo(fsPath).Name);

                                    Directory.CreateDirectory(savePath);

                                    directory.GetEntryCount(out long count);

                                    var entries = new DirectoryEntry[count];
                                    long entriesRead = 0;
                                    do
                                    {
                                        directory.Read(out entriesRead, entries.AsSpan());
                                    }
                                    while (entriesRead == 0);

                                    foreach (var directoryEntry in entries)
                                    {
                                        string name = System.Text.Encoding.UTF8.GetString(directoryEntry.Name).TrimEnd('\0');

                                        string fullPath = fsPath + "/" + name;

                                        string extractionPath = System.IO.Path.Combine(path, name);

                                        switch (directoryEntry.Type)
                                        {
                                            case DirectoryEntryType.File:
                                                section.OpenFile(out var file, fullPath.ToU8Span(), OpenMode.Read);

                                                Directory.CreateDirectory(new FileInfo(extractionPath).DirectoryName);

                                                using (var stream = File.OpenWrite(extractionPath))
                                                {
                                                    file.GetSize(out var size);

                                                    LogMessage($"{fullPath}");

                                                    file.AsStorage().CopyToStream(stream, size, this);
                                                }
                                                break;
                                            case DirectoryEntryType.Directory:
                                                ExtractDirectory(extractionPath, fullPath);
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ExplorerView_RowExpanded(object o, RowExpandedArgs args)
        {
            var treeStore = (_explorerView.Model as TreeModelFilter).Model as TreeStore;

            if(treeStore.IterNChildren(args.Iter) == 1 && treeStore.IterChildren(out var child, args.Iter))
            {
                string path = (string)treeStore.GetValue(args.Iter, 2);
                bool isPlaceHolder = (string)treeStore.GetValue(child, 0) == "[expand]";

                if (isPlaceHolder)
                {
                    treeStore.Remove(ref child);

                    TranverseAndExpand(args.Iter, path);

                    _explorerView.ExpandToPath(treeStore.GetPath(args.Iter));
                }
            }
        }

        private void TranverseAndExpand(TreeIter parentIter, string path)
        {
            var levels = path.Split("/", StringSplitOptions.RemoveEmptyEntries);

            string ncaName = string.Empty;

            switch (_appType)
            {
                case "xci":
                    var partitionType = Enum.Parse<XciPartitionType>(levels[0], true);
                    if (_xci.HasPartition(partitionType))
                    {
                        var partition = _xci.OpenPartition(partitionType);

                        ncaName = levels[1];

                        if (ncaName.ToLower().EndsWith(".nca"))
                        {
                            if (partition.FileExists(ncaName))
                            {
                                var entry = partition.Files.ToList().Find(x => x.Name == ncaName);

                                var nca = new Nca(VirtualFileSystem.KeySet, partition.OpenFile(entry, OpenMode.Read).AsStorage());

                                if (levels.Last() != ncaName)
                                {
                                    var ncaSection = Enum.Parse<NcaSectionType>(levels[2], true);

                                    string relativePath = string.Join('/', levels.AsSpan().Slice(3).ToArray());

                                    TransverseNca(nca, ncaSection, "/" + relativePath);
                                }
                                else
                                {
                                    ExpandNca(parentIter, nca, path);
                                }
                            }
                        }
                    }
                    break;
                case "nsp":
                    ncaName = levels[0];
                    if (ncaName.ToLower().EndsWith(".nca"))
                    {
                        if (_fs.FileExists(ncaName))
                        {
                            var entry = _fs.Files.ToList().Find(x => x.Name == ncaName);

                            var nca = new Nca(VirtualFileSystem.KeySet, _fs.OpenFile(entry, OpenMode.Read).AsStorage());

                            if (levels.Last() != ncaName)
                            {
                                var ncaSection = Enum.Parse<NcaSectionType>(levels[1], true);

                                string relativePath = string.Join('/', levels.AsSpan().Slice(2).ToArray());

                                TransverseNca(nca, ncaSection, "/" + relativePath);
                            }
                            else
                            {
                                ExpandNca(parentIter, nca, path);
                            }
                        }
                    }
                    break;
            }

            void TransverseNca(Nca nca, NcaSectionType sectionType, string relativePath)
            {
                if(nca.CanOpenSection(sectionType))
                {
                    var section = nca.OpenFileSystem(sectionType, IntegrityCheckLevel.IgnoreOnInvalid);

                    if (section != null)
                    {
                        if(section.DirectoryExists("/" + relativePath))
                        {
                            section.OpenDirectory(out var directory, relativePath.ToU8Span(), OpenDirectoryMode.All);

                            ExpandDirectory(parentIter, directory, section, path, relativePath);
                        }
                    }
                }
            }
        }

        private void CloseButton_Clicked(object sender, EventArgs args)
        {
            Dispose();
        }

        private void LoadApp()
        {
            if (File.Exists(AppPath))
            {
                var fileInfo = new FileInfo(AppPath);

                FileStream file = null;

                TreeIter iter;

                try
                {
                    ulong size = 0;
                    switch (fileInfo.Extension.ToLower())
                    {
                        case ".xci":
                            file = File.OpenRead(AppPath);
                            _xci = new Xci(VirtualFileSystem.KeySet, file.AsStorage());

                            foreach (var partitionType in Enum.GetValues<XciPartitionType>())
                            {
                                if (_xci.HasPartition(partitionType))
                                {
                                    var partition = _xci.OpenPartition(partitionType);

                                    size = (ulong)partition.Files.Sum(x => x.Size);

                                    iter = AddNode(TreeIter.Zero, partitionType.ToString(), partitionType.ToString(), size, "Partition");

                                    foreach (var entry in partition.Files)
                                    {
                                        string path = partitionType.ToString() + "/" + entry.Name;

                                        size = (ulong)entry.Size;

                                        string name = entry.Name;

                                        if (path.EndsWith(".nca"))
                                        {
                                            AddNca(iter, partition.OpenFile(entry, OpenMode.Read).AsStorage(), path, name);
                                        }
                                        else
                                        {
                                            AddNode(iter, name, path, size);
                                        }
                                    }
                                }
                            }

                            _appType = "xci";
                            break;
                        case ".nsp":
                        case ".pfs0":
                            file = File.OpenRead(AppPath);
                            _fs = new PartitionFileSystem(file.AsStorage());

                            size = (ulong)_fs.Files.Sum(x => x.Size);

                            iter = AddNode(TreeIter.Zero, "/", "/", size, "Partition");

                            foreach (var entry in _fs.Files)
                            {
                                string path = "/" + entry.Name;

                                size = (ulong)entry.Size;

                                string name = entry.Name;

                                if (path.EndsWith(".nca"))
                                {
                                    AddNca(iter, _fs.OpenFile(entry, OpenMode.Read).AsStorage(), path, name);
                                }
                                else if (_fs.DirectoryExists(path))
                                {
                                    iter = AddNode(iter, entry.Name, path, (ulong)entry.Size, "DIR");

                                    ExpandDirectory(iter, entry, _fs, path, path);
                                }
                                else
                                {
                                    AddNode(iter, name, path, size);
                                }
                            }

                            _appType = "nsp";
                            break;
                        default:
                            break;
                    }

                    if (file != null)
                    {
                        _sizeLabel.Text = $"{file.Length} bytes";
                        _typeLabel.Text = _appType.ToUpper();
                    }
                }
                catch (Exception ex)
                {
                    Dispose();
                }
            }
            else
            {
                Dispose();
            }
        }

        private TreeIter AddNode(TreeIter parentIter, string name, string fullPath, ulong size, string dataType = "", string titleId = "")
        {
            if(parentIter.Equals(TreeIter.Zero))
            {
                return ((_explorerView.Model as TreeModelFilter).Model as TreeStore).AppendValues(name, size, fullPath, dataType, titleId);
            }
            else 
            {
               return ((_explorerView.Model as TreeModelFilter).Model as TreeStore).AppendValues(parentIter, name, size, fullPath, dataType, titleId);
            }
        }

        private TreeIter AddNca(TreeIter parentIter, IStorage storage, string path, string name)
        {
            Nca nca = new Nca(VirtualFileSystem.KeySet, storage);

            TreeIter iter = TreeIter.Zero;

            storage.GetSize(out var size);

            string titleId = $"{nca.Header.TitleId:X16}";

            iter = AddNode(parentIter, name, path, (ulong)size, nca.Header.ContentType.ToString(), titleId);

            ExpandNca(iter, nca, path);

            return iter;
        }

        private void ExpandNca(TreeIter parentIter, Nca nca, string path)
        {
            foreach(var sectionType in Enum.GetValues<NcaSectionType>())
            {
                TreeIter iter = TreeIter.Zero;

                long size = 0;

                string contentPath = string.Empty;

                if(nca.CanOpenSection(sectionType))
                {
                    contentPath = path + "/" + sectionType.ToString().ToUpper();

                    var data = nca.OpenFileSystem(sectionType, IntegrityCheckLevel.IgnoreOnInvalid);

                    if (data != null)
                    {
                        data.GetTotalSpaceSize(out size, "/".ToU8Span());

                        iter = AddNode(parentIter, sectionType.ToString(), contentPath, (ulong)size, "Section");

                        ExpandFilesystem(iter, data, contentPath);
                    }
                }
            }
        }

        private void ExpandFilesystem(TreeIter parentIter, IFileSystem fileSystem, string path, bool recursive = false)
        {
            foreach(var entry in fileSystem.EnumerateEntries("*", SearchOptions.Default))
            {
                TreeIter iter = TreeIter.Zero;
                string entryPath = $"{path}/{entry.Name}";
                switch(entry.Type)
                {
                    case DirectoryEntryType.File:
                        fileSystem.OpenFile(out var file, entry.FullPath.ToU8Span(), OpenMode.Read);
                        file.GetSize(out var size);
                        iter = AddNode(parentIter, entry.Name, entryPath, (ulong)size, "");
                        break;
                    case DirectoryEntryType.Directory:
                        iter = AddNode(parentIter, entry.Name, entryPath, (ulong)entry.Size, "DIR");
                        if (recursive)
                        {
                            ExpandDirectory(iter, entry, fileSystem, entryPath);
                        }
                        else
                        {
                            iter = AddNode(iter, "[expand]", entryPath, 0, "");
                        }
                        break;
                }
            }
        }

        private void ExpandDirectory(TreeIter parentIter, DirectoryEntryEx entry, IFileSystem fileSystem, string path, bool recursive = false)
        {
            if(fileSystem.OpenDirectory(out var directory, entry.FullPath.ToU8Span(), OpenDirectoryMode.All) == Result.Success)
            {
                if (directory.GetEntryCount(out var count) == Result.Success)
                {
                    var entries = new DirectoryEntry[count];
                    long entriesRead = 0;
                    do
                    {
                        directory.Read(out entriesRead, entries.AsSpan());
                    }
                    while (entriesRead == 0);

                    foreach (var directoryEntry in entries)
                    {
                        string name = System.Text.Encoding.UTF8.GetString(directoryEntry.Name).TrimEnd('\0');
                        string fullPath = entry.FullPath + "/" + name;

                        TreeIter iter = TreeIter.Zero;
                        string entryPath = $"{path}/{name}";
                        switch (directoryEntry.Type)
                        {
                            case DirectoryEntryType.File:
                                fileSystem.OpenFile(out var file, fullPath.ToU8Span(), OpenMode.Read);
                                file.GetSize(out var size);
                                iter = AddNode(parentIter, name, entryPath, (ulong)size, "");
                                break;
                            case DirectoryEntryType.Directory:
                                iter = AddNode(parentIter, name, entryPath, (ulong)directoryEntry.Size, "DIR");
                                if (recursive)
                                {
                                    ExpandDirectory(iter, directoryEntry, fileSystem, entryPath, fullPath);
                                }
                                else
                                {
                                    iter = AddNode(iter, "[expand]", entryPath, 0, "");
                                }
                                break;
                        }
                    }
                }
            }
        }

        private void ExpandDirectory(TreeIter parentIter, DirectoryEntry entry, IFileSystem fileSystem, string path, string fsPath, bool recursive = false)
        {
            if (fileSystem.OpenDirectory(out var directory, fsPath.ToU8Span(), OpenDirectoryMode.All) == Result.Success)
            {
                if (directory.GetEntryCount(out var count) == Result.Success)
                {
                    var entries = new DirectoryEntry[count];
                    long entriesRead = 0;
                    do
                    {
                        directory.Read(out entriesRead, entries.AsSpan());
                    }
                    while (entriesRead == 0);

                    foreach (var directoryEntry in entries)
                    {
                        string name = System.Text.Encoding.UTF8.GetString(directoryEntry.Name).TrimEnd('\0');
                        string fullPath = fsPath + "/" + name;

                        TreeIter iter = TreeIter.Zero;
                        string entryPath = $"{path}/{name}";
                        switch (directoryEntry.Type)
                        {
                            case DirectoryEntryType.File:
                                fileSystem.OpenFile(out var file, fullPath.ToU8Span(), OpenMode.Read);
                                file.GetSize(out var size);
                                iter = AddNode(parentIter, name, entryPath, (ulong)size, "");
                                break;
                            case DirectoryEntryType.Directory:
                                iter = AddNode(parentIter, name, entryPath, (ulong)entry.Size, "DIR");
                                if (recursive)
                                {
                                    ExpandDirectory(iter, directoryEntry, fileSystem, entryPath, fullPath);
                                }
                                else
                                {
                                    iter = AddNode(iter, "[expand]", entryPath, 0, "");
                                }
                                break;
                        }
                    }
                }
            }
        }

        private void ExpandDirectory(TreeIter parentIter, IDirectory directory, IFileSystem fileSystem, string path, string fsPath, bool recursive = false)
        {
            if (directory.GetEntryCount(out var count) == Result.Success)
            {
                var entries = new DirectoryEntry[count];
                long entriesRead = 0;
                do
                {
                    directory.Read(out entriesRead, entries.AsSpan());
                }
                while (entriesRead == 0);

                foreach (var directoryEntry in entries)
                {
                    string name = System.Text.Encoding.UTF8.GetString(directoryEntry.Name).TrimEnd('\0');
                    string fullPath = fsPath + "/" + name;

                    TreeIter iter = TreeIter.Zero;
                    string entryPath = $"{path}/{name}";
                    switch (directoryEntry.Type)
                    {
                        case DirectoryEntryType.File:
                            fileSystem.OpenFile(out var file, fullPath.ToU8Span(), OpenMode.Read);
                            file.GetSize(out var size);
                            iter = AddNode(parentIter, name, entryPath, (ulong)size, "");
                            break;
                        case DirectoryEntryType.Directory:
                            iter = AddNode(parentIter, name, entryPath, 0, "DIR");
                            if (recursive)
                            {
                                ExpandDirectory(iter, directoryEntry, fileSystem, entryPath, fullPath);
                            }
                            else
                            {
                                iter = AddNode(iter, "[expand]", entryPath, 0, "");
                            }
                            break;
                    }
                }
            }
        }

        private void ExpandDirectory(TreeIter parentIter, PartitionFileEntry entry, IFileSystem fileSystem, string path, string fsPath,bool recursive = false)
        {
            if (fileSystem.OpenDirectory(out var directory, fsPath.ToU8Span(), OpenDirectoryMode.All) == Result.Success)
            {
                if (directory.GetEntryCount(out var count) == Result.Success)
                {
                    var entries = new DirectoryEntry[count];
                    long entriesRead = 0;
                    do
                    {
                        directory.Read(out entriesRead, entries.AsSpan());
                    }
                    while (entriesRead == 0);

                    foreach (var directoryEntry in entries)
                    {
                        string name = System.Text.Encoding.UTF8.GetString(directoryEntry.Name).TrimEnd('\0');
                        string fullPath = fsPath + "/" + name;

                        TreeIter iter = TreeIter.Zero;
                        string entryPath = $"{path}/{name}";
                        switch (directoryEntry.Type)
                        {
                            case DirectoryEntryType.File:
                                fileSystem.OpenFile(out var file, fullPath.ToU8Span(), OpenMode.Read);
                                file.GetSize(out var size);
                                iter = AddNode(parentIter, name, entryPath, (ulong)size, "");
                                break;
                            case DirectoryEntryType.Directory:
                                iter = AddNode(parentIter, name, entryPath, (ulong)entry.Size, "DIR");
                                if (recursive)
                                {
                                    ExpandDirectory(iter, directoryEntry, fileSystem, entryPath, fullPath);
                                }
                                else
                                {
                                    iter = AddNode(iter, "[expand]", entryPath, 0, "");
                                }
                                break;
                        }
                    }
                }
            }
        }

        protected override void OnDestroyed()
        {
            _xci = null;
            _fs = null;

            base.OnDestroyed();
        }

        public void Report(long value)
        {
            _operationCompleted = value;

            float total = _operationTotal;
            float completed = _operationCompleted;

            Gtk.Application.Invoke(delegate
            {
                _progressBar.Fraction = completed / total;
            });
        }

        public void ReportAdd(long value)
        {
            _operationCompleted += value;

            float total = _operationTotal;
            float completed = _operationCompleted;

            Gtk.Application.Invoke(delegate
            {
                _progressBar.Fraction = completed / total;
            });
        }

        public void SetTotal(long value)
        {
            _operationTotal = value;
            _operationCompleted = 0;

            Gtk.Application.Invoke(delegate
            {
                _progressBar.Fraction = 0;
            });
        }

        public void LogMessage(string message)
        {
            Gtk.Application.Invoke(delegate
            {
                _progressLabel.Text = message;
            });
        }
    }
}