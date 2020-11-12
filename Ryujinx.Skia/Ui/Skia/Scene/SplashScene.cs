using Ryujinx.Skia.Ui.Skia.Widget;
using SkiaSharp;
using SkiaSharp.Elements;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Xamarin.Forms;
using Image = SkiaSharp.Elements.Image;
using System.Threading.Tasks;

namespace Ryujinx.Skia.Ui.Skia.Scene
{
    public class SplashScene : Scene
    {
        private LottieWidget _widget;

        public SplashScene()
        {
            string resourceID = "Ryujinx.Skia.Ui.Assets.gray.json";
            Assembly assembly = GetType().GetTypeInfo().Assembly;

            using (Stream stream = assembly.GetManifestResourceStream(resourceID))
            {
                SKRect bounds = SKRect.Create(0, 0 ,240,320);
                _widget = new LottieWidget(bounds);
                _widget.Load(stream);
            }
            _widget.Speed = 2f;
            Elements.Add(_widget);

            _widget.Stopped += Widget_Stopped;
        }

        private void Widget_Stopped(object sender, EventArgs e)
        {
            _widget.Stopped -= Widget_Stopped;
            _widget.Dispose();
            IManager.Instance.NavigateTo(new HomeScene());
        }

        public override void Measure()
        {
            var bounds = IManager.Instance.Bounds;
            SKPoint location = new SKPoint(bounds.MidX - 120, bounds.MidY - 120);

            _widget.Location = location;
        }

        public override void Draw(SKCanvas canvas)
        {
            base.Draw(canvas);
        }

        public void End(){
            _widget.FadeOut();
        }
    }
}
