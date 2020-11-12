using SkiaSharp;
using SkiaSharp.Elements;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ryujinx.Skia.Ui.Skia.Widget
{
    public class Scrollbar : UIElement, ISelectable, IHoverable
    {
        public float Value
        {
            get { return _value; }
            set
            {
                _value = value;
            }
        }

        public float OffsetValue { get; private set; }

        public bool ScrollActive => ContentSize > PageSize;

        public float PageSize { get; set; }

        public float ContentSize { get; set; }

        public int ScrollbarWidth { get; set; } = 10;

        public Orientation Orientation { get; set; } = Orientation.Vertical;

        public float Length { get; set; }

        public IScrollable Layout { get; set; }

        private readonly Rectangle _bar;
        private readonly Rectangle _bounds;
        private float _value;

        public event EventHandler<EventArgs> Activate;


        public bool IsSelected { get; set; }
        public bool IsHovered { get; set; }

        public Scrollbar(SKRect bounds = default)
        {
            Bounds = bounds;
            _bar = new Rectangle(bounds);
            _bounds = new Rectangle(bounds);
            _bar.BorderWidth = 2;
            BackgroundColor = SKColors.DimGray;
        }

        public override void AttachTo(Scene.Scene parent)
        {
            base.AttachTo(parent);
            _bar.BorderColor = ParentScene.Theme.ForegroundColor;
            _bounds.BorderColor = ParentScene.Theme.ForegroundColor;
        }

        public override void Draw(SKCanvas canvas)
        {
            base.Draw(canvas);

            if (!DrawElement)
            {
                return;
            }

            _bounds.CornerRadius = new SKPoint(10,10);
            _bounds.Draw(canvas);

            using SKPaint paint = new SKPaint()
            {
                Color = _bar.FillColor,
                Style = SKPaintStyle.Fill
            };

            SKRect normalBounds = _bar.Bounds;

            if (Orientation == Orientation.Vertical)
            {
                normalBounds.Size = new SKSize(_bar.Width, _bar.Height < 50 ? 50 : _bar.Height);

                if (normalBounds.Bottom > Bounds.Bottom)
                {
                    normalBounds.Location = new SKPoint(_bar.Left, Bounds.Bottom - normalBounds.Height);
                }
            }
            else
            {
                normalBounds.Size = new SKSize(_bar.Width < 50 ? 50 : _bar.Width, _bar.Height);

                if (normalBounds.Right > Bounds.Right)
                {
                    normalBounds.Location = new SKPoint(Bounds.Right - normalBounds.Width, _bar.Top);
                }
            }

            SKRoundRect barBounds = new SKRoundRect(normalBounds, 10, 10);

            canvas.DrawRoundRect(barBounds, paint);

            paint.Color = _bar.BorderColor;
            paint.Style = SKPaintStyle.Stroke;
            paint.StrokeWidth = _bar.BorderWidth;

            canvas.DrawRoundRect(barBounds, paint);
        }

        public void ClampValue()
        {
            float barLength = PageSize / ContentSize * Length;

            if (barLength < Length)
            {
                float halfBar = barLength / 2;

                _value = Math.Clamp(_value, halfBar, Length - halfBar);
            }
        }

        public override void Measure()
        {
            float barLength = PageSize / ContentSize * Length;

            if(!ScrollActive)
            {
                OffsetValue = 0;
                return;
            }

            float halfBar = barLength / 2;

            float barOffset = Math.Clamp(_value - halfBar, 0, Length - barLength);

            SKSize size = default;

            SKRect barBounds = default;
            switch (Orientation)
            {
                case Orientation.Vertical:
                    barBounds = SKRect.Create(new SKPoint(Bounds.Left, Bounds.Top + barOffset), new SKSize(Bounds.Width, barLength));
                    size = new SKSize(ScrollbarWidth, Length);
                    break;
                case Orientation.Horizontal:
                    barBounds = SKRect.Create(new SKPoint(Bounds.Left + barOffset, Bounds.Top), new SKSize(barLength, Bounds.Height));
                    size = new SKSize(Length, ScrollbarWidth);
                    break;
            }

            _bar.Bounds = barBounds;
            Bounds = SKRect.Create(Bounds.Location, size);
            _bounds.Bounds = Bounds;
            _bar.FillColor = BackgroundColor;

            OffsetValue = (barOffset / Length * ContentSize);
        }

        public override void Measure(SKRect bounds)
        {
            Bounds = SKRect.Create(Bounds.Location, default);
            Measure();
        }

        public void OnHover()
        {
            _bar.FillColor = ParentScene.Theme.HoverBackgroundColor;
        }

        public void OnSelect()
        {
            _bar.FillColor = ParentScene.Theme.SelectBackgroundColor;
        }
    }
}
