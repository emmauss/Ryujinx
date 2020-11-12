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

namespace Ryujinx.Skia.Ui.Skia.Scene
{
    public class HomeScene : Scene
    {
        public GameCardSizeMode GameCardSizeMode { get; set; } = GameCardSizeMode.Normal;

        private const int HeaderHeightPercentage = 8;
        private const int FooterHeightPercentage = 8;
        private const int SidebarWidthPercentage = 25;

        private const string LightIcon = "sunny";
        private const string DarkIcon = "moon";

        private readonly Margin _margin;

        private readonly SKColor _borderColor = SKColors.LightGray;

        public bool ShowTitleNames { get; set; } = true;

        // Boundaries
        private readonly Rectangle _topBar;
        private readonly Rectangle _bottomBar;
        private readonly Rectangle _sideBar;
        private readonly Rectangle _background;
        private readonly Rectangle _header;

        // UI
        private readonly Widget.Image _logo;

        private readonly Label _title;
        private readonly Label _version;
        private readonly Label _firmware;

        private readonly Label _heading;

        private readonly Label _loadedLabel;

        private readonly Entry _search;

        //widget

        private readonly Box _sideBox;
        private readonly Box _topBox;
        private readonly Box _infoBox;

        private bool _isLoading;

        private readonly GameList _gameList;

        //Misc
        private readonly string _firmwareVersion;
        private string _searchFilter;
        private bool _measured;
        private readonly ActionButton _searchIcon;
        private readonly ActionButton _extraOptions;
        private readonly ActionButton _refreshOption;

        private readonly OptionsMenuPopup _optionsPopUp;
        private readonly Checkbutton _nameToggleCheckButton;
        private readonly ToggleButton _sizeStateToggle;

        private readonly ToggleButton _themeToggle;

        public HomeScene()
        {
            _margin = new Margin(30, 40);

            _topBar = new Rectangle(default)
            {
                BorderColor = _borderColor,
                FillColor = SKColors.Transparent
            };
            _bottomBar = new Rectangle(default)
            {
                BorderColor = _borderColor,
                FillColor = SKColors.Transparent
            };
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

            _loadedLabel = new Label("", 20)
            {
                HorizontalAlignment = LayoutOptions.Stretch,
                VerticalAlignment = LayoutOptions.Stretch,
                ForegroundColor = Theme.ForegroundColor,
                FontStyle = SKFontStyle.Bold,
                FontFamily = Theme.FontFamily,
                Margin = new Margin(0)
            };

            _heading = new Label("Library", 30)
            {
                HorizontalAlignment = LayoutOptions.Stretch,
                VerticalAlignment = LayoutOptions.Stretch,
                ForegroundColor = Theme.ForegroundColor,
                FontStyle = SKFontStyle.Bold,
                FontFamily = Theme.FontFamily,
                Margin = new Margin(5)
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

            _gameList = new GameList(default)
            {
                LayoutOptions = LayoutOptions.Center
            };

            _sideBox = new Box(default)
            {
                Orientation = Orientation.Vertical
            };

            _topBox = new Box(default)
            {
                Orientation = Orientation.Horizontal,
                LayoutOptions = LayoutOptions.End
            };

            _infoBox = new Box(default)
            {
                Orientation = Orientation.Vertical,
                ContentSpacing = 0
            };

            Elements.Add(_background);

            AddElement(_sideBox);

            AddElement(_infoBox);
            AddElement(_gameList);
            AddElement(_topBox);

            _search = new Entry();
            _search.Measure();
            _search.Bounds = SKRect.Create(0, 0, 150, _search.Height);

            _searchIcon = new ActionButton("search")
            {
                IconWidth = 25,
                VerticalAlignment = LayoutOptions.Center,
                HorizontalAlignment = LayoutOptions.Center
            };

            _searchIcon.Activate += SearchIcon_Activate;

            _extraOptions = new ActionButton("options-outline")
            {
                IconWidth = 25,
                VerticalAlignment = LayoutOptions.Center,
                HorizontalAlignment = LayoutOptions.Center
            };
            
            _refreshOption = new ActionButton("reload-outline")
            {
                IconWidth = 25,
                VerticalAlignment = LayoutOptions.Center,
                HorizontalAlignment = LayoutOptions.Center
            };

            _refreshOption.Activate += RefreshOption_Activate;

            _extraOptions.Activate += ExtraOptions_Activate;

            _topBox.AddElement(_search);
            _topBox.AddElement(_searchIcon);
            _topBox.AddElement(_extraOptions);
            _topBox.AddElement(_refreshOption);

            AddElement(_loadedLabel);

            _optionsPopUp = new OptionsMenuPopup();
            _optionsPopUp.AttachTo(this);
            _optionsPopUp.AttachedElement = _extraOptions;

            _nameToggleCheckButton = new Checkbutton("Show Game Titles");
            _nameToggleCheckButton.Activate += NameToggleCheckButton_Activate;
            _nameToggleCheckButton.BorderColor = SKColors.Transparent;
            _nameToggleCheckButton.Checked = ShowTitleNames;

            _optionsPopUp.AddWidget(_nameToggleCheckButton);

            _sizeStateToggle = new ToggleButton(OptionType.Label);
            _sizeStateToggle.StateChange += SizeStateToggle_StateChanged;
            
            _themeToggle = new ToggleButton(OptionType.Icon);
            _themeToggle.StateChange += ThemeToggle_StateChanged;
            _themeToggle.HorizontalAlignment = LayoutOptions.Center;

            Label label = new Label("Game Icon Size")
            {
                ForegroundColor = Theme.ForegroundColor,
                FontSize = TextSize
            };

            _optionsPopUp.AddWidget(label);
            
            label = new Label("Theme")
            {
                ForegroundColor = Theme.ForegroundColor,
                FontSize = TextSize
            };

            _optionsPopUp.AddWidget(_sizeStateToggle);
            
            _optionsPopUp.AddWidget(label);

            _optionsPopUp.AddWidget(_themeToggle);
            
            List<string> states = new List<string>(Enum.GetNames(typeof(GameCardSizeMode)));

            _sizeStateToggle.SetStates(states);

            _sizeStateToggle.SetSelected(GameCardSizeMode.ToString());

            states = new List<string>()
            { 
                LightIcon,
                DarkIcon
            };

            _themeToggle.SetStates(states);

            _themeToggle.SetSelected(LightIcon);

            _search.BackgroundColor = SKColor.Parse("#e1e1e1");

            _search.Input += Search_Input;

            _firmwareVersion = (IManager.Instance as RenderWindow).FirmwareVersion?.VersionString;

            _version.Text = "Version 0.1";
            _firmware.Text = $"Firmware {_firmwareVersion}";

            Task.Run(ReloadApps);
        }

        private void RefreshOption_Activate(object sender, EventArgs e)
        {
            if (_isLoading)
            {
                return;
            }

            Task.Run(ReloadApps);
        }

        private void ReloadApps()
        {
            if (_isLoading)
            {
                return;
            }

            _isLoading = true;

            _gameList.Loading = true;

            if (IManager.Instance is RenderWindow renderWindow)
            {
                renderWindow.RefreshApplicationLibrary();
            }

            _isLoading = false;

            LoadGameList();
        }

        private void SizeStateToggle_StateChanged(object sender, ContextMenu.OptionSelectedEventArgs e)
        {
            if (Enum.TryParse(typeof(GameCardSizeMode), e.SelectedOption, true, out var nextMode))
            {
                GameCardSizeMode = (GameCardSizeMode)nextMode;

                _gameList.ScrollTo(0);

                IManager.Instance.InvalidateMeasure();
            }
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

        private void NameToggleCheckButton_Activate(object sender, EventArgs e)
        {
            ShowTitleNames = (sender as Checkbutton).Checked;

            IManager.Instance.InvalidateMeasure();
        }

        private void ExtraOptions_Activate(object sender, EventArgs e)
        {
            SKPoint popupLocation = new SKPoint(_extraOptions.Bounds.MidX - _optionsPopUp.Width / 2, _extraOptions.Bottom + 10);
            _optionsPopUp.Show(popupLocation);

            IManager.Instance.InvalidateMeasure();
        }

        private void SearchIcon_Activate(object sender, EventArgs e)
        {
            _searchFilter = _search.Text;

            Task.Run(LoadGameList);

            IManager.Instance.InvalidateMeasure();
        }

        private void Search_Input(object sender, IInput.InputEventArgs e)
        {
            if (e.Key == Keys.Enter)
            {
                if (sender is Entry search)
                {
                    _searchFilter = search.Text;

                    Task.Run(LoadGameList);
                }
            }

            IManager.Instance.InvalidateMeasure();
        }

        public override void OnNavigatedTo()
        {
            SKWindow.TargetFps = 60;

            Task.Run(LoadGameList);
            _logo.StartDelay(500);
            AddElement(_logo);
            _sideBox.Elements.Clear();
            _infoBox.Elements.Clear();
            _infoBox.AddElement(_title);
            _infoBox.AddElement(_version);
            _infoBox.AddElement(_firmware);

            _sideBox.Margin = new Margin(20, 10, 20, 10);

            /* Button button = new Button("test modal");
             button.Activate += Button_Activate;


             _sideBox.AddElement(button);
 */
            _sideBox.StartDelay(500);
            _infoBox.StartDelay(500);

            IManager.Instance.InvalidateMeasure();

            //Button_Activate(null, null);

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
            base.Draw(canvas);
            _title.DrawOverlay(canvas);
            _version.DrawOverlay(canvas);
            _heading.Draw(canvas);
        }

        public override void Measure()
        {
            base.Measure();

            SKRect bounds = IManager.Instance.Bounds;

            _background.Bounds = bounds;

            _header.Size = new SKSize(120, 120);
            _header.Location = new SKPoint(_margin.Left, _margin.Top);

            _topBar.Width = bounds.Width - _header.Width - _margin.Right;
            _topBar.Height = bounds.Height * (HeaderHeightPercentage / 100f);

            _sideBar.Height = bounds.Height - _header.Height;
            _sideBar.Width = bounds.Width * (SidebarWidthPercentage / 100f);
            _sideBar.Width = MathF.Min(250, _sideBar.Width);
            _sideBar.Location = new SKPoint(_margin.Left, _header.Height);

            _bottomBar.Width = bounds.Width - _sideBar.Right;
            _bottomBar.Height = bounds.Height * (FooterHeightPercentage / 100f);

            _topBar.Location = new SKPoint(_header.Width + 1, 25);
            _bottomBar.Location = new SKPoint(_margin.Left, bounds.Bottom - _bottomBar.Height);

            _sideBox.Width = 160;
            _sideBox.Height = _sideBar.Height - _sideBox.Margin.Top - _sideBox.Margin.Bottom;
            _sideBox.Location = new SKPoint(_margin.Left, _bottomBar.Top - _sideBox.Height - _sideBox.Margin.Bottom);

            _logo.Location = new SKPoint(_sideBar.Left, bounds.Bottom - 150);

            _logo.Measure();

            _infoBox.Width = _sideBar.Width - _logo.Width - 10;
            _infoBox.ContentSpacing = 5;
            _infoBox.Location = new SKPoint(_logo.Right + 10, _logo.Top - 10);
            _infoBox.Height = bounds.Bottom - _margin.Bottom - _infoBox.Top;
            _infoBox.Measure();

            float topBoxWidth = _topBar.Width * 0.6f;
            _topBox.Bounds = SKRect.Create(new SKPoint(_topBar.Right - topBoxWidth, _topBar.Top - _topBox.Margin.Top), new SKSize(topBoxWidth, _topBar.Height - _topBox.Margin.Top));
            _topBox.Measure(_topBox.Bounds);

            _sideBox.Measure();

            //_gameList.Width = bounds.Width - _sideBar.Width - _margin.Right;
            _gameList.Width = bounds.Right - _margin.Right - _sideBar.Right - _gameList.Margin.Left - _gameList.Margin.Right;
            _gameList.Height = _bottomBar.Top - _topBar.Bottom - 20;
            _gameList.Location = new SKPoint(_sideBar.Right + _gameList.Margin.Left, _topBar.Bottom + 20);

            _gameList.Measure();

            _heading.Measure();
            _heading.Location = new SKPoint(_gameList.Left + _heading.Margin.Left, _topBar.Bottom - _heading.Height - _heading.Margin.Bottom);
            _heading.Measure();

            _loadedLabel.Measure();
            _loadedLabel.Location = new SKPoint(_bottomBar.Right, _bottomBar.Top + 10);
            _loadedLabel.Measure();

            _measured = true;
        }

        public void LoadGameList()
        {
            if (_isLoading)
            {
                return;
            }

            _isLoading = true;

            foreach (var gamecard in _gameList.Elements)
            {
                (gamecard as UIElement)?.Dispose();
            }

            _gameList.Elements.Clear();

            _gameList.Loading = true;

            _loadedLabel.Text = string.Empty;

            _gameList.Measure();

            int loaded = 0;

            ApplicationLibrary.Filter(ApplicationFilter.Title, _searchFilter);

            int count = ApplicationLibrary.Filtered.Count;

            float delayInterval = MathF.Round(2000f / count);

            _gameList.Loading = false;

            _loadedLabel.Text = $"0 of {count} Apps Loaded";

            foreach (var app in ApplicationLibrary.Filtered)
            {
                GameCard card = new GameCard(app)
                {
                    BackgroundColor = Theme.BackgroundColor
                };

                card.Width = 150;
                card.Height = 200;

                loaded++;

                card.StartDelay(loaded * (int)delayInterval);

                _gameList.AddElement(card);

                _loadedLabel.Text = $"{loaded} of {count} Apps Loaded";
            }

            IManager.Instance.InvalidateMeasure();

            _isLoading = false;
        }
    }
}