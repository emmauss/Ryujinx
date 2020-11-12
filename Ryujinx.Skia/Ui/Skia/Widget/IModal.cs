using SkiaSharp;
using SkiaSharp.Elements;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ryujinx.Skia.Ui.Skia.Widget
{
    public interface IModal : IUICollection
    {
        void DrawContent(SKCanvas canvas);
        void Draw(SKCanvas canvas);
    }
}
