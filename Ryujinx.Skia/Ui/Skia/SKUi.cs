using OpenTK.Mathematics;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Ryujinx.Skia.Ui.Skia.Scene;
using Ryujinx.Skia.Ui.Skia.Widget;
using SkiaSharp;
using SkiaSharp.Elements;
using SkiaSharp.Elements.Collections;
using SkiaSharp.Views.Desktop;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Ryujinx.Skia.Ui
{
    public partial class SKWindow : IManager
    {
        private Scene _activeScene;

        private readonly Stack<Scene> _scenes;

        private bool _invalidateMeasure;

        public event EventHandler Resized;

        SKRect IManager.Bounds => SKRect.Create(Size.X, Size.Y);

        private CursorMode _cursorMode = CursorMode.Default;

        public Element GetElementAtPoint(SKPoint point)
        {
            var activeScene = ActiveScene;

            return activeScene?.Elements.GetElementAtPoint(point);
        }

        public void SuspendLayout() => _activeScene?.Controller.SuspendLayout();

        public void ResumeLayout(bool invalidate = false) => _activeScene.Controller.ResumeLayout(invalidate);

        public void Draw(SKCanvas canvas)
        {
            lock (this)
            {
                if (!IManager.Instance.Bounds.Size.IsEmpty)
                {
                    var activeScene = ActiveScene;

                    if (activeScene.DrawUi)
                    {
                        activeScene?.Draw(canvas);
                    }
                    else
                    {
                        activeScene?.DrawBlank(canvas);
                    }
                }
            }
        }

        public void NavigateTo(Scene scene)
        {
            lock (_scenes)
            {
                ActiveScene?.OnNavigatedFrom();

                _scenes.Push(scene);

                _activeScene = scene;

                scene.OnNavigatedTo();

                InvalidateMeasure();
            }
        }

        public void InvalidateMeasure()
        {
            _invalidateMeasure = true;
        }

        public void Back()
        {
            lock (_scenes)
            {
                _scenes.TryPop(out var scene);

                if(_scenes.Count <= 0)
                {
                    NavigateTo(new MainScene());

                    _activeScene = _scenes.Peek();
                }
                else
                {
                    _activeScene = _scenes.Peek();

                    _activeScene.OnNavigatedTo();
                }

                InvalidateMeasure();
            }
        }

        public void ClearStage()
        {
            lock (_scenes)
            {
                _scenes.Clear();
                _scenes.Push(new DummyScene());
                _activeScene = _scenes.Peek();
                InvalidateMeasure();
            }
        }

        public Vector2i GetStageSize()
        {
            return Size;
        }

        public Scene ActiveScene => _activeScene;

        public void Measure()
        {
            if (!IManager.Instance.Bounds.Size.IsEmpty)
            {
                if (_invalidateMeasure && !Suspend)
                {
                    _invalidateMeasure = false;

                    Scene scene = ActiveScene;

                    scene?.Measure();

                    scene?.MeasureOverlays();
                }
            }
        }

        public void HandleMouse(Vector2 mouse, MouseState state, MouseState lastState,Vector2 wheel)
        {
            SKPoint point = new SKPoint(mouse.X, mouse.Y);

            if(state.Position != state.PreviousPosition)
            {
                InvalidateMeasure();
            }

            ActiveScene?.HandleMouse(point, state, lastState, wheel);
        }

        public void HandleText(string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                ActiveScene?.HandleText(text);
            }
        }

        public void HandleKeyboard(KeyboardState keyboard, KeyboardState lastState, KeyModifiers modifiers)
        {
            if (keyboard.IsAnyKeyDown)
            {
                InvalidateMeasure();
            }

            ActiveScene?.HandleKeyboard(keyboard, lastState, modifiers);
        }

        public void SetCursorMode(CursorMode cursorMode)
        {
            if (cursorMode != _cursorMode)
            {
                switch (cursorMode)
                {
                    case CursorMode.Default:
                        Cursor = MouseCursor.Default;
                        break;
                    case CursorMode.Insertion:
                        Cursor = MouseCursor.IBeam;
                        break;
                }
            }

            _cursorMode = cursorMode;
        }

        public virtual void StartGame()
        {
        }
    }
}