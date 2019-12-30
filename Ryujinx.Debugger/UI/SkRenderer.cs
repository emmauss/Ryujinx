using System;
using System.Collections.Generic;
using System.Text;
using Gtk;
using SkiaSharp.Views.Gtk;
using SkiaSharp;
using Gdk;
using Microcharts;

namespace Ryujinx.Debugger.UI
{
    public class SkRenderer : SKDrawingArea
    {
        public Chart Chart { get; set; }
        public event EventHandler DrawGraphs;

        public SkRenderer()
        {
            this.PaintSurface += SkRenderer_PaintSurface;
        }

        private void SkRenderer_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
        {
            e.Surface.Canvas.Clear(SKColors.Black);
            if(Chart != null)
            {
                Chart.Draw(e.Surface.Canvas, e.Info.Width, e.Info.Height);
            }
           else
            {
                DrawGraphs.Invoke(this, e);
            }
        }


    }
}
