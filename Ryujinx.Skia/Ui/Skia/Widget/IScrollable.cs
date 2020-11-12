using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ryujinx.Skia.Ui.Skia.Widget
{
    public interface IScrollable
    {
        Scrollbar Scrollbar { get; set; }

        void ScrollTo(SKPoint position);

        bool ScrollEnabled { get; set; }

        bool IsScrolling { get; set; }

        void UpdateScrollbars();
    }
}
