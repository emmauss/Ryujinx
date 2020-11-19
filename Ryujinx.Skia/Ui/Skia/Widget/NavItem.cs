using System;
using SkiaSharp;

namespace Ryujinx.Skia.Ui.Skia.Widget
{
    public class NavItem : UIElement, IAction, IHoverable, ISelectable
    {
        public bool IsHovered { get; set; }
        public string Title { get; set; }
        public string Icon { get; set; }
        public event EventHandler<EventArgs> Activate;

        private Label _label;
        private Icon _icon;

        private Rectangle _boundingElement;
        public float Width { get; set; }
        public bool IsSelected { get; set; }
        public string Tag { get; }

        public NavItem(string title, string icon, string tag, float width)
        {
            this.Tag = tag;
            this.Width = width;
            Title = title;
            Icon = icon;

            _label = new Label(title, 18);

            _label.Measure();

            _icon = new Icon(icon);
            _icon.Measure();

            _boundingElement = new Rectangle(default)
            {
                CornerRadius = new SKPoint(20, 20)
            };
        }

        public override void ResetState()
        {
            IsHovered = false;
            IsSelected = false;
        }

        public void ResetStyle()
        {
            if (IsSelected)
            {
                ForegroundColor = ParentScene.Theme.SecondaryColor;
                BackgroundColor = ParentScene.Theme.PrimaryColor;
            }
            else if (IsHovered)
            {
                ForegroundColor = ParentScene.Theme.HoverForegroundColor;
                BackgroundColor = ParentScene.Theme.HoverBackgroundColor;
            }
            else
            {
                ForegroundColor = ParentScene.Theme.ForegroundColor;
                BackgroundColor = ParentScene.Theme.BackgroundColor;
            }

            _icon.ForegroundColor = ForegroundColor;
            _label.ForegroundColor = ForegroundColor;

            _label.InvalidateText();

            _boundingElement.FillColor = BackgroundColor;
        }

        public override void Measure()
        {
            _label.Measure();

            _icon.Location = new SKPoint(Bounds.Left + Padding.Left, Bounds.Top + Padding.Top);
            _icon.Measure();

            _label.Location = new SKPoint(_icon.Right + 20, _icon.Top);
            _label.Measure();

            float verticalPadding = Padding.Top + Padding.Bottom;

            var bounds = Bounds;

            bounds.Size = new SKSize(Width, Math.Max(_label.Height + verticalPadding, _icon.Height + verticalPadding));

            Bounds = bounds;

            _boundingElement.Bounds = bounds;
        }

        public override void AttachTo(Scene.Scene parent)
        {
            base.AttachTo(parent);

            _label.AttachTo(parent);
            _icon.AttachTo(parent);
        }

        public override void Measure(SKRect bounds)
        {
            Bounds = bounds;

            Measure();
        }

        public void OnActivate()
        {
            Activate?.Invoke(this, null);
        }

        public void OnHover()
        {
            IsHovered = true;
        }

        public override void Draw(SKCanvas canvas)
        {
            ResetStyle();

            base.Draw(canvas);

            _boundingElement.Draw(canvas);
            _label.Draw(canvas);
            _icon.Draw(canvas);
        }

        public void OnSelect()
        {
            IsSelected = true;
        }
    }
}