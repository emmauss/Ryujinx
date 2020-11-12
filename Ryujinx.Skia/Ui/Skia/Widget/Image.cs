using SkiaSharp;
using SkiaSharp.Elements;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Ryujinx.Skia.Ui.Skia.Widget
{
    public class Image : UIElement
    {
        private SKImage _drawImage;

        private SKBitmap _bitmap;

        public Image()
        {
            _drawImage = SKImage.Create(new SKImageInfo());
        }

        public Image(SKBitmap bitmap)
        {
            this._bitmap = bitmap;

            _drawImage = SKImage.FromBitmap(bitmap);
        }

        public Image(Stream stream)
        {
            _drawImage = SKImage.FromEncodedData(stream);
        }
        public Image(byte[] data)
        {
            using MemoryStream stream = new MemoryStream(data);
            
            _drawImage = SKImage.FromEncodedData(stream);
        }

        public void Load(SKBitmap bitmap)
        {
            _drawImage?.Dispose();
            _bitmap?.Dispose();

            _drawImage = SKImage.FromBitmap(bitmap);
        }

        public void Load(Stream stream)
        {
            _drawImage?.Dispose();
            _bitmap?.Dispose();

            _drawImage = SKImage.FromBitmap(SKBitmap.Decode(stream));
        }

        public void Load(byte[] data)
        {
            _drawImage?.Dispose();
            _bitmap?.Dispose();

            using MemoryStream stream = new MemoryStream(data);

            _drawImage = SKImage.FromEncodedData(stream);
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
                FilterQuality = SKFilterQuality.Medium,
            };

           canvas.DrawImage(_drawImage, Bounds, paint);
        }

        public override void Measure()
        {
            if(Bounds.Size == default)
            {
                Size = new SKSize(_drawImage.Width, _drawImage.Height);
            }
        }

        public override void Measure(SKRect bounds)
        {
            Bounds = bounds;

            Measure();
        }

        public override void Dispose()
        {
            _drawImage?.Dispose();
            _bitmap?.Dispose();
        }
    }
}
