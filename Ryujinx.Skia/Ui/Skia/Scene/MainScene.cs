using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Ryujinx.Skia.Ui.Skia.Widget;
using SkiaSharp;
using SkiaSharp.Elements;
using System.Threading.Tasks;
using LibHac.FsSystem.Save;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Ryujinx.Skia.App;
using Rectangle = Ryujinx.Skia.Ui.Skia.Widget.Rectangle;
using Image = SkiaSharp.Elements.Image;
using OpenTK.Windowing.Common.Input;
using System.Diagnostics;
using System.Threading;
using Ryujinx.Skia.Ui.Skia.Pages;

namespace Ryujinx.Skia.Ui.Skia.Scene
{
    public class MainScene : Scene
    {
        public ItemSize GameCardSizeMode { get; set; } = ItemSize.Normal;

        private const int HeaderHeightPercentage = 8;
        private const int FooterHeightPercentage = 8;
        private const int SidebarWidthPercentage = 25;

        private const string LightIcon = "sunny";
        private const string DarkIcon = "moon";

        private readonly Margin _margin;

        private readonly SKColor _borderColor = SKColors.LightGray;

        public bool ShowTitleNames { get; set; } = true;

        // Boundaries
        private readonly Rectangle _sideBar;
        private readonly Rectangle _background;
        private readonly Rectangle _header;

        // UI
        private readonly Widget.Image _logo;

        private readonly Label _title;
        private readonly Label _version;
        private readonly Label _firmware;

        //widget

        private readonly Box _sideBox;
        private readonly Box _infoBox;
        private readonly Box _quickOptionsBox;

        //Misc
        private readonly string _firmwareVersion;
        private bool _measured;
        private readonly ToggleButton _themeToggle;

        private Page _activePage;

        public MainScene()
        {
            _margin = new Margin(30, 20);

            _sideBar = new Rectangle(default)
            {
                BorderColor = _borderColor,
                FillColor = SKColors.Transparent
            };
            _header = new Rectangle(default)
            {
                BorderColor = _borderColor,
                FillColor = SKColors.Transparent
            };
            _background = new Rectangle(default)
            {
                FillColor = Theme.SceneBackgroundColor
            };

            _logo = new Widget.Image();
            string resourceID = "Ryujinx.Skia.Ui.Assets.Icon.png";
            Assembly assembly = GetType().GetTypeInfo().Assembly;

            using (Stream stream = assembly.GetManifestResourceStream(resourceID))
            {
                _logo.Load(SKBitmap.Decode(stream));
            }

            _logo.Width = 90;
            _logo.Height = 90;

            _logo.HorizontalAlignment = LayoutOptions.Center;
            _logo.VerticalAlignment = LayoutOptions.Center;

            _title = new Label("Ryujinx", 20)
            {
                HorizontalAlignment = LayoutOptions.Stretch,
                VerticalAlignment = LayoutOptions.Stretch,
                ForegroundColor = Theme.ForegroundColor,
                FontStyle = SKFontStyle.Bold,
                FontFamily = Theme.FontFamily,
                Margin = new Margin(0)
            };

            _version = new Label("Version", 16)
            {
                HorizontalAlignment = LayoutOptions.Stretch,
                VerticalAlignment = LayoutOptions.Stretch,
                ForegroundColor = Theme.ForegroundColor,
                FontFamily = Theme.FontFamily,
                Margin = new Margin(0)
            };

            _firmware = new Label("Firmware", 16)
            {
                HorizontalAlignment = LayoutOptions.Stretch,
                VerticalAlignment = LayoutOptions.Stretch,
                ForegroundColor = Theme.ForegroundColor,
                FontFamily = Theme.FontFamily,
                Margin = new Margin(0)
            };

            _sideBox = new Box(default)
            {
                Orientation = Orientation.Vertical
            };

            _quickOptionsBox = new Box(default)
            {
                Orientation = Orientation.Vertical
            };

            _infoBox = new Box(default)
            {
                Orientation = Orientation.Vertical,
                ContentSpacing = 0
            };

            Elements.Add(_background);
            Elements.Add(_sideBar);
           // Elements.Add(_header);

            AddElement(_sideBox);

            AddElement(_infoBox);

            AddElement(_quickOptionsBox);
            
            _themeToggle = new ToggleButton(OptionType.Icon);
            _themeToggle.StateChange += ThemeToggle_StateChanged;
            _themeToggle.HorizontalAlignment = LayoutOptions.Center;

            _quickOptionsBox.AddElement(_themeToggle);
            
            List<string> states = new List<string>(Enum.GetNames(typeof(ItemSize)));

            states = new List<string>()
            { 
                LightIcon,
                DarkIcon
            };

            _themeToggle.SetStates(states);

            _themeToggle.SetSelected(LightIcon);

            _firmwareVersion = (IManager.Instance as RenderWindow).FirmwareVersion?.VersionString;

            _version.Text = "Version 0.1";
            _firmware.Text = $"Firmware {_firmwareVersion}";

            _activePage = new HomePage();

            _activePage.AttachTo(this);
        }

        private void ThemeToggle_StateChanged(object sender, ContextMenu.OptionSelectedEventArgs e)
        {
            if (e.SelectedOption == LightIcon)
            {
                Theme = Themes.Light;
            }
            else if (e.SelectedOption == DarkIcon)
            {
                Theme = Themes.Dark;
            }

            IManager.Instance.InvalidateMeasure();
        }

        public override void OnNavigatedTo()
        {
            SKWindow.TargetFps = 60;

            _logo.StartDelay(500);
            AddElement(_logo);
            _sideBox.Elements.Clear();
            _infoBox.Elements.Clear();
            _infoBox.AddElement(_title);
            _infoBox.AddElement(_version);
            _infoBox.AddElement(_firmware);

            _sideBox.Margin = new Margin(20, 10, 20, 10);

            _sideBox.StartDelay(500);
            _infoBox.StartDelay(500);

            IManager.Instance.InvalidateMeasure();

            Loaded = true;
        }

        public override void OnNavigatedFrom()
        {
            Elements.Remove(_logo);
            _sideBox.Elements.Clear();
        }

        public override void Draw(SKCanvas canvas)
        {
            if (!_measured)
            {
                return;
            }

            _background.FillColor = Theme.SceneBackgroundColor;
            _sideBar.FillColor = Theme.SideBarColor;
            _header.FillColor = Theme.SideBarColor;

            base.Draw(canvas);
            _title.DrawOverlay(canvas);
            _version.DrawOverlay(canvas);

            _activePage.Draw(canvas);
        }

        public override void Measure()
        {
            base.Measure();

            SKRect bounds = IManager.Instance.Bounds;

            _background.Bounds = bounds;

            _header.Size = new SKSize(120, 120);
            _header.Location = new SKPoint(_margin.Left, _margin.Top);

            _quickOptionsBox.Measure(_header.Bounds);

            _sideBar.Height = bounds.Height;
            _sideBar.Width = bounds.Width * (SidebarWidthPercentage / 100f);
            _sideBar.Width = MathF.Min(250, _sideBar.Width);
            _sideBar.Location = new SKPoint(0, 0);

            _sideBox.Width = 160;
            _sideBox.Height = _sideBar.Height - _sideBox.Margin.Top - _sideBox.Margin.Bottom;
            _sideBox.Location = new SKPoint(_margin.Left, _margin.Top);

            _logo.Location = new SKPoint(_sideBar.Left + _margin.Left, bounds.Bottom - 150);

            _logo.Measure();

            _infoBox.Width = _sideBar.Width - _logo.Width - 10;
            _infoBox.ContentSpacing = 5;
            _infoBox.Location = new SKPoint(_logo.Right + 10, _logo.Top - 10);
            _infoBox.Height = bounds.Bottom - _margin.Bottom - _infoBox.Top;
            _infoBox.Measure();

            _sideBox.Measure();

            SKRect pageBounds = SKRect.Create(_sideBar.Right, 0, bounds.Width - _sideBar.Width, bounds.Height);

            _activePage.Measure(pageBounds);

            _measured = true;
        }
    }
}