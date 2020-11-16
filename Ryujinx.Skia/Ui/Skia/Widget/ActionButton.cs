using SkiaSharp;
using SkiaSharp.Elements;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Ryujinx.Skia.Ui.Skia.Widget
{
    public class ActionButton : UIElement, ISelectable, IHoverable, IAction
    {
        public int CornerRadius { get; set; } = 5;
        public int BorderWidth { get; set; } = 2;
        public SKColor BorderColor { get; set; } = SKColors.White;

        public Rectangle BoundingElement { get; set; }

        public event EventHandler<EventArgs> Activate;

        public string Tag { get; set; }
        public bool IsSelected { get; set; }
        public bool IsHovered { get; set; }

        public bool Enabled { get; set; } = true;

        public string Name
        {
            get => _name; set
            {
                _name = value;
                _icon = new Icon(_name);
            }
        }

        public int IconWidth
        {
            get => _iconWidth; set
            {
                _iconWidth = value;
            }
        }

        private Icon _icon;
        private int _iconWidth = 20;
        private string _name;

        public ActionButton(string icon, SKRect bounds = default)
        {
            BoundingElement = new Rectangle(bounds);

            Bounds = bounds;

            _icon = new Icon(icon);
        }

        public override void AttachTo(Scene.Scene parent)
        {
            base.AttachTo(parent);

            BackgroundColor = parent.Theme.BackgroundColor;
            ForegroundColor = parent.Theme.ForegroundColor;
        }

        public override void Measure(SKRect bounds)
        {
            Bounds = bounds;

            _icon.Measure();

            BoundingElement.Bounds = bounds;

            SKPoint contentLocation = new SKPoint(bounds.MidX - _icon.Width / 2, bounds.MidY - _icon.Height / 2);

            _icon.Bounds = SKRect.Create(contentLocation, _icon.Bounds.Size);
        }

        public override void Draw(SKCanvas canvas)
        {
            base.Draw(canvas);

            if (!DrawElement)
            {
                return;
            }

            ResetStyle();

            BoundingElement.Bounds = Bounds;

            SKRoundRect boundingRect = new SKRoundRect(Bounds, CornerRadius);

            using SKPaint paint = new SKPaint()
            {
                Color = BoundingElement.FillColor,
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };

            canvas.DrawRoundRect(boundingRect, paint);

            paint.Color = BoundingElement.BorderColor;
            paint.Style = SKPaintStyle.Stroke;

            canvas.DrawRoundRect(boundingRect, paint);

            canvas.Save();
            canvas.ClipRoundRect(boundingRect, SKClipOperation.Intersect, true);
            try
            {
                _icon.Draw(canvas);
            }
            catch (Exception)
            {

            }
            canvas.Restore();

            IsSelected = false;
        }

        public void ResetStyle()
        {
            BackgroundColor = ParentScene.Theme.PrimaryColor;

            if (Enabled)
            {
                if (IsSelected)
                {
                    BoundingElement.FillColor = ParentScene.Theme.SelectBackgroundColor;

                    if (_icon.ForegroundColor != ParentScene.Theme.SelectForegroundColor)
                    {
                        _icon.ForegroundColor = ParentScene.Theme.SelectForegroundColor;
                    }
                }
                else if (IsHovered)
                {
                    BoundingElement.FillColor = ParentScene.Theme.HoverBackgroundColor;

                    if (_icon.ForegroundColor != ParentScene.Theme.HoverForegroundColor)
                    {
                        _icon.ForegroundColor = ParentScene.Theme.HoverForegroundColor;
                    }
                }
                else
                {
                    _icon.ForegroundColor = ParentScene.Theme.SecondaryColor;

                    if (_icon.ForegroundColor != ParentScene.Theme.SecondaryColor)
                    {
                        _icon.ForegroundColor = ParentScene.Theme.SecondaryColor;
                    }

                    BoundingElement.FillColor = BackgroundColor;
                }
            }
            else
            {
                _icon.ForegroundColor = ParentScene.Theme.SecondaryColor;

                if (_icon.ForegroundColor != ParentScene.Theme.SecondaryColor)
                {
                    _icon.ForegroundColor = ParentScene.Theme.SecondaryColor;
                }

                BoundingElement.FillColor = Colors.NeonGrey;
            }
            BoundingElement.BorderColor = Colors.NeonGrey;
        }

        public void OnActivate()
        {
            if (Enabled)
            {
                Activate?.Invoke(this, null);
            }
        }

        public void OnHover()
        {
            IsHovered = Enabled && true;
        }

        public void OnSelect()
        {
            IsSelected = Enabled && true;
        }

        public override void Measure()
        {
            _icon.Measure();

            SKSize contentSize = _icon.Size;

            if (Bounds.Size == default)
            {
                Bounds = SKRect.Create(Bounds.Left, Bounds.Top, contentSize.Width  + Padding.Left + Padding.Right, contentSize.Height + Padding.Top + Padding.Bottom);
            }

            Measure(Bounds);
        }
    }
}
