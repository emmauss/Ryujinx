using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Ryujinx.Configuration;
using Ryujinx.Skia.Ui.Skia.Scene;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace Ryujinx.Skia.Ui
{
    public unsafe partial class SKWindow : GameWindow
    {
        public static Vector2i BaseSize = new Vector2i(1280, 720);

        private Vector2 _scale = Vector2.One;

        private static int _targetFps = 60;

        private static long _ticksPerFrame;

        private long _ticks = 0;

        private bool _recreateContext;

        private System.Diagnostics.Stopwatch _chrono;

        private long _lastTime;

        private Vector2 _currentMouseWheel;
        private Vector2 _lastMouseWheel;

        public bool IsActive { get; set; }

        public new Vector2i Size { get; set; }

        public const SKColorType ColorType = SKColorType.Rgba8888;

        public string _lastTextInput;

        private SKSurface _surface;

        public bool Suspend { get; set; }

        private Thread _mainRenderThread;

        private KeyModifiers _modifiers = default;
        private bool _mousePressed;
        private bool _shouldGameMeasure;

        private new MouseState LastMouseState { get; set; }
        private new KeyboardState LastKeyboardState { get; set; }

        public SKWindow(int width, int height) : base(
            new GameWindowSettings()
            {
                IsMultiThreaded = true,
                RenderFrequency = 60,
                UpdateFrequency = 30
            },
            new NativeWindowSettings()
            {
                API = ContextAPI.OpenGL,
                APIVersion = new Version(3, 3),
                Profile = ContextProfile.Core,
                Flags = ContextFlags.ForwardCompatible,
                AutoLoadBindings = false,
                Size = new Vector2i(width, height),
                Title = $"Skia Test",
            }
        )
        {
            UiBackend = (IUIBackend)new OpenGlBackend(3, 3);

            _scenes = new Stack<Scene>();

            _activeScene = null;

            _ticksPerFrame = System.Diagnostics.Stopwatch.Frequency / TargetFps;

            Size = new Vector2i(width, height);

            IManager.SetCurrentDirector(this);

            _scale = new Vector2(1, 1);

            GLFW.SetInputMode(this.WindowPtr, LockKeyModAttribute.LockKeyMods, true);

            this.MouseWheel += SKWindow_MouseWheel;
            this.TextInput += SKWindow_TextInput;

            KeyDown += Key_Down;
            KeyUp += Key_Up;
            LastMouseState = MouseState;
            LastKeyboardState = KeyboardState;
        }

        private void SKWindow_TextInput(TextInputEventArgs obj)
        {
            _lastTextInput = obj.AsString;
        }

        private void SKWindow_MouseWheel(MouseWheelEventArgs obj)
        {
            _currentMouseWheel = obj.Offset;
        }

        private void Key_Up(KeyboardKeyEventArgs obj)
        {
            _modifiers = obj.Modifiers;
        }

        private void Key_Down(KeyboardKeyEventArgs obj)
        {
            _modifiers = obj.Modifiers;
        }

        protected unsafe override void OnLoad()
        {
            UiBackend.Present();

            UiBackend.InitializeSkiaSurface();

            UiBackend.CreateGameResources();

            _recreateContext = false;
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
        }

        public unsafe void Start()
        {
            IsActive = true;

            _chrono = new System.Diagnostics.Stopwatch();
            _chrono.Start();
            _lastTime = 0;

            UiBackend.Size = new SKSize(Size.X, Size.Y);

            UiBackend.Initilize(this.WindowPtr);

            UiBackend.SwitchContext(ContextType.None);

            _mainRenderThread = new Thread(Render)
            {
                Name = "GUI.RenderLoop"
            };
            _mainRenderThread.Start();

            Thread.CurrentThread.Name = "GUI.MainThread";

            MainLoop();

            _mainRenderThread.Join();
        }

        public void SwapInterval(int interval)
        {
            UiBackend.SwapInterval(interval);
        }

        public unsafe void Render()
        {
            UiBackend.SwitchContext(ContextType.Main);

            OnLoad();

            SwapInterval(0);

            while (IsActive)
            {
                var scene = ActiveScene;

                var gamescene = scene as GameScene;

                long elapsed = _chrono.ElapsedTicks;

                _ticks += elapsed - _lastTime;

                _lastTime = elapsed;

                gamescene?.ProcessFrame(this);

                if(gamescene == null)
                {
                    // Spin waiting is expensive
                    Thread.Sleep(5);
                }

                if (_ticks >= _ticksPerFrame)
                {
                    gamescene?.Render(this);

                    SwapBuffers();

                    _ticks = Math.Min(_ticks - _ticksPerFrame, _ticksPerFrame);
                }
            }

            GLFW.MakeContextCurrent(null);

            UiBackend.Dispose();
        }

        public void RenderUI()
        {
            if (_recreateContext)
            {
                _recreateContext = false;

                UiBackend.InitializeSkiaSurface();
            }

            lock (this)
            {
                DrawCanvas();
            }
        }

        public void CopyGameFramebuffer()
        {
            UiBackend.CopyGameFramebuffer();
        }

        public void MainLoop()
        {
            while (IsActive)
            {
                UpdateFrame();

                // Polling becomes expensive if it's not slept
                Thread.Sleep(1);
            }
        }

        public new void UpdateFrame()
        {
            ProcessEvents();

            Suspend = GLFW.GetWindowAttrib(this.WindowPtr, WindowAttributeGetBool.Iconified);

            _scale = new Vector2(1, 1);

            var mouse = this.MousePosition;
            var mouseState = MouseState.GetSnapshot();
            var lastMouseState = LastMouseState.GetSnapshot();

            LastMouseState = mouseState;

            var keyboardState = this.KeyboardState.GetSnapshot();
            var lastKeyboardState = LastKeyboardState.GetSnapshot();
            var wheel = _currentMouseWheel - _lastMouseWheel;
            _lastMouseWheel = _currentMouseWheel;

            LastKeyboardState = keyboardState;

            Scene scene = ActiveScene;

            if (scene is GameScene gameScene)
            {
                gameScene.Update(this);

                _shouldGameMeasure = !_shouldGameMeasure;
            }


            var isFocused = GLFW.GetWindowAttrib(this.WindowPtr, WindowAttributeGetBool.Focused);

            lock (this)
            {
                if ((scene is GameScene && _invalidateMeasure && _shouldGameMeasure) || !(scene is GameScene))
                {
                    Measure();
                }

                if (isFocused)
                {
                    HandleMouse(mouse * _scale, mouseState, lastMouseState, wheel);
                    HandleKeyboard(keyboardState, lastKeyboardState, _modifiers);
                    HandleText(_lastTextInput);
                }
            }

            _lastTextInput = string.Empty;
        }

        public new bool IsFocused => GLFW.GetWindowAttrib(this.WindowPtr, WindowAttributeGetBool.Focused);

        public static int TargetFps
        {
            get => _targetFps; set
            {
                _targetFps = value;

                _ticksPerFrame = System.Diagnostics.Stopwatch.Frequency / TargetFps;
            }
        }

        public static IUIBackend UiBackend { get; set; }

        public void DrawCanvas()
        {
            _surface?.Dispose();

            UiBackend.ResetSkiaContext();

            using (_surface = UiBackend.GetSurface())
            {
                if (_surface == null)
                {
                    return;
                }

                using var canvas = _surface.Canvas;

                canvas.Scale(_scale.X, _scale.Y);

                Draw(canvas);

                canvas.Flush();
            }
        }

        public new void SwapBuffers()
        {
            if (!Suspend)
            {
                UiBackend.SwitchContext(ContextType.Main);

                RenderUI();

                UiBackend.Draw();

                UiBackend.Present();
            }
        }

        protected unsafe override void OnResize(ResizeEventArgs e)
        {
            lock (this)
            {
                Size = new Vector2i(e.Width, e.Height);

                _recreateContext = true;

                UiBackend.Size = new SKSize(Size.X, Size.Y);

                ActiveScene?.DismissPopup();

                Resized?.Invoke(this, null);
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            IsActive = false;

            _mainRenderThread.Join();

            lock (this)
            {
                while (_scenes.TryPop(out Scene scene))
                {
                    scene.Dispose();
                }
            }

            _scenes.Clear();

            _activeScene = null;

            base.OnClosing(e);
        }
    }
}