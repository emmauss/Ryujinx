using System;

namespace Ryujinx.Graphics.Gal
{
    public unsafe interface IGalRenderer : IDisposable
    {
        void QueueAction(Action ActionMthd);

        void RunActions();

        IGalBlend Blend { get; }

        IGalFrameBuffer FrameBuffer { get; }

        IGalRasterizer Rasterizer { get; }

        IGalShader Shader { get; }

        IGalTexture Texture { get; }
    }
}