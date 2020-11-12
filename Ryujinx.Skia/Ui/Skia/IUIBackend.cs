using System;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Ryujinx.Graphics.GAL;
using SkiaSharp;

namespace Ryujinx.Skia.Ui
{
    public unsafe interface IUIBackend : IDisposable
    {
        SKImageInfo SKImageInfo { get; }
        GRContext Context{ get; }

        SKSize Size { get; set; }
        void Initilize(Window* window);

        void InitializeSkiaSurface();

        void CreateGameResources();

        void CopyGameFramebuffer();

        SKSurface GetSurface();

        void ResetSkiaContext();

        void Draw();

        void Present();

        void SwitchContext(ContextType context);

        void SwapInterval(int value);

        IRenderer CreateRenderer();

        void ReleaseRenderer();
    }
}