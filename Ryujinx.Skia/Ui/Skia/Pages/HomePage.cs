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
using Ryujinx.Skia.Ui.Skia.Scene;

namespace Ryujinx.Skia.Ui.Skia.Pages
{
    public class HomePage : Page
    {
        public ItemSize GameCardSizeMode { get; set; } = ItemSize.Normal;

        private const int HeaderHeightPercentage = 8;
        private const int FooterHeightPercentage = 8;
        private readonly Margin _margin;

        private readonly SKColor _borderColor = SKColors.LightGray;

        public bool ShowTitleNames { get; set; } = true;

        // Boundaries
        private readonly Rectangle _topBar;
        private readonly Rectangle _bottomBar;
        private readonly Rectangle _background;

        // UI

        private readonly Label _heading;

        private readonly Label _loadedLabel;

        private readonly Entry _search;

        //widget
        private readonly Box _topBox;

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

        public HomePage()
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
            _background = new Rectangle(default);

            _loadedLabel = new Label("", 20)
            {
                HorizontalAlignment = LayoutOptions.Stretch,
                VerticalAlignment = LayoutOptions.Stretch,
                FontStyle = SKFontStyle.Bold,
                Margin = new Margin(0)
            };

            _heading = new Label("Library", 30)
            {
                HorizontalAlignment = LayoutOptions.Stretch,
                VerticalAlignment = LayoutOptions.Stretch,
                FontStyle = SKFontStyle.Bold,
                Margin = new Margin(5)
            };

            _gameList = new GameList(default)
            {
                LayoutOptions = LayoutOptions.Center
            };

            _topBox = new Box(default)
            {
                Orientation = Orientation.Horizontal,
                LayoutOptions = LayoutOptions.End
            };

            Elements.Add(_background);

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
            _optionsPopUp.Content = _extraOptions;

            _nameToggleCheckButton = new Checkbutton("Show Game Titles");
            _nameToggleCheckButton.Activate += NameToggleCheckButton_Activate;
            _nameToggleCheckButton.BorderColor = SKColors.Transparent;
            _nameToggleCheckButton.Checked = ShowTitleNames;

            _optionsPopUp.AddWidget(_nameToggleCheckButton);

            _sizeStateToggle = new ToggleButton(OptionType.Label);
            _sizeStateToggle.StateChange += SizeStateToggle_StateChanged;

            Label label = new Label("Game Icon Size", TextSize);

            _optionsPopUp.AddWidget(label);

            _optionsPopUp.AddWidget(_sizeStateToggle);

            List<string> states = new List<string>(Enum.GetNames(typeof(ItemSize)));

            _sizeStateToggle.SetStates(states);

            _sizeStateToggle.SetSelected(GameCardSizeMode.ToString());

            _search.BackgroundColor = SKColor.Parse("#e1e1e1");

            _search.Input += Search_Input;

            _firmwareVersion = (IManager.Instance as RenderWindow).FirmwareVersion?.VersionString;

            Task.Run(ReloadApps);
        }

        public override void DrawContent(SKCanvas canvas)
        {
            if (!_measured)
            {
                return;
            }

            _background.FillColor = ParentScene.Theme.SceneBackgroundColor;
            _heading.Draw(canvas);
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
            if (Enum.TryParse(typeof(ItemSize), e.SelectedOption, true, out var nextMode))
            {
                GameCardSizeMode = (ItemSize)nextMode;

                _gameList.ScrollTo(0);

                IManager.Instance.InvalidateMeasure();
            }
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

        public override void AttachTo(Scene.Scene parent)
        {
            base.AttachTo(parent);

            _optionsPopUp.AttachTo(parent);

            SKWindow.TargetFps = 60;

            _sizeStateToggle.SetSelected(GameCardSizeMode.ToString());

            Task.Run(LoadGameList);

            IManager.Instance.InvalidateMeasure();

            Loaded = true;
        }

        public override void Measure()
        {
            SKRect bounds = Bounds;

            _background.Bounds = bounds;

            _topBar.Width = bounds.Width;
            _topBar.Height = bounds.Height * (HeaderHeightPercentage / 100f);

            _bottomBar.Width = bounds.Width;
            _bottomBar.Height = bounds.Height * (FooterHeightPercentage / 100f);

            _topBar.Location = new SKPoint(bounds.Left + 1, 25);
            _bottomBar.Location = new SKPoint(_margin.Left, bounds.Bottom - _bottomBar.Height);

            float topBoxWidth = _topBar.Width * 0.6f;
            _topBox.Bounds = SKRect.Create(new SKPoint(_topBar.Right - topBoxWidth, _topBar.Top - _topBox.Margin.Top), new SKSize(topBoxWidth, _topBar.Height - _topBox.Margin.Top));
            _topBox.Measure(_topBox.Bounds);

            //_gameList.Width = bounds.Width - _sideBar.Width - _margin.Right;
            _gameList.Width = bounds.Width - _gameList.Margin.Left - _gameList.Margin.Right;
            _gameList.Height = _bottomBar.Top - _topBar.Bottom - 20;
            _gameList.Location = new SKPoint(bounds.Left + _gameList.Margin.Left, _topBar.Bottom + 20);

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
                    BackgroundColor = ParentScene.Theme.BackgroundColor
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

        public override void Measure(SKRect bounds)
        {
            Bounds = bounds;

            Measure();
        }
    }
}