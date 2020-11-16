using Ryujinx.Skia.Ui.Skia.Scene;
using SkiaSharp;
using SkiaSharp.Elements;
using SkiaSharp.Elements.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace Ryujinx.Skia.Ui.Skia.Widget
{
    public class FileDialog : Dialog
    {
        public string Selected
        {
            get
            {
                var select = _fileList.SelectedItem;

                if (select != null)
                {
                    return Path.Combine(_currentDirectory, select.Value.ToString());
                }

                return string.Empty;
            }
        }

        private Box _navBox;
        private ListBox _fileList;
        private readonly string selected;
        private Stack<FileSystemLevel> _backLevels;
        private Stack<FileSystemLevel> _nextLevels;

        private FileSystemLevel _activeLevel;
        private ActionButton _backButton;
        private ActionButton _fowardButton;
        private ActionButton _upButton;
        private ActionButton _enterButton;
        private Entry _navEntry;

        private string _currentDirectory;

        public FileDialog(Scene.Scene parent,
                string title,
                DialogButtons buttons,
                string acceptButtonText = "",
                string declineButtonText = "",
                string cancelButtonText = "") : base(parent, title, buttons, acceptButtonText, declineButtonText, cancelButtonText)
        {
            _navBox = new Box(default)
            {
                Orientation = Orientation.Horizontal,
                VerticalAlignment = LayoutOptions.Stretch,
                HorizontalAlignment = LayoutOptions.Stretch,
                BackgroundColor = SKColors.Transparent,
                ScrollEnabled = false
            };

            _fileList = new ListBox(default);

            _fileList.HorizontalAlignment = LayoutOptions.Stretch;
            _fileList.VerticalAlignment = LayoutOptions.Stretch;

            _navBox.AttachTo(parent);

            _backButton = new ActionButton("arrow-back-outline", default)
            {
                IconWidth = 25,
                VerticalAlignment = LayoutOptions.Center,
                HorizontalAlignment = LayoutOptions.Center
            };

            _fowardButton = new ActionButton("arrow-forward-outline", default)
            {
                IconWidth = 25,
                VerticalAlignment = LayoutOptions.Center,
                HorizontalAlignment = LayoutOptions.Center
            };

            _upButton = new ActionButton("arrow-up-outline", default)
            {
                IconWidth = 25,
                VerticalAlignment = LayoutOptions.Center,
                HorizontalAlignment = LayoutOptions.Center
            };

            _enterButton = new ActionButton("enter-outline", default)
            {
                IconWidth = 25,
                VerticalAlignment = LayoutOptions.Center,
                HorizontalAlignment = LayoutOptions.Center
            };

            _backButton.Activate += Back_Button_Activate;
            _fowardButton.Activate += Forward_Button_Activate;
            _upButton.Activate += Up_Button_Activate;
            _enterButton.Activate += Enter_Button_Activate;

            _navEntry = new Entry();
            _navEntry.Measure();
            _navEntry.Bounds = SKRect.Create(0, 0, 750, _navEntry.Height);
            _navEntry.VerticalAlignment = LayoutOptions.Center;

            _navBox.AddElement(_backButton);
            _navBox.AddElement(_fowardButton);
            _navBox.AddElement(_upButton);
            _navBox.AddElement(_navEntry);
            _navBox.AddElement(_enterButton);

            _fileList.AttachTo(parent);

            _fileList.Padding = new Margin(10);

            _navBox.Padding = new Margin(0, 10, 0, 10);

            _fileList.ItemActivated += ListBox_ItemActivated;

            _backLevels = new Stack<FileSystemLevel>();
            _nextLevels = new Stack<FileSystemLevel>();

            ChangeDirectory("/home/nhv3/personal/Ryujinx");

            FixedHeight = true;

            DialogHeight = 600;
            DialogWidth = 1000;
        }

        private void Enter_Button_Activate(object sender, EventArgs e)
        {
            var newPath = _navEntry.Text;

            if (Directory.Exists(newPath))
            {
                ChangeDirectory(newPath);
            }
        }

        private void Up_Button_Activate(object sender, EventArgs e)
        {
            lock(this)
            {
                var directoryInfo = new DirectoryInfo(_activeLevel.Path);

                var parent = directoryInfo.Parent;

                if(parent.Exists)
                {
                    ChangeDirectory(parent.FullName);
                }
            }
        }

        private void Forward_Button_Activate(object sender, EventArgs e)
        {
            lock (this)
            {
                if (_nextLevels.Count > 0)
                {
                    var level = _nextLevels.Pop();

                    _backLevels.Push(_activeLevel);

                    _activeLevel = level;

                    RefreshList();
                }
            }
        }

        private void Back_Button_Activate(object sender, EventArgs e)
        {
            lock (this)
            {
                if (_backLevels.Count > 0)
                {
                    var level = _backLevels.Pop();

                    _nextLevels.Push(_activeLevel);

                    _activeLevel = level;

                    RefreshList();
                }
            }
        }

        private void ListBox_ItemActivated(object sender, ItemSelectedArgs e)
        {
            if(e != null)
            {
                var path = Path.Combine(_currentDirectory, e.Item.Value.ToString());

                if(Directory.Exists(path))
                {
                    ChangeDirectory(path);
                }
            }
        }

        public void ChangeDirectory(string path)
        {
            var filesystem = new FileSystemLevel(path);

            filesystem.RefreshFiles();

            if (_activeLevel == null)
            {
                _activeLevel = filesystem;
            }
            else
            {
                _backLevels.Push(_activeLevel);
                _nextLevels.Clear();
                _activeLevel = filesystem;
            }

            RefreshList(false);
        }

        public void RefreshList(bool refreshFiles = false)
        {
            _currentDirectory = Path.GetFullPath(_activeLevel.Path);

            _navEntry.Text = _currentDirectory;

            if (_activeLevel != null)
            {
                if (refreshFiles)
                {
                    _activeLevel.RefreshFiles();
                }

                _fileList.Clear();

                foreach (var entry in _activeLevel.Directories)
                {
                    _fileList.Add(entry.Name);
                }

                foreach (var entry in _activeLevel.Files)
                {
                    _fileList.Add(entry.Name);
                }
            }
        }

        public override void Draw(SKCanvas canvas)
        {
            base.Draw(canvas);
        }

        public override void DrawContent(SKCanvas canvas)
        {
           _navBox.Draw(canvas);

            _fileList.Draw(canvas);
        }

        public override SKRect MeasureContent(SKRect bounds)
        {
            SKRect newBounds = new SKRect();

            if(bounds.Size == default)
            {
                _navBox.Measure(bounds);

                newBounds.Size += _navBox.Bounds.Size;

                SKRect listBounds = SKRect.Create(newBounds.Location + new SKPoint(0, _navBox.Size.Height + 10), new SKSize( DialogWidth - 20,DialogHeight - _navBox.Height - 30));

                _fileList.Measure(listBounds);

                newBounds.Size += listBounds.Size + new SKSize(20, 20);
            }
            else
            {
                var navBoxBounds = bounds;

                navBoxBounds.Size = new SKSize(bounds.Width, _navBox.Height);

                _navBox.Measure(navBoxBounds);

                SKRect listBounds = SKRect.Create(new SKPoint(_navBox.Left, _navBox.Bottom + 5), new SKSize(bounds.Width, bounds.Height - _navBox.Height - 40));

                _fileList.Measure(listBounds);

                newBounds = bounds;
            }

            _backButton.Enabled = _backLevels.Count > 0;
            _fowardButton.Enabled = _nextLevels.Count > 0;

            return newBounds;
        }

        public override Element GetElementInContent(SKPoint point)
        {
            if(_navBox.IsPointInside(point))
            {
                return _navBox.GetElementAtPosition(point);
            }

            if(_fileList.IsPointInside(point))
            {
                return _fileList;
            }

            return null;
        }
    }
}