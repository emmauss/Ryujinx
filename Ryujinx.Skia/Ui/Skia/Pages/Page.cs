using Ryujinx.Skia.Ui.Skia.Scene;
using Ryujinx.Skia.Ui.Skia.Widget;
using SkiaSharp;
using SkiaSharp.Elements;
using SkiaSharp.Elements.Collections;

namespace Ryujinx.Skia.Ui.Skia.Pages
{
    public abstract class Page : UIElement, IUICollection
    {
        private readonly ElementsController _controller = new ElementsController();

        public ElementsController Controller => _controller;
        public ElementsCollection Elements => Controller.Elements;

        public int TextSize { get; set; } = 16;
        public bool Loaded { get; protected set; }

        public override void Draw(SKCanvas canvas)
        {
            base.Draw(canvas);

            Controller.Draw(canvas);

            DrawContent(canvas);
        }

        public abstract void DrawContent(SKCanvas canvas);

        public Element GetElementAtPosition(SKPoint position)
        {
            return Elements.GetElementAtPoint(position);
        }

        public void AddElement(UIElement element)
        {
            if (ParentScene != null)
            {
                element.AttachTo(ParentScene);
            }

            Elements.Add(element);
            IManager.Instance?.InvalidateMeasure();
        }

        public override void AttachTo(Scene.Scene parent)
        {
            base.AttachTo(parent);

            foreach(var element in Elements)
            {
                if(element is UIElement uIElement)
                {
                    uIElement.AttachTo(parent);
                }
            }
        }
    }
}