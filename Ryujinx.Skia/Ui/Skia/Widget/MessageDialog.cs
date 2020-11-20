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
    public class MessageDialog : Dialog
    {
        public string PrimaryText { get; set; }
        public string SecondaryText { get; set; }

        private Box _contentBox;
        private Label _primaryText;
        private Label _secondaryText;

        public MessageDialog(Scene.Scene parent,
                      string title,
                      string primaryText,
                      string secondaryText,
                      DialogButtons buttons,
                      string acceptButtonText = "",
                      string declineButtonText = "",
                      string cancelButtonText = "") : base(parent, title, buttons, acceptButtonText, declineButtonText, cancelButtonText)
        {
            PrimaryText = primaryText;
            SecondaryText = secondaryText;

            _contentBox = new Box(default)
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = LayoutOptions.Center,
                ScrollEnabled = false
            };

            _contentBox.AttachTo(parent);

            _primaryText = new Label(PrimaryText);
            _secondaryText = new Label(SecondaryText);

            _primaryText.HorizontalAlignment = LayoutOptions.Center;
            _secondaryText.HorizontalAlignment = LayoutOptions.Center;
            _primaryText.ForegroundColor = ParentScene.Theme.ForegroundColor;
            _secondaryText.ForegroundColor = ParentScene.Theme.ForegroundColor;

            _contentBox.AddElement(_primaryText);
            _contentBox.AddElement(_secondaryText);
        }

        public override void DrawContent(SKCanvas canvas)
        {
            _contentBox.Draw(canvas);
        }

        public override SKRect MeasureContent(SKRect bounds)
        {
            _contentBox.Measure(default);

            if(_contentBox.Width > DialogWidth)
            {
                DialogWidth = (int)(_contentBox.Width + Margin.Left + Margin.Right);

                bounds.Size = new SKSize(DialogWidth, _contentBox.Height);
            }

            _contentBox.Measure(bounds);

            return _contentBox.Bounds;
        }

        public override Element GetElementInContent(SKPoint point)
        {
            return _contentBox.GetElementAtPosition(point);
        }
    }
}
