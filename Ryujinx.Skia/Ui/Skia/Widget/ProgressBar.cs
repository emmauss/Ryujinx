using Ryujinx.Skia.Ui.Skia.Scene;
using SkiaSharp;
using SkiaSharp.Elements;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ryujinx.Skia.Ui.Skia.Widget
{
    public class ProgressBar : UIElement
    {
        private float _maximum;
        private float _range;
        private float _minimum;
        private float progress;

        private Rectangle _boundingRect { get; set; }
        private Rectangle _progressRect { get; set; }

        public SKColor BorderColor { get; set; } = SKColors.Gray;

        public float Minimum
        {
            get => _minimum; set
            {
                _minimum = MathF.Min(value, Maximum);
                _range = Maximum - _minimum;
            }
        }

        public float Maximum
        {
            get => _maximum; set
            {
                _maximum = MathF.Max(value, Minimum);
                _range = _maximum - Minimum;
            }
        }

        public float Progress
        {
            get => progress; set
            {
                progress = value;
                IManager.Instance.InvalidateMeasure();
            }
        }

        public ProgressBar(SKRect bounds)
        {
            Bounds = bounds;
            _boundingRect = new Rectangle(bounds);
            _progressRect = new Rectangle(default);
        }

        public override void Draw(SKCanvas canvas)
        {
            base.Draw(canvas);

            if (!DrawElement)
            {
                return;
            }
            using SKPaint paint = new SKPaint()
            {
                Color = _boundingRect.FillColor,
                Style = SKPaintStyle.Fill
            };
            paint.StrokeWidth = 1;

            canvas.Save();
            var boundingRect = new SKRoundRect(Bounds, _boundingRect.CornerRadius.X, _boundingRect.CornerRadius.Y);
            canvas.ClipRoundRect(boundingRect, antialias: true);

            canvas.DrawRoundRect(boundingRect, paint);

            paint.Color = _progressRect.FillColor;
            paint.Style = SKPaintStyle.Fill;

            canvas.DrawRoundRect(new SKRoundRect(_progressRect.Bounds, _progressRect.CornerRadius.X, _progressRect.CornerRadius.Y), paint);
            canvas.Restore();


            paint.Color = _boundingRect.BorderColor;
            paint.Style = SKPaintStyle.Stroke;
            canvas.DrawRoundRect(boundingRect, paint);
        }

        public override void AttachTo(Scene.Scene parent)
        {
            base.AttachTo(parent);
            _progressRect.FillColor = ParentScene.Theme.ForegroundColor;
        }

        public override void FadeOut()
        {
            Animator?.Stop();
            Animator = new Animation();
            Animator.With(255, 0, 1000, SetBarAlpha);
            Animator.Play();

            IManager.Instance?.InvalidateMeasure();
        }

        public void SetBarAlpha(double alpha)
        {
            _boundingRect.BorderColor = _boundingRect.BorderColor.WithAlpha((byte)alpha);
            _boundingRect.FillColor   = _boundingRect.FillColor.WithAlpha((byte)alpha);
            _progressRect.BorderColor = _progressRect.BorderColor.WithAlpha((byte)alpha);
            _progressRect.FillColor = _progressRect.FillColor.WithAlpha((byte)alpha);
        }

        public override void Measure()
        {
            Progress = Math.Clamp(Progress, Minimum, Maximum);

            float progressValue = Progress - Minimum;
            float progressPercentage = progressValue / _range;

            _boundingRect.Bounds = Bounds;
            _progressRect.Bounds = Bounds;
            _progressRect.Width = Bounds.Width * progressPercentage;

            float cornerRadius = Bounds.Height / 2;

            _boundingRect.CornerRadius = new SKPoint(cornerRadius, cornerRadius);
        }

        public override void Measure(SKRect bounds)
        {
            Bounds = bounds;
            Measure();
        }
    }
}
