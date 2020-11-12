using System;
using SkiaSharp;

namespace Ryujinx.Skia.Ui.Skia.Widget
{
    public class ListItem :  UIElement, ISelectable, IHoverable, IAction
    {
        private Label _label;

        private Rectangle _boundingElement;

        public event EventHandler<EventArgs> Activate;

        public object Value{ get; set; }
        public bool IsSelected { get; set; }
        public bool IsHovered { get; set; }

        public ListItem(object value)
        {
            Value = value;

            _label = new Label(value.ToString());

            _boundingElement = new Rectangle(default);

            HorizontalAlignment = LayoutOptions.Stretch;
        }

        public override void AttachTo(Scene.Scene parent) 
        {
            base.AttachTo(parent);

            ForegroundColor = ParentScene.Theme.ForegroundColor;

            BackgroundColor = ParentScene.Theme.BackgroundColor;

            _label.AttachTo(ParentScene);
        }

        public override void Measure()
        {
            _label.Text = Value.ToString();

            _label.Measure(Bounds);

            Bounds = _label.Bounds;

            _boundingElement.Bounds = Bounds;

            ResetStyle();
        }

        public override void Draw(SKCanvas canvas)
        {
            base.Draw(canvas);

            if (DrawElement)
            {
                _boundingElement.Draw(canvas);

                _label.Draw(canvas);

                using var paint = new SKPaint();

                paint.Color = SKColors.Red;
                paint.Style = SKPaintStyle.Stroke;

                canvas.DrawRect(Bounds, paint);
            }
        }

        public override void Measure(SKRect bounds)
        {
            Bounds = bounds;

            Measure();
        }

        public void ResetStyle()
        {
            _boundingElement.FillColor = BackgroundColor;

            if (IsSelected)
            {
                _boundingElement.FillColor = ParentScene.Theme.SelectBackgroundColor;
                _label.BackgroundColor = ParentScene.Theme.SelectBackgroundColor;
                _label.ForegroundColor = ParentScene.Theme.SelectForegroundColor;
            }
            else if (IsHovered)
            {
                _boundingElement.FillColor = ParentScene.Theme.HoverBackgroundColor;
                _label.BackgroundColor = ParentScene.Theme.HoverBackgroundColor;
                _label.ForegroundColor = ParentScene.Theme.HoverForegroundColor;
            }
            else
            {
                _boundingElement.FillColor = BackgroundColor;
                _label.BackgroundColor = BackgroundColor;
                _label.ForegroundColor = ForegroundColor;
            }
        }

        public void OnSelect()
        {
            IsSelected = true;
        }

        public void OnHover()
        {
            IsHovered = true;
        }

        public void RemoveSelection()
        {
            IsSelected = false;
        }

        public void OnActivate()
        {
            Activate?.Invoke(this, null);
        }
    }
}