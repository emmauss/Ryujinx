using Ryujinx.Skia.Ui.Skia.Widget;
using SkiaSharp;
using SkiaSharp.Elements;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ryujinx.Skia.Ui.Skia.Scene
{
    public class TestScene : Scene
    {
        private Layout _layout;


        public TestScene()
        {
            _layout = new Box(SKRect.Create(400, 20, 500, 500));
            AddElement(_layout);
            _layout.BackgroundColor = SKColor.Parse("#1f1e1d");
            _layout.Padding = new Margin(10);

            Button button = new Button("Add 1 item", SKRect.Create(50, 20, 100, 40));

            button.Activate += Button_Activate;

            AddElement(button);

            button = new Button("Add 30 items", SKRect.Create(50, 500, 150, 40));

            button.Activate += Button30_Activate;

            AddElement(button);

            LottieWidget widget = new LottieWidget(SKRect.Create(50, 100, 200, 400));

            string resourceID = "Ryujinx.Skia.Ui.Assets.greencircle.json";
            Assembly assembly = GetType().GetTypeInfo().Assembly;

            using (Stream stream = assembly.GetManifestResourceStream(resourceID))
            {
                widget.Load(stream);
            }

            AddElement(widget);

            widget = new LottieWidget(SKRect.Create(50, 350, 100, 200));

            resourceID = "Ryujinx.Skia.Ui.Assets.plant.json";
            assembly = GetType().GetTypeInfo().Assembly;

            using (Stream stream = assembly.GetManifestResourceStream(resourceID))
            {
                widget.Load(stream);
            }

            AddElement(widget);

            Icon entry = new Icon("add");
            entry.Bounds = SKRect.Create(500, 100, 100, 50);
            //AddElement(entry);
            entry.BackgroundColor = SKColors.Black;
        }

        private void Button_Activate(object sender, EventArgs e)
        {
            Entry button = new Entry($"Item {_layout.Elements.Count + 1}");
            button.Measure(SKRect.Create(50, 20, 100, 40));

            _layout.AddElement(button);

            var slider = new Slider(SKRect.Create(0,0, 200, 50))
            {
                HorizontalAlignment = LayoutOptions.Stretch,
                Minimum = 0,
                Maximum = 100,
                Value = 0
            };

            _layout.AddElement(slider);

            /*var dialog = new FileDialog(this, "Hey", DialogButtons.OK, "Y");

            Task.Run(() =>
            {
                dialog.Run();

                var result = dialog.DialogResult;

                var input  = dialog.Selected;
            });*/
        }

        private void Button30_Activate(object sender, EventArgs e)
        {
            int i = 0;

            while (i < 30)
            {
                Button button = new Button($"Item {_layout.Elements.Count + 1}", SKRect.Create(50, 20, 100, 40));

                _layout.AddElement(button);
                i++;
            }
        }

        public override void Measure()
        {
            foreach(var element in Elements)
            {
                (element as UIElement).Measure();
            }
        }
    }
}
