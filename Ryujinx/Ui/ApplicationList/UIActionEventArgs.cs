using System;
using System.Collections.Generic;
using System.Text;

namespace Ryujinx.Ui
{
    public class UIActionEventArgs
    {
        public UIAction UIAction { get; set; }
        public ApplicationListItem Item { get; set; }
    }
}
