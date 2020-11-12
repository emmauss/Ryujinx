using System;
using System.IO;
using System.Reflection;
using Ryujinx.Skia.Ui.Skia.Scene;
using SkiaSharp;
using Topten.RichTextKit;

namespace Ryujinx.Skia.Ui.Skia.Widget
{
    public class Icon : UIElement
    {
        public static SKTypeface IconTypeface { get; set; }
        private string _name;

        private RichString _renderer;

        private bool _recreateRenderer = true;
        private int _fontSize = 20;

        public int CornerRadius { get; set; }

        public string Name
        {
            get => _name; set
            {
                _name = value;

                _recreateRenderer = true;

                Bounds = SKRect.Create(Bounds.Location, default);

                IManager.Instance.InvalidateMeasure();
            }
        }

        public override SKColor ForegroundColor
        {
            get => base.ForegroundColor; set
            {
                base.ForegroundColor = value;

                _recreateRenderer = true;
            }
        }

        public override SKColor BackgroundColor
        {
            get => base.BackgroundColor; set
            {
                base.BackgroundColor = value;

                _recreateRenderer = true;
            }
        }

        public SKTypeface Typeface { get; private set; } = SKTypeface.Default;

        public int FontSize
        {
            get => _fontSize; set
            {
                _fontSize = value;

                _recreateRenderer = true;

                IManager.Instance.InvalidateMeasure();
            }
        }

        public Icon(string name)
        {
            Name = name;
        }

        public override void AttachTo(Scene.Scene parent)
        {
            base.AttachTo(parent);

            ForegroundColor = parent.Theme.ForegroundColor;
        }


        public override void Draw(SKCanvas canvas)
        {
            base.Draw(canvas);

            if (!DrawElement)
            {
                return;
            }

            TextPaintOptions options = new TextPaintOptions
            {
                IsAntialias = true
            };

            canvas.DrawRoundRect(new SKRoundRect(Bounds, CornerRadius), new SKPaint() { Color = BackgroundColor, Style = SKPaintStyle.Fill });

            _renderer?.Paint(canvas, Bounds.Location, options);
        }

        public override void Measure()
        {

            if (_recreateRenderer)
            {
                _recreateRenderer = false;

                _renderer = new RichString();
                _renderer.TextColor(ForegroundColor);
                _renderer.Alignment(TextAlignment.Center);
                _renderer.FontSize(FontSize);
                _renderer.FontWeight((int)SKFontStyleWeight.Normal);
                _renderer.FontItalic(false);
                _renderer.FontFamily("IonIcon");
                _renderer.MarginLeft(Padding.Left);
                _renderer.MarginTop(Padding.Top);
                _renderer.MarginRight(Padding.Right);
                _renderer.MarginBottom(Padding.Bottom);
                _renderer.Add(FontMapper.GetGlyphUnicodeCodepoint(Name));
            }

            try
            {
                var width = _renderer.MeasuredWidth + Padding.Left + Padding.Right;
                var height = _renderer.MeasuredHeight;

                Size = new SKSize(width, height);
            }
            catch (Exception)
            {

            }
        }

        public override void Measure(SKRect bounds)
        {
            Measure();
        }
    }
}