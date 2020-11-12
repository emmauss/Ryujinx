using SkiaSharp;
using SkiaSharp.Elements;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Ryujinx.Skia.Ui.Skia.Widget
{
    public class Checkbutton : UIElement, ISelectable, IHoverable, IAction
    {
        private const int CheckBoxSize = 20;

        private readonly SKPath _checkMarkPath = SKPath.ParseSvgPathData(@"M 22.566406 4.730469 L 20.773438 3.511719 
C 20.277344 3.175781 19.597656 3.304688 19.265625 3.796875 
L 10.476563 16.757813 L 6.4375 12.71875 C 6.015625 12.296875 
5.328125 12.296875 4.90625 12.71875 L 3.371094 14.253906 
C 2.949219 14.675781 2.949219 15.363281 3.371094 15.789063 
L 9.582031 22 C 9.929688 22.347656 10.476563 22.613281 
10.96875 22.613281 C 11.460938 22.613281 11.957031 22.304688 
12.277344 21.839844 L 22.855469 6.234375 C 23.191406 5.742188 
23.0625 5.066406 22.566406 4.730469 Z ");

        public int CornerRadius { get; set; } = 5;
        public int BorderWidth { get; set; } = 2;
        public SKColor BorderColor { get; set; } = SKColors.White;

        public Label Content { get; set; }

        public Rectangle BoundingElement { get; set; }

        private readonly Rectangle _checkBox;
        private SKPath _renderedCheck;

        public event EventHandler<EventArgs> Activate;

        public string Label { get; set; }

        public bool Checked{ get; set; }

        private SKColor _checkBoxColor;


        public bool IsSelected { get; set; }
        public bool IsHovered { get; set; }

        public Checkbutton(string label, SKRect bounds = default)
        {
            Content = new Label(label);

            BoundingElement = new Rectangle(bounds);
            _checkBox = new Rectangle(SKRect.Create(CheckBoxSize, CheckBoxSize));

            _checkBox.BorderColor = SKColors.LightGray;

            Bounds = bounds;

            Label = label;
        }

        public override void AttachTo(Scene.Scene parent)
        {
            base.AttachTo(parent);

            float textWidth = new SKPaint() { TextSize = ParentScene.TextSize }.MeasureText(Label);

            Content.AttachTo(parent);

            Content.Bounds = SKRect.Create(new SKSize(textWidth, ParentScene.TextSize));

            Content.FontSize = ParentScene.TextSize;

            ForegroundColor = parent.Theme.ForegroundColor;
            BackgroundColor = parent.Theme.BackgroundColor;

            _checkBoxColor = ForegroundColor;
        }

        public override void Measure(SKRect bounds)
        {
            Bounds = bounds;

            BoundingElement.Bounds = bounds;

            SKPoint contentLocation = new SKPoint(bounds.Left + Padding.Left + CheckBoxSize + 10, bounds.MidY - Content.Height / 2 + 1);
 
            Content.Bounds = SKRect.Create(contentLocation, Content.Bounds.Size);

            _checkBox.Location = new SKPoint(bounds.Left + Padding.Left, bounds.MidY - _checkBox.Bounds.Height / 2);


            var checkSize = _checkMarkPath.Bounds;
            var pathScale = CheckBoxSize / Math.Max(checkSize.Height, checkSize.Width) * 0.70f;
            var scaleMatrix = SKMatrix.CreateScale(pathScale, pathScale);

            var positionMatrix = SKMatrix.CreateTranslation(_checkBox.Location.X + 1, _checkBox.Location.Y);
            _renderedCheck = new SKPath(_checkMarkPath);

            _renderedCheck.Transform(scaleMatrix);
            _renderedCheck.Transform(positionMatrix);
            Content.Measure();
        }

        public override void Draw(SKCanvas canvas)
        {
            base.Draw(canvas);

            if (!DrawElement)
            {
                return;
            }

            ResetStyle();

            canvas.Save();

            SKRoundRect boundingRect = new SKRoundRect(Bounds, CornerRadius);

            using SKPaint paint = new SKPaint()
            {
                Color = BoundingElement.FillColor,
                Style = SKPaintStyle.Fill
            };

            canvas.DrawRoundRect(boundingRect, paint);

            canvas.ClipRoundRect(boundingRect);

            paint.Color = BoundingElement.BorderColor;
            paint.Style = SKPaintStyle.Stroke;

            canvas.DrawRoundRect(boundingRect, paint);

            Content.Draw(canvas);
            _checkBox.Draw(canvas);

            if (Checked)
            {
                using (SKPaint checkPaint = new SKPaint())
                {
                    checkPaint.Color = ForegroundColor;
                    checkPaint.IsAntialias = true;

                    canvas.DrawPath(_renderedCheck, checkPaint);
                }
            }

            canvas.Restore();
        }

        public void ResetStyle()
        {
            BackgroundColor = ParentScene.Theme.BackgroundColor;
            ForegroundColor = ParentScene.Theme.ForegroundColor;
            BoundingElement.BorderColor = BorderColor;
            BoundingElement.FillColor = ParentScene.Theme.BackgroundColor;
            BoundingElement.BorderWidth = BorderWidth;
            BoundingElement.CornerRadius = new SKPoint(CornerRadius, CornerRadius);
            Content.ForegroundColor = ParentScene.Theme.ForegroundColor;

            _checkBox.FillColor = SKColors.White;

            if (IsSelected)
            {
                if (_checkBoxColor != ParentScene.Theme.SelectForegroundColor)
                {
                    _checkBoxColor = ParentScene.Theme.SelectForegroundColor;
                    _checkBox.BorderColor = ParentScene.Theme.SelectForegroundColor;

                    Content.InvalidateText();
                }
            }
            else if (IsHovered)
            {
                if (_checkBoxColor != ParentScene.Theme.HoverForegroundColor)
                {
                    _checkBoxColor = ParentScene.Theme.HoverForegroundColor;
                    _checkBox.BorderColor = ParentScene.Theme.HoverForegroundColor;

                    Content.InvalidateText();
                }
            }
            else
            {
                if (_checkBoxColor != ParentScene.Theme.SecondaryColor)
                {
                    _checkBoxColor = ParentScene.Theme.ForegroundColor;
                    _checkBox.BorderColor = ParentScene.Theme.ForegroundColor;

                    Content.InvalidateText();
                }

                BoundingElement.FillColor = BackgroundColor;
            }
        }

        public void OnActivate()
        {
            Checked = !Checked;

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
            Content.Size = default;

            Content.Measure();

            SKSize contentSize = Content.Size;
            contentSize.Height = MathF.Max(contentSize.Height, CheckBoxSize);

            if(Bounds == default)
            {
                Bounds = SKRect.Create(0, 0, contentSize.Width + Padding.Left + Padding.Right + CheckBoxSize + 10,   contentSize.Height + Padding.Top + Padding.Bottom);
            }

            Measure(Bounds);
        }
    }
}
