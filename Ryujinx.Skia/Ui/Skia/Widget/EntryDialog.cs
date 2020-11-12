using Ryujinx.Skia.Ui.Skia.Scene;
using SkiaSharp;
using SkiaSharp.Elements;
using SkiaSharp.Elements.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Ryujinx.Skia.Ui.Skia.Widget
{
    public class EntryDialog : Dialog
    {
        public string PrimaryText { get; set; }

        private Box _contentBox;
        private Label _primaryText;

        private Entry _entry;

        public string Input => _entry.Text;

        public EntryDialog(Scene.Scene parent,
                      string title,
                      string primaryText,
                      DialogButtons buttons,
                      string acceptButtonText = "",
                      string declineButtonText = "",
                      string cancelButtonText = "") : base(parent, title, buttons, acceptButtonText, declineButtonText, cancelButtonText)
        {
            PrimaryText = primaryText;

            _contentBox = new Box(default)
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = LayoutOptions.Center,
                ScrollEnabled = false
            };

            _contentBox.AttachTo(parent);

            _primaryText = new Label(PrimaryText);
            _entry = new Entry();

            _primaryText.HorizontalAlignment = LayoutOptions.Center;
            _entry.HorizontalAlignment = LayoutOptions.Center;
            _primaryText.ForegroundColor = ParentScene.Theme.ForegroundColor;
            _entry.ForegroundColor = ParentScene.Theme.ForegroundColor;

            _entry.HorizontalAlignment = LayoutOptions.Stretch;

            _contentBox.AddElement(_primaryText);
            _contentBox.AddElement(_entry);
        }

        public override void DrawContent(SKCanvas canvas)
        {
            _contentBox.Draw(canvas);
        }

        public override SKRect MeasureContent(SKRect bounds)
        {
            _contentBox.Measure(Bounds);

            return _contentBox.Bounds;
        }

        public override Element GetElementInContent(SKPoint point)
        {
            return (UIElement)_contentBox.GetElementAtPosition(point);
        }
    }
}
