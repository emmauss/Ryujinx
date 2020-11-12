using OpenTK.Mathematics;
using Ryujinx.Skia.Ui.Skia.Widget;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ryujinx.Skia.Ui.Skia.Scene
{
    interface IManager
    {
        public static IManager Instance { get; private set; }

        public static void SetCurrentDirector(IManager director)
        {
            Instance = director;
        }

        event EventHandler Resized;

        SKRect Bounds { get; }

        void NavigateTo(Scene scene);
        void Back();
        void ClearStage();

        void StartGame();

        Vector2i GetStageSize();
        void InvalidateMeasure();

        void SetCursorMode(CursorMode cursorMode);
    }
}
