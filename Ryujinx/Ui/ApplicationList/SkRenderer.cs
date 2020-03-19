using SkiaSharp;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.Gtk;
using System;

namespace Ryujinx.Ui
{
    public class SkRenderer : SKDrawingArea
    {
        public event EventHandler DrawObjects;

        private readonly SKColor _backgroundColor;

        public SkRenderer(SKColor backgroundColor)
        {
            _backgroundColor = backgroundColor;

            this.PaintSurface += SkRenderer_PaintSurface;
        }

        private void SkRenderer_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            e.Surface.Canvas.Clear(_backgroundColor);

            DrawObjects?.Invoke(this, e);
        }
    }
}