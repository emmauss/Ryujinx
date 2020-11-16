using OpenTK.Windowing.GraphicsLibraryFramework;
using Ryujinx.Skia.Ui.Skia.Scene;
using SkiaSharp;
using SkiaSharp.Elements;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ryujinx.Skia.Ui.Skia.Widget
{
    public class Slider : UIElement, IInput
    {
        private float _maximum;
        private float _range;
        private float _minimum;
        private float _value;

        public event EventHandler<IInput.InputEventArgs> Input;

        private Rectangle _boundingRect { get; set; }
        private Rectangle _sliderRect { get; set; }

        private Ellipse _sliderCircle{ get; set; }

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

        public float Value
        {
            get => _value; set
            {
                _value = value;
                IManager.Instance.InvalidateMeasure();
            }
        }

        public bool IsInputGrabbed { get; set; }

        public Slider(SKRect bounds)
        {
            Bounds = bounds;
            _boundingRect = new Rectangle(bounds);
            _sliderRect = new Rectangle(default);
            _sliderCircle = new Ellipse(default)
            {
                Width = 15,
                Height = 15
            };
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

            _boundingRect.FillColor = ParentScene.Theme.BackgroundColor;

            canvas.Save();

            var bounds = Bounds;

            bounds.Location = new SKPoint(bounds.Left, bounds.MidY - 5);
            bounds.Size = new SKSize(bounds.Width, 10);

            var boundingRect = new SKRoundRect(bounds, _boundingRect.CornerRadius.X, _boundingRect.CornerRadius.Y);

            canvas.ClipRoundRect(boundingRect, antialias: true);

            canvas.DrawRoundRect(boundingRect, paint);

            paint.Color = _sliderRect.FillColor;
            paint.Style = SKPaintStyle.Fill;

            bounds = _sliderRect.Bounds;

            bounds.Location = new SKPoint(bounds.Left, bounds.MidY - 5);

            canvas.DrawRoundRect(new SKRoundRect(bounds, _sliderRect.CornerRadius.X, _sliderRect.CornerRadius.Y), paint);
            canvas.Restore();

            _sliderCircle.BorderColor = Colors.NeonGrey;

            if (IsInputGrabbed)
            {
                _sliderCircle.FillColor = ParentScene.Theme.PrimaryColor;
            }
            else
            {
                _sliderCircle.FillColor = ParentScene.Theme.LightPrimaryColor;
            }

            paint.Color = _boundingRect.BorderColor;
            paint.Style = SKPaintStyle.Stroke;
            canvas.DrawRoundRect(boundingRect, paint);

            _sliderCircle.Draw(canvas);
        }

        public override void AttachTo(Scene.Scene parent)
        {
            base.AttachTo(parent);
            _sliderRect.FillColor = ParentScene.Theme.LightPrimaryColor;
        }

        public override void Measure()
        {
            Value = Math.Clamp(Value, Minimum, Maximum);

            float progressValue = Value - Minimum;
            float progressPercentage = progressValue / _range;

            _boundingRect.Bounds = Bounds;
            _sliderRect.Bounds = Bounds;
            _sliderRect.Width = Bounds.Width * progressPercentage;

            _sliderCircle.X = _sliderRect.Right - _sliderCircle.Width / 2;
            _sliderCircle.Y = _sliderRect.Bounds.MidY - _sliderCircle.Height / 2;

            float cornerRadius = Bounds.Height / 2;

            _boundingRect.CornerRadius = new SKPoint(cornerRadius, cornerRadius);
        }

        public override void Measure(SKRect bounds)
        {
            Bounds = bounds;
            Measure();
        }

        public void OnGrabInput()
        {
        }

        public void OnLeaveInput()
        {
        }

        public void HandleMouse(SKPoint position, InputMode inputMode)
        {
            switch (inputMode)
            {
                case InputMode.MousePress:
                case InputMode.MouseDown:
                    IsInputGrabbed = true;
                    SetValueByPosition(position);
                    break;
                case InputMode.MouseUp:
                    IsInputGrabbed = false;
                    SetValueByPosition(position);
                    break;
            }
        }

        private void SetValueByPosition(SKPoint position)
        {
            float x = position.X;

            if (x < Bounds.Left)
            {
                Value = 0;
            }
            else if(x > Bounds.Right)
            {
                Value = Maximum;
            }
            else
            {
                var length = Bounds.Width;
                var offset = x - Bounds.Left;
                Value = offset / length * _range;
            }
        }

        public void HandleKeyboard(Keys key, KeyModifiers modifiers, InputMode inputMode)
        {
        }

        public void HandleText(string text)
        {
        }
    }
}
