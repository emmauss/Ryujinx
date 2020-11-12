using System;
using System.Collections.Generic;
using System.Text;

namespace Ryujinx.Skia.Ui.Skia.Widget
{
    public struct Margin
    {
        public int Top { get; set; }
        public int Bottom { get; set; }
        public int Left { get; set; }
        public int Right { get; set; }

        public Margin(int margin)
        {
            Top = margin;
            Bottom = margin;
            Left = margin;
            Right = margin;
        }

        public Margin(int verticalMargin, int horizontalMargin)
        {
            Top = verticalMargin;
            Bottom = verticalMargin;
            Left = horizontalMargin;
            Right = horizontalMargin;
        }

        public Margin(int top, int right, int bottom, int left)
        {
            Top = top;
            Bottom = bottom;
            Left = left;
            Right = right;
        }
    }
}
