using Ryujinx.Skia.Ui.Skia.Scene;
using SkiaSharp;
using SkiaSharp.Elements;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Topten.RichTextKit;

namespace Ryujinx.Skia.Ui.Skia.Widget
{
    public class Label : UIElement
    {
        public TextAlignment TextAlignment
        {
            get => _textAlignment; set
            {
                _textAlignment = value;

                _recreateRenderer = true;

                IManager.Instance.InvalidateMeasure();
            }
        }
        private string _text;

        public bool IsAnimated { get; set; } = false;

        public bool IsSingleLine { get; set; } = true;

        private float _offset = 0;
        private string _fontFamily;
        private SKFontStyle _fontStyle;
        private TextAlignment _textAlignment;

        private RichString _renderer;

        private bool _recreateRenderer = true;
        private Animation _nextAnimation;

        public string Text
        {
            get => _text; set
            {
                if (value != _text)
                {
                    _text = value;

                    _recreateRenderer = true;

                    Bounds = SKRect.Create(Bounds.Location, default);

                    IManager.Instance.InvalidateMeasure();
                }
            }
        }

        public override SKColor ForegroundColor
        {
            get => base.ForegroundColor; set
            {
                if (ForegroundColor != value)
                {
                    base.ForegroundColor = value;

                    _recreateRenderer = true;
                }
            }
        }

        public override SKColor BackgroundColor
        {
            get => base.BackgroundColor; set
            {
                if (BackgroundColor != value)
                {
                    base.BackgroundColor = value;

                    _recreateRenderer = true;
                }
            }
        }

        public SKTypeface Typeface { get; private set; } = SKTypeface.Default;

        public string FontFamily
        {
            get => _fontFamily; set
            {
                _fontFamily = value;

                _recreateRenderer = true;

                Typeface = SKTypeface.FromFamilyName(value, FontStyle);

                IManager.Instance.InvalidateMeasure();
            }
        }

        public SKFontStyle FontStyle
        {
            get => _fontStyle; set
            {
                _fontStyle = value;

                _recreateRenderer = true;

                Typeface = SKTypeface.FromFamilyName(FontFamily, value);

                IManager.Instance.InvalidateMeasure();
            }
        }

        public int FontSize { get; set; } = 16;

        public Label()
        {
            Text = string.Empty;

            _fontFamily = "Calibri";

            _fontStyle = SKFontStyle.Normal;

            _textAlignment = TextAlignment.Left;
        }

        public Label(string text, int fontSize = 14)
        {
            FontSize = fontSize;
            Text = text;

            _fontFamily = "Calibri";

            _fontStyle = SKFontStyle.Normal;

            _textAlignment = TextAlignment.Left;
        }

        public void InvalidateText()
        {
            _renderer = new RichString();
            _renderer.TextColor(ForegroundColor);
            _renderer.Alignment(_textAlignment);
            _renderer.FontSize(FontSize);
            _renderer.FontWeight(FontStyle.Weight);
            _renderer.FontItalic(FontStyle.Slant == SKFontStyleSlant.Italic);
            _renderer.MarginLeft(Padding.Left);
            _renderer.MarginTop(Padding.Top);
            _renderer.MarginRight(Padding.Right);
            _renderer.MarginBottom(Padding.Bottom);
            _renderer.FontFamily(FontFamily);
            _renderer.Add(Text);

            if (!IsSingleLine && Bounds.Width > 0)
            {
                _renderer.MaxWidth = (float?)Bounds.Width;
            }
        }

        public override void AttachTo(Scene.Scene parent)
        {
            base.AttachTo(parent);

            Typeface = SKTypeface.FromFamilyName(FontFamily, FontStyle);
            
            ForegroundColor = parent.Theme.ForegroundColor;
        }


        public override void Draw(SKCanvas canvas)
        {
            base.Draw(canvas);

            if (!DrawElement)
            {
                return;
            }

            try
            {
                _renderer?.Paint(canvas, Bounds.Location - new SKPoint(_offset, 0));
            }
            catch (Exception)
            {

            }
        }

        public override void Measure()
        {
            if (ParentScene == null)
            {
                return;
            }

            if (_recreateRenderer)
            {
                _recreateRenderer = false;

                _renderer = new RichString();
                _renderer.TextColor(ForegroundColor);
                _renderer.Alignment(_textAlignment);
                _renderer.FontSize(FontSize);
                _renderer.FontWeight(FontStyle.Weight);
                _renderer.FontItalic(FontStyle.Slant == SKFontStyleSlant.Italic);
                _renderer.MarginLeft(Padding.Left);
                _renderer.MarginTop(Padding.Top);
                _renderer.MarginRight(Padding.Right);
                _renderer.MarginBottom(Padding.Bottom);
                _renderer.FontFamily(FontFamily);
                _renderer.Add(Text);

                if (!IsSingleLine && Bounds.Width > 0)
                {
                    _renderer.MaxWidth = (float?)Bounds.Width;
                }
            }

            if (Bounds.Size.IsEmpty || Height == 0)
            {
                try
                {
                    var width = _renderer.MeasuredWidth + Padding.Left + Padding.Right;
                    var height = _renderer.MeasuredHeight;

                    Size = new SKSize(width, height);
                }
                catch (Exception)
                {

                }
            }
        }

        public override void Measure(SKRect bounds)
        {
            Bounds = bounds;

            Measure();
        }

        public void ResetAnimation()
        {
            if (Animator != null && Animator.IsActive)
            {
                Animator?.Stop(false);
                _nextAnimation?.Stop(false);
                _offset = 0;
            }
        }

        public override void Dispose()
        {
            _nextAnimation?.Stop(false);
            base.Dispose();
        }

        public void Animate()
        {
            if ((Animator == null || !(bool)(Animator?.IsActive)) && (_nextAnimation == null || !(bool)_nextAnimation?.IsActive))
            {
                _offset = 0;

                if (IsSingleLine)
                {
                    var size = _renderer.MeasuredWidth;

                    if (size > Bounds.Width)
                    {
                        float offset = size - Bounds.Width;
                        Animator?.Stop();

                        long duration = (long)(offset / 30 * 1000);

                        Animator = new Animation();

                        _nextAnimation = new Animation();

                        _nextAnimation.With(0, offset, duration, (value) =>
                        {
                            _offset = (float)value;
                            IManager.Instance.InvalidateMeasure();
                        },
                             endCallback: () =>
                                {
                                    Thread.Sleep(2000);
                                    _offset = 0;
                                    _nextAnimation = null;
                                });

                        Animator.With(0, 1, 1000);
                        Animator.ContinueWith(_nextAnimation);

                        Animator.Play();
                    }
                }
            }
            else
            {

            }
        }
    }
}