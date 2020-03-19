using SkiaSharp;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.Gtk;
using System;

namespace Ryujinx.Ui
{
    public class SkRenderer : SKDrawingArea
    {
        public event EventHandler DrawGraphs;

        private SKColor _backgroundColor;

        public SkRenderer(SKColor backgroundColor)
        {
            _backgroundColor = backgroundColor;

            PaintSurface += SkRenderer_PaintSurface;
        }

        private void SkRenderer_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            e.Surface.Canvas.Clear(_backgroundColor);

            DrawGraphs.Invoke(this, e);
        }
    }
}