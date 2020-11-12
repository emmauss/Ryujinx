using System;
using System.Collections.Generic;
using System.Text;

namespace Ryujinx.Skia.Ui.Skia.Widget
{
    interface IHoverable
    {
        bool IsHovered{ get; set; }
        
        void OnHover();
    }
}
