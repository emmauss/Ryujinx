using SkiaSharp;

namespace Ryujinx.Ui
{
    public class ApplicationListItem
    {
        public SKBitmap        Image;
        public ApplicationData Data;
        public SKPoint         Coords;
        public SKSize          ResizedSize;
        public bool            Selected;
    }
}
