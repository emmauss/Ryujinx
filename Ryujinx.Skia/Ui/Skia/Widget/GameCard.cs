using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using LibHac.Fs;
using Ryujinx.Skia.App;
using Ryujinx.Skia.Ui.Skia.Scene;
using SkiaSharp;
using SkiaSharp.Elements;

using static Ryujinx.Skia.Ui.Skia.Widget.ContextActions;

namespace Ryujinx.Skia.Ui.Skia.Widget
{
    public class GameCard : UIElement, IHoverable, IAction, ISelectable
    {
        public const int GameCardWidth  = 180;
        public const int GameCardHeight = 220;

        private bool _showTitle;

        private SKRect _textBounds;

        public event EventHandler<EventArgs> Activate;

        public Image Image { get; set; }
        
        public Label Title { get; set; }
        //public Label LastPlayed { get; set; }
        private readonly Rectangle _boundingElement;
        private readonly Rectangle _imageBorder;

        public string Path { get; set; }

        public bool IsSelected{ get; set; }
        public bool IsHovered { get; set ; }

        private ApplicationData _applicationData{ get; set; }

        public GameCard(ApplicationData applicationData) : this()
        {
            Image = new Image(applicationData.Icon);
            Title = new Label(applicationData.TitleName, 16);
            Path = applicationData.Path;
            Title.IsAnimated = true;
            Title.FontStyle = SKFontStyle.Bold;
            _applicationData = applicationData;
        }

        private GameCard()
        {
            _boundingElement = new Rectangle(default)
            {
                BorderColor = SKColors.Transparent
            };
            
            _imageBorder = new Rectangle(default)
            {
                CornerRadius = new SKPoint(20, 20),
                BorderWidth = 5
            };
        }

        public void OpenGameDirectory()
        {
            string path = new FileInfo(_applicationData.Path).Directory.FullName;

            Process.Start(new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true,
                Verb = "open"
            });
        }

        private void OpenSaveDirectory(SaveDataFilter filter)
        {
            if (!ulong.TryParse(_applicationData.TitleId, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out ulong titleIdNumber))
            {
                MessageDialog dialog = new MessageDialog(ParentScene,
                             "Ryujinx - Error",
                             "Ryujinx has encountered an error",
                             "UI error: The selected game did not have a valid title ID",
                             DialogButtons.OK);

                dialog.Run();

                return;
            }

            ApplicationHelper.OpenSaveDir(_applicationData.TitleName, _applicationData.ControlHolder, titleIdNumber, filter);
        }

        public override void LayoutContextMenu()
        {
            CreateContextMenu();
            
            base.LayoutContextMenu();
        }

        public override void AttachTo(Scene.Scene parent)
        {
            base.AttachTo(parent);
        }

        public void CreateContextMenu()
        {
            if (ContextMenu == null)
            {
                ContextMenu = new ContextMenu();

                Dictionary<string, string> options = new Dictionary<string, string>()
                {
                    { "play", Play},
                    { "open_game_dir", ContextActions.OpenGameDirectory},
                    { "open_user_save_dir", OpenUserSaveDirectory},
                    { "open_device_save_dir", OpenDeviceSaveDirectory},
                    { "open_bcat_save_dir", OpenBcatSaveDirectory},
                    { "manage_title_updates", ManageTitleUpdates},
                    { "manage_dlc", ManageDlc},
                    { "open_mods_directory", OpenModsDirectory}
                };

                ContextMenu.AttachTo(ParentScene);
                ContextMenu.OptionSelected += ContextMenu_OptionSelected;
                ContextMenu.SetOptions(options);
                ContextMenu.AttachedElement = this;
            }
        }

        private void ContextMenu_OptionSelected(object sender, ContextMenu.OptionSelectedEventArgs e)
        {
            Task.Run(() =>
            {
                switch (e.SelectedOption)
                {
                    case "play":
                        LoadApp();
                        break;
                    case "open_game_dir":
                        OpenGameDirectory();
                        break;
                    case "open_user_save_dir":
                        SaveDataFilter filter = new SaveDataFilter();
                        filter.SetUserId(new UserId(1, 0));
                        OpenSaveDirectory(filter);
                        break;
                    case "open_device_save_dir":
                        filter = new SaveDataFilter();
                        filter.SetSaveDataType(SaveDataType.Device);
                        OpenSaveDirectory(filter);
                        break;
                    case "open_bcat_save_dir":
                        filter = new SaveDataFilter();
                        filter.SetSaveDataType(SaveDataType.Bcat);
                        OpenSaveDirectory(filter);
                        break;
                }
            });
        }

        public override void Draw(SKCanvas canvas)
        {
            base.Draw(canvas);
            if (!DrawElement)
            {
                return;
            }

            _boundingElement.FillColor = ParentScene.Theme.BackgroundColor;
            _boundingElement.CornerRadius = new SKPoint(20, 20);
            _boundingElement.Draw(canvas);
            canvas.Save();
            canvas.ClipRoundRect(new SKRoundRect(Bounds, 20), antialias: true);

            if (_showTitle)
            {
                Title.Draw(canvas);
                //LastPlayed.Draw(canvas);
            }

            canvas.Restore();
            canvas.Save();
            canvas.ClipRoundRect(new SKRoundRect(Image.Bounds, 20), antialias: true);
            Image.Draw(canvas);
            canvas.Restore();

            _imageBorder.Draw(canvas);

            base.DrawOverlay(canvas);
        }

        public void ResetStyle()
        {
            _boundingElement.FillColor = BackgroundColor;

            if (Title.ForegroundColor != ParentScene.Theme.ForegroundColor)
            {
                Title.ForegroundColor = ParentScene.Theme.ForegroundColor;
                Title.BackgroundColor = ParentScene.Theme.BackgroundColor;

                Title.InvalidateText();
            }
            
            if (IsSelected)
            {
                _imageBorder.BorderColor = SKColors.HotPink;
            }
            else if (IsHovered)
            {
                _imageBorder.BorderColor = SKColors.LightBlue;
            }
            else
            {
                _imageBorder.BorderColor = SKColors.Transparent;

                Title.ResetAnimation();
            }
        }

        public override void Measure()
        {
            ItemSize mode = (ParentScene as MainScene).GameCardSizeMode;

            var showTitle = (ParentScene as MainScene).ShowTitleNames;

            if (showTitle != _showTitle)
            {
                IManager.Instance.InvalidateMeasure();
            }

            _showTitle = showTitle;

            switch (mode)
            {
                case ItemSize.Small:
                    Width  = GameCardWidth  * 0.75f;
                    Height = GameCardHeight * 0.75f;
                    break;
                case ItemSize.Normal:
                    Width  = GameCardWidth;
                    Height = GameCardHeight;
                    break;
                case ItemSize.Large:
                    Width  = GameCardWidth  *  1.5f;
                    Height = GameCardHeight * 1.5f;
                    break;
            }

            if (IsSelected)
            {
                _imageBorder.BorderColor = SKColors.Pink;
            }

            if (!IsHovered && !IsSelected)
            {
                Title.ResetAnimation();
            }

            float imageAreaHeight = 0.8f * (Bounds.Height - Padding.Top - Padding.Bottom);
            float imageAreaWidth = Bounds.Width - Padding.Left - Padding.Right;

            float imageSize = imageAreaHeight > imageAreaWidth ? imageAreaWidth : imageAreaHeight;

            _textBounds = SKRect.Create(new SKSize());

            Image.Size = new SKSize(imageSize, imageSize);
            float verticalAdjustment = (imageAreaHeight - imageSize) / 2;
            float horizontalAdjustment = (imageAreaWidth - imageSize) / 2;

            Image.Location = new SKPoint(Bounds.Left + Padding.Left + horizontalAdjustment, Bounds.Top + Padding.Top + verticalAdjustment);

            Image.Measure();

            _imageBorder.Bounds = SKRect.Create(new SKPoint(Image.Location.X - 4, Image.Location.Y - 4), Image.Size + new SKSize(8, 8));

            if (_showTitle)
            {
                _textBounds.Location = new SKPoint(Bounds.Left + Padding.Left, Bounds.Top + imageAreaHeight + 10);

                _textBounds.Size = new SKSize(Width - Padding.Left - Padding.Right, Bottom - Padding.Bottom - _textBounds.Top);

                Title.Bounds = _textBounds;

                Title.Measure();
            }
            else
            {
                Bounds = _imageBorder.Bounds;
            }

            _boundingElement.Bounds = Bounds;

            _imageBorder.Bounds = SKRect.Create(new SKPoint(Location.X - 4, Location.Y - 4), Size + new SKSize(8, 8));

            ResetStyle();
        }

        public override void Measure(SKRect bounds)
        {
            Bounds = bounds;
            Measure();
        }

        public override void Dispose()
        {
            Image.Dispose();
        }

        public void OnHover()
        {
            _imageBorder.BorderColor = SKColors.LightBlue;

            IsHovered = true;

            if (Title.IsAnimated)
            {
                Title.Animate();
            }
        }

        public void OnActivate()
        {
            _imageBorder.BorderColor = SKColors.Red;
            Activate?.Invoke(this, null);
            LoadApp();
        }

        public void LoadApp()
        {
            (IManager.Instance as RenderWindow)?.LoadApplication(Path);
        }

        public void OnSelect()
        {
            _imageBorder.BorderColor = SKColors.Pink;

            IsSelected = true;
        }
    }
}