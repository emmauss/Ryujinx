using System;
using System.Collections.Generic;
using System.Text;

namespace Ryujinx.Skia.Ui.Skia.Widget
{
    interface IAction
    {
        event EventHandler<EventArgs> Activate;

        void OnActivate();
    }
}
