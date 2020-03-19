using SkiaSharp;
using System;

namespace Ryujinx.Ui
{
    public class ApplicationListItem : IDisposable
    {
        public SKBitmap        Image;
        public SKBitmap        ResizedImage;
        public ApplicationData Data;
        public SKPoint         Coords;
        public SKSize          ResizedSize;
        public bool            Selected;

        public void Dispose()
        {
            Image?.Dispose();
            ResizedImage?.Dispose();
        }
    }
}
