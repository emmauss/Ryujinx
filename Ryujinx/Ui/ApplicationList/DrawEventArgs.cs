using SkiaSharp;
using System;

namespace Ryujinx.Ui
{
    public class DrawEventArgs : EventArgs
    {
        public bool QueueRender { get; set; }

        public SKCanvas Canvas { get; set; }
    }
}