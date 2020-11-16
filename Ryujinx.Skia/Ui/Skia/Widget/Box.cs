using SkiaSharp;
using SkiaSharp.Elements;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ryujinx.Skia.Ui.Skia.Widget
{
    public class Box : Layout
    {
        public virtual Orientation Orientation { get; set; } = Orientation.Vertical;

        public Box(SKRect bounds)
        {
            Bounds = bounds;

            ContentSpacing = 0;
        }

        public override void Measure()
        {
            if (ScrollEnabled)
            {
                Scrollbar.Orientation = Orientation;
            }

            base.Measure();

            SKRect contentBounds = default;

            for (int index = 0; index < Elements.Count; index++)
            {
                Element element = Elements[index];

                if (element is UIElement uiElement)
                {
                    uiElement.Measure();


                    SKSize size = contentBounds.Size;
                    var margin = uiElement.Margin;

                    int xMargins = margin.Right + margin.Left;
                    int yMargins = margin.Top + margin.Bottom;


                    if (Orientation == Orientation.Horizontal)
                    {
                        size = new SKSize( size.Width + (index > 0 ? ContentSpacing : 0) + uiElement.Size.Width + xMargins, MathF.Max(uiElement.Height + yMargins, size.Height));
                    }
                    else
                    {
                        size = new SKSize(MathF.Max(size.Width, uiElement.Width +xMargins),size.Height + (index > 0 ? ContentSpacing : 0) + uiElement.Size.Height + yMargins);
                    }

                    contentBounds.Size = size;
                }
            }

            if(Bounds.Size == default)
            {
                Size = contentBounds.Size  + new SKSize(Padding.Left + Padding.Right, Padding.Top + Padding.Bottom);
            }

            Bounds = SKRect.Create(Bounds.Location, Size);

            OnLayout();

            UpdateScrollbars();
        }

        public override void Measure(SKRect bounds)
        {
            Bounds = bounds;
            Measure();
        }

        public override void OnLayout()
        {
            float x = Bounds.Left + Padding.Left, y = Bounds.Top + Padding.Top;

            float contentWidth = Orientation == Orientation.Vertical ? Bounds.Width - Padding.Left - Padding.Right :
                    Bounds.Height - Padding.Top - Padding.Bottom;
            float maxDirectionLength = 0;

            for (int index = 0; index < Elements.Count; index++)
            {
                Element uiElement = Elements[index];
                var element = uiElement as UIElement;

                if (element != null)
                {
                    SKPoint position = new SKPoint();
                    int xMargins = element.Margin.Right + element.Margin.Left;
                    int yMargins = element.Margin.Top + element.Margin.Bottom;

                    float elementHeight = element.Bounds.Height + yMargins;
                    float elementWidth = element.Bounds.Width + xMargins;


                    if (Orientation == Orientation.Horizontal)
                    {
                        position = new SKPoint(x + element.Margin.Left + (index > 0 ? ContentSpacing : 0), y + element.Margin.Top);

                        x += elementWidth + (index > 0 ? ContentSpacing : 0);

                        element.Size = new SKSize(element.Width,  element.VerticalAlignment == LayoutOptions.Stretch ? contentWidth - yMargins : element.Height);

                        switch (element.VerticalAlignment)
                        {
                            case LayoutOptions.End:
                                position.Y = Bounds.Bottom - Margin.Bottom - element.Height - element.Margin.Bottom;
                                break;
                            case LayoutOptions.Center:
                                position.Y = Bounds.MidY - element.Height / 2;
                                break;
                            default:
                                position.Y = y + element.Margin.Top;
                                break;
                        }

                        maxDirectionLength = MathF.Max(maxDirectionLength, x);

                    }
                    else
                    {
                        position = new SKPoint(x + element.Margin.Left, y + element.Margin.Top + (index > 0 ? ContentSpacing : 0));

                        y += elementHeight + (index > 0 ? ContentSpacing : 0);

                        element.Size = new SKSize(element.HorizontalAlignment == LayoutOptions.Stretch ? contentWidth - xMargins : element.Width, element.Height);

                        switch (element.HorizontalAlignment)
                        {
                            case LayoutOptions.End:
                                position.X = Bounds.Right - Margin.Right - element.Width - element.Margin.Right;
                                break;
                            case LayoutOptions.Center:
                                position.X = Bounds.MidX - element.Width / 2;
                                break;
                            default:
                                position.X = x + element.Margin.Left;
                                break;
                        }

                        maxDirectionLength = MathF.Max(maxDirectionLength, y);
                    }

                    element.Measure(SKRect.Create( position, element.Size));

                    var bounds = element.Bounds;

                    bounds.Location = position;

                    element.Bounds = bounds;
                }
            }


            ContentSize = Orientation == Orientation.Vertical ?
                            new SKSize(contentWidth, y - Bounds.Top) :
                            new SKSize(x - Bounds.Left, contentWidth);

            if (LayoutOptions != LayoutOptions.Start)
            {
                float alignment = Orientation == Orientation.Horizontal ?
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

                if (Orientation == Orientation.Horizontal)
                {
                    for (int i = 0; i < Elements.Count; i++)
                    {
                        Element element = Elements[i];
                        if (element is UIElement uiElement)
                        {
                            element.Location = new SKPoint(element.Location.X + alignment, element.Location.Y);
                            uiElement.Measure();
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < Elements.Count; i++)
                    {
                        Element element = Elements[i];
                        if (element is UIElement uiElement)
                        {
                            element.Location = new SKPoint(element.Location.X, element.Location.Y + alignment);
                            uiElement.Measure();
                        }
                    }
                }
            }
        }
    }
}
