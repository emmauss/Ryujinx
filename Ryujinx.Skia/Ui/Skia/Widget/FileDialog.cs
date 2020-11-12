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
                    return select.Value.ToString();
                }

                return string.Empty;
            }
        }

        private Box _contentBox;
        private Box _navBox;
        private ListBox _fileList;
        private readonly string selected;

        private Stack<FileSystemLevel> _backLevels;
        private Stack<FileSystemLevel> _nextLevels;

        private FileSystemLevel _activeLevel;

        public FileDialog(Scene.Scene parent,
                string title,
                DialogButtons buttons,
                string acceptButtonText = "",
                string declineButtonText = "",
                string cancelButtonText = "") : base(parent, title, buttons, acceptButtonText, declineButtonText, cancelButtonText)
        {
            _contentBox = new Box(default)
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = LayoutOptions.Center,
                BackgroundColor = SKColors.Transparent,
                ScrollEnabled = false
            };

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

            _contentBox.AttachTo(parent);

            _contentBox.AddElement(_navBox);

            _navBox.AddElement(_fileList);

            ChangeDirectory("/home/nhv3/personal/Ryujinx");

            FixedHeight = true;

            DialogHeight = 600;
            DialogWidth = 1000;
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

            RefreshList();
        }

        public void RefreshList(bool refreshFiles = false)
        {
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

        public override void Measure()
        {
            base.Measure();

            _navBox.Measure(SKRect.Create(_navBox.Location, new SKSize(_navBox.Width, 450)));
        }

        public override void Draw(SKCanvas canvas)
        {
            base.Draw(canvas);

            var d = DrawElement;
        }

        public override void DrawContent(SKCanvas canvas)
        {
            throw new NotImplementedException();
        }

        public override SKRect MeasureContent(SKRect bounds)
        {
            throw new NotImplementedException();
        }

        public override UIElement GetElementInContent(SKPoint point)
        {
            throw new NotImplementedException();
        }
    }
}