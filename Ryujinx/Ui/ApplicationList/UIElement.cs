using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ryujinx.Ui
{
    class UIElement
    {
        public SKRect Rect { get; set; }

        public UIAction Action { get; set; }

        public bool IsSelected { get; set; }

        public bool IsOverlayElement { get; set; }
    }
}
