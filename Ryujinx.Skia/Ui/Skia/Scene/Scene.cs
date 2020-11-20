using DequeNet;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Ryujinx.Skia.Ui.Skia.Widget;
using SkiaSharp;
using SkiaSharp.Elements;
using SkiaSharp.Elements.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Ryujinx.Skia.Ui.Skia.Scene
{
    public abstract class Scene : IScene, IUICollection, IDisposable
    {
        public static Stopwatch InputTimer;

        public static long LastKeyInput;

        private UIElement _hoveredElement;
        private UIElement _focussedInput;
        public int TextSize { get; set; } = 16;

        private readonly ElementsController controller = new ElementsController();

        public ElementsController Controller => controller;
        public ElementsCollection Elements => Controller.Elements;

        public bool IsSelfRendering { get; set; }
        public Animation Animator { get; set; }
        public Theme Theme { get; protected set; }

        public Deque<IModal> Modals { get; set; }

        public IPopup ActivePopup { get; set; }

        protected ManualResetEvent _resetEvent;

        public bool DrawUi { get; set; } = true;

        public bool Loaded { get; set; }
        public UIElement HoveredElement { get => _hoveredElement; set => _hoveredElement = value; }
        public UIElement FocussedInput { get => _focussedInput; set => _focussedInput = value; }

        public Scene()
        {
            Animator = new Animation();
            Theme = Themes.Light;

            Modals = new Deque<IModal>();
            _resetEvent = new ManualResetEvent(false);

            SKWindow.TargetFps = 30;

            InputTimer = Stopwatch.StartNew();
        }

        public void DrawController(SKCanvas canvas)
        {
            for (int i = 0; i < Controller.Elements.Count; i++)
            {
                Element element = Controller.Elements[i];

                element.Draw(canvas);
            }
        }

        public void DrawBlank(SKCanvas canvas)
        {
            using SKPaint paint = new SKPaint() { Color = Theme.SceneBackgroundColor, Style = SKPaintStyle.Fill };
            canvas.Clear(Theme.SceneBackgroundColor);
            var bounds = IManager.Instance.Bounds;
            canvas.DrawRect(bounds, paint);
        }

        public virtual void Draw(SKCanvas canvas)
        {
            Controller.BackgroundColor = Theme.SceneBackgroundColor;
            canvas.Clear(Theme.SceneBackgroundColor);
            DrawController(canvas);

            DrawMisc(canvas);

            Deque<IModal> drawn = new Deque<IModal>();

            if (Modals.Count > 0)
            {
                lock (Modals)
                {
                    using SKPaint modalBackdropPaint = new SKPaint() { Color = Theme.ModalBackdropColor, Style = SKPaintStyle.Fill };

                    canvas.DrawRect(IManager.Instance.Bounds, modalBackdropPaint);

                    while (Modals.Count > 0)
                    {
                        var modal = Modals.PopLeft();
                        modal.Draw(canvas);

                        drawn.PushRight(modal);
                    }
                }
            }

            Modals = drawn;

            if (ActivePopup != null)
            {
                if (ActivePopup is UIElement popup)
                {
                    try
                    {
                        popup.Draw(canvas);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        public virtual void DrawMisc(SKCanvas canvas)
        {

        }

        public virtual void HandleText(string text)
        {
            if (FocussedInput != null)
            {
                (FocussedInput as IInput)?.HandleText(text);
            }
        }

        public virtual void HandleKeyboard(KeyboardState keyboard, KeyboardState lastState, KeyModifiers modifiers)
        {
            if (FocussedInput != null)
            {
                if (FocussedInput is Layout input)
                {
                    input.HandleKeyboard(keyboard, modifiers);

                    goto Toggle;
                }

                long elapsed = InputTimer.ElapsedMilliseconds;

                if (elapsed - LastKeyInput < 100)
                {
                    goto Toggle;
                }

                LastKeyInput = elapsed;

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

            Toggle:

            if (!keyboard.IsKeyDown(Keys.GraveAccent) && lastState.IsKeyDown(Keys.GraveAccent))
            {
                DrawUi = !DrawUi;
            }
        }

        public void ShowModal(IModal modal)
        {
            lock (Modals)
            {
                Modals.PushRight(modal);
            }

            IManager.Instance.InvalidateMeasure();
        }

        public void ShowPopup(IPopup popup)
        {
            DismissPopup();

            ActivePopup = popup;
        }

        public void DismissPopup()
        {
            ActivePopup?.Dismiss();

            ActivePopup = null;
        }

        public void DismissModal()
        {
            lock (Modals)
            {
                Modals.PopRight();
            }

            IManager.Instance.InvalidateMeasure();
        }

        public virtual void HandleMouse(SKPoint position, MouseState state, MouseState lastState, OpenTK.Mathematics.Vector2 wheel)
        {
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

            if (ActivePopup != null)
            {
                var popup = ActivePopup as UIElement;

                if (popup != null && popup.IsPointInside(position))
                {
                    ActivePopup.HandleMouse(position, mode);
                }
                else if (mode == InputMode.MouseUp)
                {
                    DismissPopup();
                }
                else
                {
                    popup?.ResetState();
                }

                return;
            }

            Element element = null;

            if (HoveredElement != null && wheel.Length == 0)
            {
                if (HoveredElement.IsPointInside(position))
                {
                    element = HoveredElement;
                }
                else
                {
                    if (HoveredElement is IHoverable hoverable)
                    {
                        hoverable.IsHovered = false;

                        IManager.Instance.SetCursorMode(CursorMode.Default);

                        HoveredElement = null;
                    }
                }
            }
            if (element == null)
            {
                lock (Modals)
                {
                    if (Modals.Count > 0)
                    {
                        var modal = Modals.PeekRight();

                        element = modal.GetElementAtPosition(position);

                        goto element;
                    }
                }

                element = GetElementAtPosition(position);
            }

        element:

            if (element is Layout layout)
            {
                layout.HandleMouse(position, state, lastState, wheel);

                if (state.IsButtonDown(MouseButton.Button1))
                {
                    FocussedInput = element as UIElement;
                }
                return;
            }

            if (element != null && element is UIElement uiElement)
            {
                if (!uiElement.IsActive)
                {
                    return;
                }

                if (!state.IsButtonDown(MouseButton.Button2) && lastState.IsButtonDown(MouseButton.Button2))
                {
                    uiElement.ShowContextMenu(position);

                    return;
                }

                if (element is IHoverable hoverElement)
                {
                    HoveredElement = uiElement;

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

        public virtual void Measure()
        {
        }

        public void MeasureOverlays()
        {
            lock (Modals)
            {
                var _modals = Modals;

                for(int i = 0; i < _modals.Count; i++)
                {
                    IModal modal = _modals[i];
                    (modal as UIElement).Measure();
                }
            }

            if (ActivePopup != null)
            {
                if (ActivePopup is ContextMenu contextMenu)
                {
                    contextMenu?.AttachedElement?.LayoutContextMenu();
                }
                else if (ActivePopup is UIElement element)
                {
                    element.Measure();
                }
            }
        }

        public void AddElement(UIElement element)
        {
            element.AttachTo(this);
            Elements.Add(element);
            IManager.Instance?.InvalidateMeasure();
        }

        public virtual Element GetElementAtPosition(SKPoint position)
        {
            return Elements.GetElementAtPoint(position);
        }

        public virtual void Dispose()
        {
            foreach(var element in Elements)
            {
                (element as IDisposable)?.Dispose();

                if(element is SkiaSharp.Elements.Image image)
                {
                    image.Bitmap.Dispose();
                }
            }

            Elements.Clear();
        }

        public virtual void OnNavigatedTo(){

        }

        public virtual void OnNavigatedFrom(){

        }
    }
}
