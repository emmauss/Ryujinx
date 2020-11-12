using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ryujinx.Skia.Ui.Skia.Widget
{
    public class OptionLabel : UIElement, IAction, IHoverable, ISelectable
    {
        private readonly string _label;
        private readonly OptionType OptionType;

        public string Label => _label;

        private Label _renderer;
        private Icon _icon;

        public bool IsSelected { get; set; }
        public bool IsHovered { get; set; }

        public event EventHandler<EventArgs> Activate;

        private Rectangle _boundingRect;

        public OptionLabel(string label, OptionType optionType = OptionType.Label)
        {
            _label = label;
            this.OptionType = optionType;
            _icon = new Icon(label);

            _renderer = new Label(Label);
            _boundingRect = new Rectangle(default);
            _boundingRect.BorderColor = SKColors.LightGray;
            _boundingRect.BorderWidth = 1;
            Margin = default;
            Padding = new Margin(7);
        }

        public override void Draw(SKCanvas canvas)
        {
            ResetStyle();
            base.Draw(canvas);

            if (!DrawElement)
            {
                return;
            }

            _boundingRect.FillColor = BackgroundColor;
            _boundingRect.BorderColor = SKColors.Transparent;
            _boundingRect.Bounds = Bounds;

            _boundingRect.Draw(canvas);

            if (OptionType == OptionType.Label)
            {
                _renderer.Draw(canvas);
            }
            else
            {
                _icon.Draw(canvas);
            }

            IsHovered = false;
        }

        public override void ResetState()
        {
            IsHovered = false;
            IsSelected = false;
        }

        public override void AttachTo(Scene.Scene parent)
        {
            base.AttachTo(parent);

            _renderer.AttachTo(parent);

            _icon.AttachTo(parent);

            _renderer.ForegroundColor = parent.Theme.ForegroundColor;
            _renderer.BackgroundColor = parent.Theme.BackgroundColor;
            _renderer.FontFamily = parent.Theme.FontFamily;
        }

        public override void Measure()
        {
            _renderer.Measure();

            _renderer.Location = Location + new SKPoint(Padding.Left, Padding.Top);

            _icon.Location = _renderer.Location;
            _icon.Measure();

            if (Size == default)
            {
                if (OptionType == OptionType.Label)
                {
                    Bounds = _renderer.Bounds;
                }
                else
                {
                    Bounds = _icon.Bounds;
                }

                Size += new SKSize(Padding.Left + Padding.Right, Padding.Top + Padding.Bottom);
            }
        }

        public override void Measure(SKRect bounds)
        {
            Bounds = bounds;

            _renderer.Measure(bounds);
            _icon.Measure(bounds);

            Measure();
        }

        public void ResetStyle()
        {
            _renderer.BackgroundColor = ParentScene.Theme.BackgroundColor;

            
            if (IsHovered)
            {
                _renderer.ForegroundColor = ParentScene.Theme.ContextHoverForegroundColor;
                _renderer.BackgroundColor = ParentScene.Theme.ContextHoverBackgroundColor;

                _renderer.InvalidateText();
            }
            else if (IsSelected)
            {
                _renderer.ForegroundColor = ParentScene.Theme.ContextSelectForegroundColor;
                _renderer.BackgroundColor = ParentScene.Theme.ContextSelectBackgroundColor;

                _renderer.InvalidateText();
            }
            else
            {
                _renderer.ForegroundColor = ParentScene.Theme.ForegroundColor;

                _renderer.ForegroundColor = ParentScene.Theme.ForegroundColor;
                _renderer.BackgroundColor = ParentScene.Theme.BackgroundColor;

                _renderer.InvalidateText();
            }

            ForegroundColor = _renderer.ForegroundColor;
            BackgroundColor = _renderer.BackgroundColor;

            _icon.ForegroundColor = ForegroundColor;
            _icon.BackgroundColor = BackgroundColor;
        }

        public void OnActivate()
        {
            Activate?.Invoke(this, null);
        }

        public void OnHover()
        {
            IsHovered = true;
            IsSelected = false;
        }

        public void OnSelect()
        {
            IsSelected = true;
            IsHovered = false;
        }
    }
}
