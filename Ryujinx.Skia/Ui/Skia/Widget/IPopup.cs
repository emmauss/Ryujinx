using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ryujinx.Skia.Ui.Skia.Widget
{
    public interface IPopup : IInput
    {
        bool DismissOnFocusOut { get; set; }

        bool IsDismissed { get; set; }

        void Dismiss();

        void Show(SKPoint location);
    }
}
