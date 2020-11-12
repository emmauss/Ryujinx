using System;
using System.Collections.Generic;
using System.Text;

namespace Ryujinx.Skia.Ui.Skia.Widget
{
    interface ISelectable
    {
        bool IsSelected { get; set; }

        void OnSelect();
    }
}
