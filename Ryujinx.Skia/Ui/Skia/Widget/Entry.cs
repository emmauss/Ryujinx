using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Ryujinx.Skia.Ui.Skia.Scene;
using SkiaSharp;
using SkiaSharp.Elements;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Topten.RichTextKit;
using static Ryujinx.Skia.Ui.Skia.Widget.IInput;

namespace Ryujinx.Skia.Ui.Skia.Widget
{
    public class Entry : UIElement, IInput, IHoverable
    {
        private bool _shiftPressed;

        public event EventHandler TextChanged;

        public event EventHandler<InputEventArgs> Input;

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

        public Entry(bool isAnimated, bool isSingleLine, SKTypeface typeface, int fontSize)
        {
            this.IsAnimated = isAnimated;
            this.IsSingleLine = isSingleLine;
            this.Typeface = typeface;
            this.FontSize = fontSize;

        }

        public bool IsAnimated { get; set; } = false;

        public bool IsSingleLine { get; set; } = true;

        private float _offset = 0;
        private string _fontFamily;
        private SKFontStyle _fontStyle;
        private TextAlignment _textAlignment;

        private RichString _renderer;

        private bool _recreateRenderer = true;
        private readonly Animation _nextAnimation;

        private int _caretPosition;

        private TextPaintOptions _selectionOptions = new TextPaintOptions()
        {
            IsAntialias = true,
            SelectionColor = Colors.NeonBlue
        };

        public string Text
        {
            get => _text; set
            {
                lock (this)
                {
                    _text = value;

                    _recreateRenderer = true;

                    IManager.Instance.InvalidateMeasure();

                    TextChanged?.Invoke(this, null);
                }
            }
        }

        public override SKColor ForegroundColor
        {
            get => base.ForegroundColor; set
            {
                base.ForegroundColor = value;

                _recreateRenderer = true;
            }
        }

        public override SKColor BackgroundColor
        {
            get => base.BackgroundColor; set
            {
                base.BackgroundColor = value;

                _recreateRenderer = true;
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
        public bool IsInputGrabbed { get; set ; }
        public bool IsHovered { get; set; }

        public Animation NextAnimation => _nextAnimation;

        private CaretInfo _caret = new CaretInfo();

        public Entry()
        {
            Text = string.Empty;

            _fontFamily = "Calibri";

            _fontStyle = SKFontStyle.Normal;

            _textAlignment = TextAlignment.Left;

            _nextAnimation = new Animation();

            Padding = new Margin(10);
        }

        public Entry(string text, int fontSize = 14)
        {
            FontSize = fontSize;
            Text = text;

            _fontFamily = "Calibri";

            _fontStyle = SKFontStyle.Normal;

            _textAlignment = TextAlignment.Left;

            _nextAnimation = new Animation();

            Padding = new Margin(10);
        }

        public void InvalidateText()
        {
            _renderer = new RichString();
            _renderer.TextColor(ForegroundColor);
            _renderer.Alignment(_textAlignment);
            _renderer.FontSize(FontSize);
            _renderer.FontWeight(FontStyle.Weight);
            _renderer.FontItalic(FontStyle.Slant == SKFontStyleSlant.Italic);
            _renderer.FontFamily(FontFamily);
            _renderer.Add(Text);

            SetCaretOffset();

            if (!IsSingleLine && Bounds.Width > 0)
            {
                _renderer.MaxWidth = Bounds.Width;
            }
        }

        public override void AttachTo(Scene.Scene parent)
        {
            base.AttachTo(parent);

            Typeface = SKTypeface.FromFamilyName(FontFamily, FontStyle);

            ForegroundColor = parent.Theme.ForegroundColor;

            FontFamily = parent.Theme.FontFamily;
        }


        public override void Draw(SKCanvas canvas)
        {
            base.Draw(canvas);

            if (!DrawElement)
            {
                return;
            }

            if (_recreateRenderer)
            {
                _recreateRenderer = false;

                InvalidateText();
            }
            else
            {
                SetCaretOffset();
            }

            if (IsHovered)
            {
                IManager.Instance.SetCursorMode(CursorMode.Insertion);
            }

            canvas.Save();

            using SKPaint paint = new SKPaint()
            {
                Color = BackgroundColor,
                Style = SKPaintStyle.Fill
            };

            canvas.DrawRect(Bounds, paint);

            SKRect drawBounds = SKRect.Create(Bounds.Left + Padding.Left,
                                                Bounds.Top + Padding.Top,
                                                Bounds.Width - Padding.Left - Padding.Right,
                                                Bounds.Height - Padding.Top - Padding.Bottom);

            canvas.ClipRect(drawBounds, antialias: true);

            _caretPosition = _caretPosition < 0 ? 0 : _caretPosition;

            int position = _caretPosition;

            if (_caretPosition >= Text.Length)
            {
                position = Text.Length;
            }

            var caret = _renderer?.GetCaretInfo(position);

            var caretLocation = new SKPoint(2, 0);
            var caretSize = new SKSize(2, drawBounds.Height);

            lock (this)
            {
                if (caret.HasValue)
                {
                    _caret = caret.Value;

                    caretLocation = _caret.CaretRectangle.Location;

                    try
                    {
                        _renderer?.Paint(canvas, drawBounds.Location - new SKPoint(_offset, -2), _selectionOptions);
                    }
                    catch (Exception)
                    {

                    }
                }
            }

            if (IsInputGrabbed)
            {
                SKRect caretBounds = SKRect.Create(caretLocation + drawBounds.Location - new SKPoint(_offset, 0), caretSize);

                paint.Color = Colors.NeonRed;
                paint.Style = SKPaintStyle.StrokeAndFill;
                canvas.DrawRect(caretBounds, paint);
            }

            if (position == 0 || caretLocation.X == 0)
            {

            }

            canvas.Restore();


            paint.Color = SKColors.LightGray;
            paint.Style = SKPaintStyle.Stroke;
            paint.StrokeWidth = 2;
            canvas.DrawRect(Bounds, paint);
        }

        public override void Measure()
        {
            if (_recreateRenderer)
            {
                _recreateRenderer = false;

                _renderer = new RichString();
                _renderer.TextColor(ForegroundColor);
                _renderer.Alignment(_textAlignment);
                _renderer.FontSize(FontSize);
                _renderer.FontWeight(FontStyle.Weight);
                _renderer.FontFamily(FontFamily);
                _renderer.FontItalic(FontStyle.Slant == SKFontStyleSlant.Italic);
                _renderer.Add(Text);

                _caretPosition = Math.Clamp(_caretPosition, 0, Text.Length);

                SetCaretOffset();

                _caret = _renderer.GetCaretInfo(_caretPosition);

                if (!IsSingleLine && Bounds.Width > 0)
                {
                    _renderer.MaxWidth = (float?)Bounds.Width - Padding.Left - Padding.Right;
                }
            }

            if (Bounds.Size.IsEmpty)
            {
                var width = _renderer.MeasuredWidth + Padding.Left + Padding.Right;
                var height = _renderer.MeasuredHeight;

                if (height == 0)
                {
                    height = FontSize + 2;
                }

                height += Padding.Top + Padding.Bottom;

                Size = new SKSize(width, height);
            }
        }

        public override void Measure(SKRect bounds)
        {
            Bounds = bounds;

            Measure();
        }

        public void OnGrabInput()
        {
            IsInputGrabbed = true;
        }

        public void HandleMouse(SKPoint position, InputMode inputMode)
        {
            var point = position - Bounds.Location - new SKPoint(Padding.Left, Padding.Top);
            var hit = _renderer.HitTest(point.X, point.Y);
            if (inputMode != InputMode.None)
            {
                _caret = _renderer.GetCaretInfo(hit.ClosestCodePointIndex);
            }

            switch (inputMode)
            {
                case InputMode.MouseDown:
                    _selectionOptions = new TextPaintOptions
                    {
                        SelectionStart = hit.ClosestCodePointIndex,
                        IsAntialias = true,
                        SelectionColor = Colors.NeonBlue
                    };
                    break;

                case InputMode.MousePress:
                    _selectionOptions.SelectionEnd = hit.ClosestCodePointIndex;
                    break;

                case InputMode.MouseUp:
                    _selectionOptions.SelectionEnd = hit.ClosestCodePointIndex;
                    break;
            }

            _caretPosition = _caret.CodePointIndex;
        }

        public void DeleteSelection()
        {
            var selection = _selectionOptions;

            selection.SelectionStart ??= 0;
            selection.SelectionEnd   ??= 0;

            if (selection.SelectionStart != selection.SelectionEnd)
            {
                int start = Math.Min((int)selection.SelectionStart, (int)selection.SelectionEnd);
                int end   = Math.Max((int)selection.SelectionStart, (int)selection.SelectionEnd);

                start = start >= 0 ? start : 0;
                end   = end   >= 0 ? end   : 0;

                Text = Text.Remove(start, end - start);

                _caretPosition = (int)Math.Min(selection.SelectionStart.Value, selection.SelectionEnd.Value);

                _selectionOptions.SelectionStart = _caretPosition;
                _selectionOptions.SelectionEnd   = _caretPosition;
            }
        }

        public void HandleKeyboard(Keys key, KeyModifiers modifiers, InputMode inputMode)
        {
            var selection = _selectionOptions;

            selection.SelectionStart ??= 0;
            selection.SelectionEnd   ??= 0;

            _shiftPressed = false;

            try
            {
                if (_caretPosition >= Text.Length)
                {
                    _caretPosition = Text.Length;
                }

                if (modifiers.HasFlag(KeyModifiers.Shift))
                {
                    _shiftPressed = true;
                }

                if (key == Keys.Left)
                {
                    _caretPosition--;
                }
                else if (key == Keys.Right)
                {
                    _caretPosition++;
                }
                else if (key == Keys.Up)
                {
                    _caretPosition = 0;
                }
                else if (key == Keys.Down)
                {
                    _caretPosition = Text.Length;
                }

                if (_shiftPressed)
                {
                    _selectionOptions.SelectionEnd = _caretPosition;

                    return;
                }
                else if(key >= Keys.Up && key <= Keys.Right)
                {
                    _selectionOptions.SelectionStart = _caretPosition;
                    _selectionOptions.SelectionEnd = _caretPosition;
                }

                if (key == Keys.Backspace)
                {
                    _caretPosition--;

                    _caretPosition = _caretPosition < 0 ? 0 : _caretPosition;

                    if (selection.SelectionStart != selection.SelectionEnd)
                    {
                        DeleteSelection();
                    }
                    else if (_caretPosition < Text.Length)
                    {
                        Text = Text.Remove(_caretPosition, 1);
                    }
                }
                else if (key == Keys.Delete)
                {
                    if (selection.SelectionStart != selection.SelectionEnd)
                    {
                        DeleteSelection();
                    }
                    else
                    {
                        Text = Text.Remove(_caretPosition, 1);
                    }
                }
                else if (key == Keys.Tab)
                {
                    if (selection.SelectionStart != selection.SelectionEnd)
                    {
                        DeleteSelection();
                    }

                    Text = Text.Insert(_caretPosition, "    ");

                    _caretPosition += "    ".Length;
                }
                else if (key == Keys.Space)
                {
                    if (selection.SelectionStart != selection.SelectionEnd)
                    {
                        DeleteSelection();
                    }

                    Text = Text.Insert(_caretPosition, " ");
                    _caretPosition += " ".Length;
                }
                else
                {
                    return;
                }

                _selectionOptions.SelectionStart = _caretPosition;
                _selectionOptions.SelectionEnd   = _caretPosition;
            }
            finally
            {
                if (_recreateRenderer)
                {
                    InvalidateText();
                }

                Measure();

                Input?.Invoke(this, new InputEventArgs()
                {
                    Key = key,
                    Modifiers = modifiers,
                    InputMode = inputMode,
                });
            }
        }

        public void SetCaretOffset()
        {
            int position = _caretPosition;

            if (_caretPosition >= Text.Length)
            {
                position = Text.Length - 1;
            }

            var caret = _renderer.GetCaretInfo(position);

            float caretOffset = caret.CaretRectangle.Left;

            float viewWidth = Bounds.Width - Padding.Left - Padding.Right;

            if (caretOffset < _offset)
            {
                _offset = caretOffset - 5;
            }

            if (caretOffset > _offset + viewWidth)
            {
                _offset = caretOffset - viewWidth + 5;
            }

            _offset = _offset < 0 ? 0 : _offset;
        }

        public void OnLeaveInput()
        {
            IsInputGrabbed = false;
        }

        public void HandleText(string text)
        {
            DeleteSelection();

            _caretPosition = _caretPosition < 0 ? 0 : _caretPosition;

            Text = Text.Insert(_caretPosition, text);

            _caretPosition += text.Length;

            _selectionOptions.SelectionStart = _caretPosition;
            _selectionOptions.SelectionEnd   = _caretPosition;

            InvalidateText();

            Measure();
        }

        public void OnHover()
        {
            IsHovered = true;
        }
    }
}