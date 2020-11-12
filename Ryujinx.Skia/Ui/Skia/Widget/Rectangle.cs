using SkiaSharp;
using SkiaSharp.Elements;

namespace Ryujinx.Skia.Ui.Skia.Widget
{
    public class Rectangle : Element
    {
        #region Constructors

        public Rectangle(SKRect bounds) : this()
        {
            Bounds = bounds;
        }

        private Rectangle()
        {
            _fillColor = SKColors.Transparent;
            _borderColor = SKColors.Black;
            _borderWidth = 1;
            _drawBorder = true;
        }

        #endregion Constructors

        #region Properties

        private bool _drawBorder;
        private bool _drawFill;

        private SKColor _fillColor;
        public SKColor FillColor
        {
            get => _fillColor;
            set
            {
                _fillColor = value;
                _drawFill = value != SKColors.Transparent;
                Invalidate();
            }
        }

        private SKColor _borderColor;
        public SKColor BorderColor
        {
            get => _borderColor;
            set
            {
                _borderColor = value;
                _drawBorder = _borderWidth > 0 && _borderColor != SKColors.Transparent;
                Invalidate();
            }
        }

        private float _borderWidth;
        public float BorderWidth
        {
            get => _borderWidth;
            set
            {
                _borderWidth = value;
                _drawBorder = _borderWidth > 0 && _borderColor != SKColors.Transparent;
                Invalidate();
            }
        }

        private SKPoint _cornerRadius;
        public SKPoint CornerRadius
        {
            get => _cornerRadius;
            set
            {
                _cornerRadius = value;
                Invalidate();
            }
        }

        #endregion Properties

        #region Public methods

        public override void Draw(SKCanvas canvas)
        {
            if (_drawFill || _drawBorder)
            {
                using (var paint = new SKPaint { IsAntialias = true })
                {
                    if (_drawFill)
                    {
                        paint.Color = FillColor;
                        canvas.DrawRoundRect(Bounds, CornerRadius.X, CornerRadius.Y, paint);
                    }

                    if (_drawBorder)
                    {
                        paint.Style = SKPaintStyle.Stroke;
                        paint.Color = BorderColor;
                        paint.StrokeWidth = BorderWidth;
                        canvas.DrawRoundRect(Bounds, CornerRadius.X, CornerRadius.Y, paint);
                    }
                }
            }
        }

        #endregion Public methods
    }
}