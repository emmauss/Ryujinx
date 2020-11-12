using System;
using System.Collections.Generic;
using System.Text;

namespace Ryujinx.Skia.Ui.Skia.Widget
{
    [Flags]
    public enum DialogButtons
    {
        None,
        OK = 1,
        Cancel = 2,
        Yes = 4,
        No = 8,
    }
}
