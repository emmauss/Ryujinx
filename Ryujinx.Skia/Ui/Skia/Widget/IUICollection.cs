using SkiaSharp;
using SkiaSharp.Elements;
using SkiaSharp.Elements.Collections;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ryujinx.Skia.Ui.Skia.Widget
{
    public interface IUICollection
    {
        public ElementsController Controller { get; }

        public ElementsCollection Elements { get; }

        public Element GetElementAtPosition(SKPoint position);
    }
}
