using SkiaSharp;
using SkiaSharp.Elements;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Ryujinx.Skia.Ui.Skia.Widget
{
    public class Button : UIElement, ISelectable, IHoverable, IAction
    {
        public int CornerRadius { get; set; } = 5;
        public int BorderWidth { get; set; } = 2;
        public SKColor BorderColor { get; set; } = SKColors.White;

        public Label Content { get; set; }

        public Rectangle BoundingElement { get; set; }

        public event EventHandler<EventArgs> Activate;

        public string Label { get; set; }

        public string Tag { get; set; }
        public bool IsSelected { get; set; }
        public bool IsHovered { get; set ; }

        public Button(string label, SKRect bounds = default)
        {
            Content = new Label(label)
            {
                BackgroundColor = SKColors.Transparent,
                ForegroundColor = SKColors.Transparent
            };

            BoundingElement = new Rectangle(bounds);

            Bounds = bounds;

            Label = label;
        }

        public override void AttachTo(Scene.Scene parent)
        {
            base.AttachTo(parent);

            Content.AttachTo(parent);

            Content.FontSize = ParentScene.TextSize;

            BackgroundColor = parent.Theme.BackgroundColor;
            ForegroundColor = parent.Theme.ForegroundColor;
        }

        public override void Measure(SKRect bounds)
        {
            Bounds = bounds;

            Content.Measure();

            BoundingElement.Bounds = bounds;

            SKPoint contentLocation = new SKPoint(bounds.MidX - Content.Width / 2, bounds.MidY - Content.Height / 2 );

            Content.Bounds = SKRect.Create(contentLocation, Content.Bounds.Size);
        }

        public override void Draw(SKCanvas canvas)
        {
            base.Draw(canvas);

            if (!DrawElement)
            {
                return;
            }

            ResetStyle();

            BoundingElement.Bounds = Bounds;

            SKRoundRect boundingRect = new SKRoundRect(Bounds, CornerRadius);

            using SKPaint paint = new SKPaint()
            {
                Color = BoundingElement.FillColor,
                Style = SKPaintStyle.Fill
            };

            canvas.DrawRoundRect(boundingRect, paint);

            paint.Color = BoundingElement.BorderColor;
            paint.Style = SKPaintStyle.Stroke;

            canvas.DrawRoundRect(boundingRect, paint);

            canvas.Save();
            canvas.ClipRoundRect(boundingRect, SKClipOperation.Intersect, true);
            try
            {
                Content.Draw(canvas);
            }
            catch (Exception)
            {

            }
            
            canvas.Restore();
        }

        public void ResetStyle()
        {
            BackgroundColor = ParentScene.Theme.PrimaryColor;

            if (IsSelected)
            {
                BoundingElement.FillColor = ParentScene.Theme.SelectBackgroundColor;

                if (Content.ForegroundColor != ParentScene.Theme.SelectForegroundColor)
                {
                    Content.ForegroundColor = ParentScene.Theme.SelectForegroundColor;

                    Content.InvalidateText();
                }
            }
            else if (IsHovered)
            {
                BoundingElement.FillColor = ParentScene.Theme.HoverBackgroundColor;

                if (Content.ForegroundColor != ParentScene.Theme.HoverForegroundColor)
                {
                    Content.ForegroundColor = ParentScene.Theme.HoverForegroundColor;

                    Content.InvalidateText();
                }
            }
            else
            {
                Content.ForegroundColor = ParentScene.Theme.SecondaryColor;

                if (Content.ForegroundColor != ParentScene.Theme.SecondaryColor)
                {
                    Content.ForegroundColor = ParentScene.Theme.SecondaryColor;

                    Content.InvalidateText();
                }

                BoundingElement.FillColor = BackgroundColor;
            }
            BoundingElement.BorderColor = Colors.NeonGrey;
        }

        public void OnActivate()
        {
            Activate?.Invoke(this, null);
        }

        public void OnHover()
        {
            IsHovered = true;
        }

        public void OnSelect()
        {
            IsSelected = true;
        }

        public override void Measure()
        {
            Content.Measure();
            
            SKSize contentSize = Content.Size;

            if(Bounds.Size == default)
            {
                Bounds = SKRect.Create(Bounds.Left, Bounds.Top, contentSize.Width + 50 + Padding.Left + Padding.Right,   contentSize.Height + Padding.Top + Padding.Bottom);
            }

            Measure(Bounds);
        }
    }
}
