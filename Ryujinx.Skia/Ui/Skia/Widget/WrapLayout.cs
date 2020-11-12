using SkiaSharp;
using SkiaSharp.Elements;
using SkiaSharp.Elements.Collections;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ryujinx.Skia.Ui.Skia.Widget
{
    public class WrapLayout : Layout
    {
        public Orientation Direction { get; set; } = Orientation.Horizontal;

        public WrapLayout(SKRect bounds)
        {
            Bounds = bounds;
        }

        public override void Measure()
        {
            if (ScrollEnabled)
            {
                Scrollbar.Orientation = Direction == Orientation.Horizontal ? Orientation.Vertical : Orientation.Horizontal;
            }

            base.Measure();

            for (int i = 0; i < Elements.Count; i++)
            {
                Element uiElement = Elements[i];
                (uiElement as UIElement)?.Measure();
            }

            OnLayout();

            UpdateScrollbars();
        }

        public override void Measure(SKRect bounds)
        {
            Measure();   
        }

        public override void OnLayout()
        {
            float maxDirectionLimit = Direction == Orientation.Horizontal? Bounds.Right : Bounds.Bottom;
            int maxLineSize = 0;
            float x = Bounds.Left + Padding.Left, y = Bounds.Top + Padding.Top;
            float maxDirectionLength = 0;

            for (int i = 0; i < Elements.Count; i++)
            {
                Element uiElement = Elements[i];
                var element = uiElement as UIElement;

                if (element != null)
                {
                    SKPoint position = new SKPoint();
                    float elementHeight = element.Bounds.Height + element.Margin.Top + element.Margin.Bottom;
                    float elementWidth = element.Bounds.Width + element.Margin.Left + element.Margin.Right;

                    if (Direction == Orientation.Horizontal)
                    {
                        maxLineSize = (int)Math.Max(maxLineSize, elementHeight);

                        if(x + element.Width > maxDirectionLimit - Padding.Right)
                        {
                            maxDirectionLength = MathF.Max(maxDirectionLength, x);
                            x = Bounds.Left + Padding.Left;
                            y += maxLineSize + ContentSpacing;
                        }
                        maxDirectionLength = MathF.Max(maxDirectionLength, x);

                        position = new SKPoint(x + element.Margin.Left, y + element.Margin.Top);

                        x += elementWidth + ContentSpacing;
                    }
                    else
                    {
                        maxLineSize = (int)Math.Max(maxLineSize, elementWidth);

                        if (y + element.Height > maxDirectionLimit - Padding.Bottom)
                        {
                            maxDirectionLength = MathF.Max(maxDirectionLength, y);
                            y = Bounds.Top + Padding.Top;
                            x += maxLineSize + ContentSpacing;
                        }
                        
                        maxDirectionLength = MathF.Max(maxDirectionLength, y);

                        position = new SKPoint(x + element.Margin.Left, y + element.Margin.Top);

                        y += elementHeight;
                    }

                    var bounds = element.Bounds;

                    bounds.Location = position;

                    element.Bounds = bounds;
                }
            }

            ContentSize = Direction == Orientation.Horizontal ?
                            new SKSize(Bounds.Width, y + maxLineSize - Bounds.Top) :
                            new SKSize(x + maxLineSize - Bounds.Left, Bounds.Height);

            if (LayoutOptions != LayoutOptions.Start) 
            {
                float alignment = Direction == Orientation.Horizontal ?
                        Bounds.Width - (maxDirectionLength - Bounds.Left) : Bounds.Height - (maxDirectionLength - Bounds.Top);

                alignment = alignment < 0 ? 0 : alignment;

                switch (LayoutOptions)
                {
                    case LayoutOptions.Center:
                        alignment = alignment / 2;
                        break;
                    case LayoutOptions.End:
                        break;
                    default:
                        alignment = 0;
                        break;
                }

                if (Direction == Orientation.Horizontal)
                {
                    for (int i = 0; i < Elements.Count; i++)
                    {
                        Element element = Elements[i];
                        if (element is UIElement)
                        {
                            element.Location = new SKPoint(element.Location.X + alignment, element.Location.Y);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < Elements.Count; i++)
                    {
                        Element element = Elements[i];
                        if (element is UIElement)
                        {
                            element.Location = new SKPoint(element.Location.X, element.Location.Y + alignment);
                        }
                    }
                }
            }
        }
    }
}
