using Ryujinx.Skia.Ui.Skia.Scene;
using SkiaSharp;
using SkiaSharp.Elements;
using SkiaSharp.Elements.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Ryujinx.Skia.Ui.Skia.Widget
{
    public abstract class Dialog : UIElement, IModal
    {
        protected int DialogWidth = 300;
        protected int DialogHeight = 300;
        protected bool FixedHeight = false;

        public string Title { get; set; }
        public DialogButtons Buttons { get; set; }
        public string AcceptButtonText { get; set; }
        public string DeclineButtonText { get; set; }
        public string CancelButtonText { get; set; }

        private ManualResetEvent _resetEvent;
        private Box _buttonBox;
        private Label _title;

        private Rectangle _titleRectangle;
        private Rectangle _buttonRectangle;

        private Rectangle _boundingElement;

        private Line _titleLine;
        private SKRect _contentSize;

        public string DialogResult { get; private set; }

        public ElementsController Controller{ get; }

        public ElementsCollection Elements => Controller.Elements;

        public Dialog(Scene.Scene parent,
                      string title,
                      DialogButtons buttons,
                      string acceptButtonText = "",
                      string declineButtonText = "",
                      string cancelButtonText = "")
        {
            Controller = new ElementsController();
            ParentScene = parent;
            Title = title;
            Buttons = buttons;
            AcceptButtonText = acceptButtonText;
            DeclineButtonText = declineButtonText;
            CancelButtonText = cancelButtonText;

            _resetEvent = new ManualResetEvent(false);
        }

        private void Add(UIElement element)
        {
            Elements.Add(element);

            element.AttachTo(ParentScene);
        }

        private void CreateDialog()
        {
            foreach(var element in Elements){
                (element as UIElement).Dispose();
            }

            Padding = new Margin(10);

            _title = new Label(Title);

            _title.FontSize = 20;
            _title.FontStyle = SKFontStyle.Bold;
            _title.Margin = new Margin(5);

            _buttonBox = new Box(default)
            {
                ScrollEnabled = false,
                Orientation = Orientation.Horizontal,
                LayoutOptions = LayoutOptions.End
            };

            _titleLine = new Line(default, default);
            _titleLine.Width = DialogWidth;
            _titleLine.Color = SKColors.Gray;

            _titleRectangle   = new Rectangle(SKRect.Create(new SKSize(DialogWidth, 50))) { BorderColor = SKColors.Transparent };
            _buttonRectangle  = new Rectangle(SKRect.Create(new SKSize(DialogWidth, 60))) { BorderColor = SKColors.Transparent };

            _boundingElement = new Rectangle(default)
            {
                FillColor = ParentScene.Theme.BackgroundColor,
                BorderColor = SKColors.LightGray,
                BorderWidth = 1,
                CornerRadius = new SKPoint(10, 10)
            };

            Button button = null;

            Elements.Add(_titleRectangle);
            
            Elements.Add(_buttonRectangle);

            _title.ForegroundColor = ParentScene.Theme.SecondaryColor;

            Add(_title);
            Add(_buttonBox);

            if (Buttons != DialogButtons.None)
            {
                if (Buttons.HasFlag(DialogButtons.OK))
                {
                    button = new Button(string.IsNullOrWhiteSpace(AcceptButtonText) ? "OK" : AcceptButtonText) { Tag = "OK" };

                    _buttonBox.AddElement(button);
                    button.Activate += Button_Activate;

                    button.BackgroundColor = SKColors.Red;
                }

                if (Buttons.HasFlag(DialogButtons.Cancel))
                {
                    button = new Button(string.IsNullOrWhiteSpace(CancelButtonText) ? "Cancel" : CancelButtonText) { Tag = "Cancel" };

                    _buttonBox.AddElement(button);

                    button.Activate += Button_Activate;
                    button.BackgroundColor = SKColors.Red;
                }

                if (Buttons.HasFlag(DialogButtons.Yes))
                {
                    button = new Button(string.IsNullOrWhiteSpace(AcceptButtonText) ? "Yes" : AcceptButtonText) { Tag = "Yes" };

                    _buttonBox.AddElement(button);

                    button.Activate += Button_Activate;
                    button.BackgroundColor = SKColors.Red;
                }

                if (Buttons.HasFlag(DialogButtons.No))
                {

                    button = new Button(string.IsNullOrWhiteSpace(DeclineButtonText) ? "No" : AcceptButtonText) { Tag = "No" };

                    _buttonBox.AddElement(button);
                    button.Activate += Button_Activate;
                    button.BackgroundColor = SKColors.Red;
                }
            }
        }

        private void Button_Activate(object sender, EventArgs e)
        {
            DialogResult = (sender as Button).Tag;

            _resetEvent.Set();
        }

        public void Dismiss()
        {
            _resetEvent.Set();

            this.FadeOut();

            ParentScene.DismissModal();

            Dispose();
        }

        public void DrawController(SKCanvas canvas)
        {
            for (int i = 0; i < Controller.Elements.Count; i++)
            {
                Element element = Controller.Elements[i];

                element.Draw(canvas);
            }
        }

        public abstract void DrawContent(SKCanvas canvas);

        public override void Draw(SKCanvas canvas)
        {
            _titleRectangle.FillColor = ParentScene.Theme.PrimaryColor;
            _boundingElement.FillColor = ParentScene.Theme.ModalBackgroundColor;

            canvas.Save();
            canvas.ClipRoundRect(new SKRoundRect(_boundingElement.Bounds, _boundingElement.CornerRadius.X, _boundingElement.CornerRadius.Y),
                                antialias: true);
            _boundingElement.Draw(canvas);
            _titleLine.Draw(canvas);
            DrawController(canvas);

            DrawContent(canvas);

            canvas.Restore();
        }

        public override void Measure()
        {
            SKRect windowBounds = IManager.Instance.Bounds;

            _buttonBox.Measure();

            _contentSize = MeasureContent(default);

            float totalHeight = FixedHeight ? DialogHeight : _buttonBox.Height + _titleRectangle.Height + _contentSize.Height + 20;

            SKRect bounds = SKRect.Create(windowBounds.MidX - DialogWidth / 2, windowBounds.MidY - totalHeight / 2, DialogWidth, totalHeight);

            _titleRectangle.Location = bounds.Location;
            _contentSize.Location = new SKPoint(bounds.Left + Padding.Left, _titleRectangle.Bottom + 20);
            _titleLine.Location = new SKPoint(Bounds.Left, _titleRectangle.Bottom);
            _buttonRectangle.Location = new SKPoint(bounds.Left, _contentSize.Bottom + 1);
            _buttonBox.Location = new SKPoint(bounds.Right - _buttonBox.Width, bounds.Bottom - _buttonBox.Height);
            _contentSize.Size = new SKSize(bounds.Width - Padding.Left - Padding.Right, _contentSize.Height);

            if (FixedHeight)
            {
                float contentHeight = DialogHeight - _buttonBox.Height - _titleRectangle.Height - 20;

                MeasureContent(SKRect.Create(_contentSize.Location, new SKSize(_contentSize.Width, contentHeight)));
            }
            else
            {
                _contentSize = MeasureContent(_contentSize);
            }

            _buttonBox.Measure(_buttonBox.Bounds);

            _title.Measure();
            _title.Location = new SKPoint(_titleRectangle.Left + Padding.Left, _titleRectangle.Bounds.MidY - _title.Height / 2);

            Bounds = bounds;

            _boundingElement.Bounds = bounds;
        }

        public abstract SKRect MeasureContent(SKRect bounds);

        public abstract Element GetElementInContent(SKPoint point);

        public override void Measure(SKRect bounds)
        {
            Bounds = bounds;
            Measure();
        }

        public void Run()
        {
            CreateDialog();

            ParentScene.ShowModal(this);

            _resetEvent.WaitOne();

            Dismiss();

        }

        public override void Dispose()
        {
            base.Dispose();

            _buttonBox.Dispose();
        }

        public override bool IsPointInside(SKPoint point)
        {
            return Bounds.Contains(point);
        }

        public Element GetElementAtPosition(SKPoint position)
        {
            if (_contentSize.Contains(position))
            {
                return GetElementInContent(position);
            }
            if (_title.IsPointInside(position))
            {
                return _title;
            }
            return _buttonBox.GetElementAtPosition(position);
        }
    }
}
