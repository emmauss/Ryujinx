using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Ryujinx.Skia.Ui.Skia.Scene;
using SkiaSharp;
using SkiaSharp.Elements;
using SkiaSharp.Elements.Collections;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ryujinx.Skia.Ui.Skia.Widget
{
    public abstract class Layout : UIElement, IUICollection, IScrollable, IInput
    {
        private float _viewOffset;

        public int ContentSpacing { get; set; } = 20;

        public LayoutOptions LayoutOptions{ get; set; }

        private readonly ElementsController controller = new ElementsController();

        public event EventHandler<IInput.InputEventArgs> Input;

        public ElementsController Controller => controller;
        public ElementsCollection Elements => Controller.Elements;

        public Scrollbar Scrollbar { get; set; }
        public bool ScrollEnabled { get; set; } = true;
        public bool IsScrolling { get; set; }

        public SKSize ContentSize { get; set; }

        public SKColor BorderColor { get; set; } = SKColors.Transparent;

        public LayoutOptions AlignContent { get; set; }
        public bool IsInputGrabbed { get; set; }
        public Element FocussedInput { get; private set; }

        public Layout()
        {
            Scrollbar = new Scrollbar();
        }

        public override void AttachTo(Scene.Scene parent)
        {
            base.AttachTo(parent);

            foreach (var item in Elements)
            {
                if (item is UIElement element)
                {
                    element.AttachTo(ParentScene);
                }
            }

            Scrollbar.AttachTo(ParentScene);
            Scrollbar.Layout = this;
        }

        public override void Draw(SKCanvas canvas)
        {
            base.Draw(canvas);

            if (!DrawElement)
            {
                return;
            }

            if (DrawElement)
            {
                canvas.Save();
                canvas.DrawRect(Bounds, new SKPaint()
                {
                    Color = BackgroundColor,
                    IsStroke = false,
                    Style = SKPaintStyle.Fill
                });

                if (ScrollEnabled && Scrollbar.ScrollActive)
                {
                    Scrollbar.Draw(canvas);
                }

                canvas.ClipRect(Bounds, SKClipOperation.Intersect);

                if (_viewOffset != Scrollbar.OffsetValue)
                {
                    float diff = Math.Abs(Scrollbar.OffsetValue - _viewOffset);

                    if (_viewOffset < Scrollbar.OffsetValue)
                    {
                        _viewOffset += MathF.Round(diff / 2f);
                    }
                    else
                    {
                        _viewOffset -= MathF.Round(diff / 2f);
                    }

                    IManager.Instance.InvalidateMeasure();
                }

                switch (Scrollbar.Orientation)
                {
                    case Orientation.Vertical:
                        canvas.Translate(0, -_viewOffset);
                        break;
                    case Orientation.Horizontal:
                        canvas.Translate(-_viewOffset, 0);
                        break;
                }

                DrawController(canvas);

                canvas.Restore();
            }
        }

        public void DrawController(SKCanvas canvas)
        {
            for (int i = 0; i < Controller.Elements.Count; i++)
            {
                Element element = Controller.Elements[i];

                element.Draw(canvas);
            }
        }

        public virtual void HandleMouse(SKPoint position, MouseState state, MouseState lastState, OpenTK.Mathematics.Vector2 wheel)
        {
            if (!ScrollEnabled)
            {
                IsScrolling = false;
            }

            if (ScrollEnabled && Scrollbar.IsPointInside(position))
            {
                if (state.IsButtonDown(MouseButton.Button1))
                {
                    IsScrolling = true;

                    Scrollbar.OnSelect();

                    ScrollTo(position);

                    IManager.Instance.InvalidateMeasure();
                }
            }

            if (IsScrolling)
            {
                if (state.IsButtonDown(MouseButton.Button1))
                {
                    IsScrolling = true;

                    Scrollbar.OnSelect();

                    ScrollTo(position);

                    IManager.Instance.InvalidateMeasure();
                }
                else
                {
                    IsScrolling = false;

                    if (Scrollbar.IsPointInside(position))
                    {
                        Scrollbar.OnHover();
                    }
                }

                return;
            }

            if (wheel.Length > 0)
            {
                float scrollValue;

                float contentSize;

                if (this.Scrollbar.Orientation == Orientation.Horizontal)
                {
                    scrollValue = wheel.X;

                    contentSize = ContentSize.Width;
                }
                else
                {
                    scrollValue = -wheel.Y;

                    contentSize = ContentSize.Height;
                }

                scrollValue *= Scrollbar.PageSize / contentSize * Scrollbar.Length / 2;

                ScrollTo(scrollValue);

                IManager.Instance.InvalidateMeasure();

                return;
            }

            var originalPostion = position;

            switch (Scrollbar.Orientation)
            {
                case Orientation.Vertical:
                    position.Y += Scrollbar.OffsetValue;
                    break;
                case Orientation.Horizontal:
                    position.X += Scrollbar.OffsetValue;
                    break;
            }

            Element element = GetElementAtPosition(position);

            InputMode mode = InputMode.None;

            if (state.IsButtonDown(MouseButton.Button1) && !lastState.IsButtonDown(MouseButton.Button1))
            {
                mode = InputMode.MouseDown;
            }
            else if (state.IsButtonDown(MouseButton.Button1) && lastState.IsButtonDown(MouseButton.Button1))
            {
                mode = InputMode.MousePress;
            }
            else if (!state.IsButtonDown(MouseButton.Button1) && lastState.IsButtonDown(MouseButton.Button1))
            {
                mode = InputMode.MouseUp;
            }

            if (mode != InputMode.None)
            {
                if (FocussedInput != null)
                {
                    if (!FocussedInput.IsPointInside(position))
                    {
                        (FocussedInput as IInput).OnLeaveInput();

                        FocussedInput = null;
                    }
                }
            }

            if (element != null)
            {
                if (element is Layout layout)
                {
                    layout.HandleMouse(position, state, lastState, wheel);
                    return;
                }
                else if (element is UIElement uiElement)
                {
                    if (!uiElement.IsActive)
                    {
                        return;
                    }

                    if (!state.IsButtonDown(MouseButton.Button2) && lastState.IsButtonDown(MouseButton.Button2))
                    {
                        uiElement.ShowContextMenu(originalPostion);

                        return;
                    }

                    if (element is IHoverable hoverElement)
                    {
                        ParentScene.HoveredElement = uiElement;

                        hoverElement.OnHover();
                    }
                    if (element is ISelectable selectElement && state.IsButtonDown(MouseButton.Button1))
                    {
                        selectElement.OnSelect();
                    }
                    if (element is IAction actionElement && !state.IsButtonDown(MouseButton.Button1) && lastState.IsButtonDown(MouseButton.Button1))
                    {
                        actionElement.OnActivate();
                    }

                    if (FocussedInput != element && mode != InputMode.None)
                    {
                        (FocussedInput as IInput)?.OnLeaveInput();

                        FocussedInput = null;
                    }

                    if (element is IInput input)
                    {
                        if (mode != InputMode.None)
                        {
                            FocussedInput = uiElement;

                            input.OnGrabInput();

                            input.HandleMouse(position, mode);
                        }
                    }
                }
            }
        }

        public override void Measure()
        {
            if (ScrollEnabled)
            {
                Scrollbar.Length = (int)(Scrollbar.Orientation == Orientation.Horizontal ? Bounds.Width : Bounds.Height);

                var padding = Padding;
                SKPoint scrollbarPosition = Bounds.Location;
                switch (Scrollbar.Orientation)
                {
                    case Orientation.Vertical:
                        padding.Right = padding.Right < Scrollbar.ScrollbarWidth + 10 ? Scrollbar.ScrollbarWidth + 10 : padding.Right;
                        scrollbarPosition.X = Bounds.Right - Scrollbar.ScrollbarWidth;
                        break;
                    case Orientation.Horizontal:
                        padding.Bottom = padding.Bottom < Scrollbar.ScrollbarWidth + 10 ? Scrollbar.ScrollbarWidth + 10 : padding.Bottom;
                        scrollbarPosition.Y = Bounds.Bottom - Scrollbar.ScrollbarWidth;
                        break;
                }
                Padding = padding;
                Scrollbar.Location = scrollbarPosition;
            }
        }

        public void AddElement(UIElement element)
        {
            if (ParentScene != null)
            {
                element.AttachTo(ParentScene);
            }
            Elements.Add(element);
        }

        public void Add(Element element)
        {
            Elements.Add(element);
        }

        public Element GetElementAtPosition(SKPoint position)
        {
            return Elements.GetElementAtPoint(position);
        }

        public abstract void OnLayout();

        public void ScrollTo(SKPoint position)
        {
            float offset = 0;

            switch (Scrollbar.Orientation)
            {
                case Orientation.Horizontal:
                    offset = Math.Clamp(position.X, Scrollbar.Bounds.Left, Scrollbar.Bounds.Right) - Scrollbar.Bounds.Left;
                    break;
                case Orientation.Vertical:
                    offset = Math.Clamp(position.Y, Scrollbar.Bounds.Top, Scrollbar.Bounds.Bottom) - Scrollbar.Bounds.Top;
                    break;
            }

            Scrollbar.Value = offset;
        }

        public void ScrollTo(float offset)
        {
            SKPoint position = default;

            Scrollbar.ClampValue();

            if(offset > Scrollbar.PageSize)
            {
                offset = Scrollbar.PageSize;
            }

            offset = Scrollbar.Value + offset;

            switch (Scrollbar.Orientation)
            {
                case Orientation.Horizontal:
                    position = new SKPoint(Math.Clamp(offset + Scrollbar.Bounds.Left, Scrollbar.Bounds.Left, Scrollbar.Bounds.Right), 0);
                    break;
                case Orientation.Vertical:
                    position = new SKPoint(0, Math.Clamp(offset + Scrollbar.Bounds.Top, Scrollbar.Bounds.Top, Scrollbar.Bounds.Bottom));
                    break;
            }

            ScrollTo(position);
        }

        public void UpdateScrollbars()
        {
            switch (Scrollbar.Orientation)
            {
                case Orientation.Horizontal:
                    Scrollbar.ContentSize = ContentSize.Width;
                    Scrollbar.PageSize = Bounds.Width;
                    break;
                case Orientation.Vertical:
                    Scrollbar.ContentSize = ContentSize.Height;
                    Scrollbar.PageSize = Bounds.Height;
                    break;
            }

            Scrollbar.Measure();
        }

        public override void Dispose()
        {
            base.Dispose();

            foreach (var element in Elements)
            {
                (element as IDisposable)?.Dispose();

                if (element is SkiaSharp.Elements.Image image)
                {
                    image.Bitmap.Dispose();
                }
            }

            Elements.Clear();
        }

        public void OnGrabInput()
        {
            IsInputGrabbed = true;
        }

        public virtual void HandleKeyboard(KeyboardState keyboard, KeyModifiers modifiers)
        {
            if (FocussedInput != null)
            {
                if (FocussedInput is Layout input)
                {
                    input.HandleKeyboard(keyboard, modifiers);
                    return;
                }

                long elapsed = Scene.Scene.InputTimer.ElapsedMilliseconds;

                if (elapsed - Scene.Scene.LastKeyInput < 100)
                {
                    return;
                }

                Scene.Scene.LastKeyInput = elapsed;

                Keys pressedkey = default;

                InputMode mode = InputMode.Keyboard;

                foreach (Keys key in Enum.GetValues(typeof(Keys)))
                {
                    if (key == Keys.Unknown)
                    {
                        continue;
                    }

                    switch (key)
                    {
                        case Keys.LeftControl:
                        case Keys.RightControl:
                            if (keyboard.IsKeyDown(key))
                            {
                                modifiers |= KeyModifiers.Control;
                            }
                            break;
                        case Keys.LeftShift:
                        case Keys.RightShift:
                            if (keyboard.IsKeyDown(key))
                            {
                                modifiers |= KeyModifiers.Shift;
                            }
                            break;
                        case Keys.LeftAlt:
                        case Keys.RightAlt:
                            if (keyboard.IsKeyDown(key))
                            {
                                modifiers |= KeyModifiers.Alt;
                            }
                            break;
                        case Keys.LeftSuper:
                        case Keys.RightSuper:
                            if (keyboard.IsKeyDown(key))
                            {
                                modifiers |= KeyModifiers.Super;
                            }
                            break;
                        case Keys.NumLock:
                            if (keyboard.IsKeyDown(key))
                            {
                                modifiers |= KeyModifiers.NumLock;
                            }
                            break;
                        case Keys.CapsLock:
                            if (keyboard.IsKeyDown(key))
                            {
                                modifiers |= KeyModifiers.CapsLock;
                            }
                            break;
                        default:
                            if (keyboard.IsKeyDown(key))
                            {
                                pressedkey = key;
                            }
                            break;
                    }
                }

                (FocussedInput as IInput).HandleKeyboard(pressedkey, modifiers, mode);
            }
        }

        public void OnLeaveInput()
        {
            IsInputGrabbed = false;

            (FocussedInput as IInput)?.OnLeaveInput();
        }

        public void HandleMouse(SKPoint position, InputMode inputMode)
        {
        }

        public void HandleKeyboard(Keys key, KeyModifiers modifiers, InputMode inputMode)
        {
        }

        public void HandleText(string text)
        {
            if (FocussedInput != null)
            {
                (FocussedInput as IInput)?.HandleText(text);
            }
        }
    }
}
