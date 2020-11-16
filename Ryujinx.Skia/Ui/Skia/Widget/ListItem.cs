using System;
using SkiaSharp;

namespace Ryujinx.Skia.Ui.Skia.Widget
{
    public class ListItem :  UIElement, ISelectable, IHoverable, IAction
    {
        public event EventHandler Selected;
        private Label _label;

        private Rectangle _boundingElement;

        public event EventHandler<EventArgs> Activate;

        public object Value{ get; set; }
        public bool IsSelected { get; set; }
        public bool IsHovered { get; set; }

        public bool IsActive{ get; set; }

        public ItemSize ItemSize { get; set; } = ItemSize.Small;

        public ListItem(object value)
        {
            Value = value;

            _label = new Label(value.ToString());

            _boundingElement = new Rectangle(default);

            _boundingElement.BorderColor = SKColor.Parse("#f0f0f0");

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
            switch(ItemSize)
            {
                case ItemSize.Small:
                    _label.FontSize = 16;
                    break;
                case ItemSize.Normal:
                    _label.FontSize = 24;
                    break;
                case ItemSize.Large:
                    _label.FontSize = 30;
                    break;
            }

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
            }
        }

        public override void Measure(SKRect bounds)
        {
            Bounds = bounds;

            Measure();
        }

        public void ResetStyle()
        {
            ForegroundColor = ParentScene.Theme.ForegroundColor;
            BackgroundColor = ParentScene.Theme.BackgroundColor;

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

            Selected?.Invoke(this, null);
        }

        public void OnHover()
        {
            IsHovered = true;
        }

        public void RemoveSelection()
        {
            IsSelected = false;
            IsActive = false;
        }

        public void OnActivate()
        {
            lock (this)
            {
                if (IsActive)
                {
                    IsActive = false;

                    Activate?.Invoke(this, null);
                }
                else
                {
                    IsActive = true;
                }
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            _label.Dispose();
        }
    }
}