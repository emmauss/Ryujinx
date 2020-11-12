using OpenTK.Mathematics;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SkiaSharp;
using SkiaSharp.Elements;
using SkiaSharp.Elements.Collections;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ryujinx.Skia.Ui.Skia.Scene
{
    public interface IScene
    { 
        void Measure();
        void HandleMouse(SKPoint position, MouseState state, MouseState lastState, Vector2 wheel);
        void HandleKeyboard(KeyboardState keyboard, KeyboardState lastState, KeyModifiers modifiers);
        void HandleText(string text);
        void Draw(SKCanvas canvas);
    }
}
