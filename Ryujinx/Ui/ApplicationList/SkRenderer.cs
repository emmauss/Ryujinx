using Gtk;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using OpenTK;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.Gtk;
using System;
using System.Threading;

namespace Ryujinx.Ui
{
    public class SkRenderer : GLWidget
    {
        public const SKColorType ColorType = SKColorType.Rgba8888;
        public event EventHandler<DrawEventArgs> DrawObjects;
        public event EventHandler Resized;

        private readonly SKColor _backgroundColor;
        private GRGlInterface _interface;
        private GRContext _contextOpenGL;
        private int _rbo;
        private GRBackendRenderTarget _renderTarget;
        private ManualResetEvent _resetEvent;
        private Thread _rendererThread;

        private SKSurface _surface;
        private SKImageInfo _info;

        private int _frameBuffer;
        private int _texture;
        private int _targetFps = 15;

        public bool _initialized;
        public bool _resized;
        public bool IsRendering { get; set; }

        public SkRenderer(SKColor backgroundColor) : base(GetGraphicsMode(),
            3, 3,
            GraphicsContextFlags.ForwardCompatible)
        {
            _backgroundColor = backgroundColor;

            this.Destroyed += Renderer_Destroyed;

            this.Initialized += Renderer_Initialized;

            this.IsRenderHandler = true;

            OpenTK.Toolkit.Init();

            _resetEvent = new ManualResetEvent(false);

            // this.RenderFrame += Render_Frame;

            _rendererThread = new Thread(RenderLoop)
            {
                Name = "UIRenderingThread"
            };
        }

        private static GraphicsMode GetGraphicsMode()
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                var mode = new GraphicsMode(new ColorFormat(24));
            }

            return new GraphicsMode(new ColorFormat());
        }

        public void RenderLoop()
        {
            var chrono = new System.Diagnostics.Stopwatch();
            chrono.Start();

            int timeToWait = 16;

            while (IsRendering)
            {
                _resetEvent.WaitOne(timeToWait);
                _resetEvent.Reset();

                long currentTickCount = chrono.ElapsedTicks;

                if (!IsRendering)
                {
                    return;
                }

                Render();

                long timeSpentMs = (chrono.ElapsedTicks - currentTickCount) / (System.Diagnostics.Stopwatch.Frequency / 1000);

                int nextFrameTime = 1000 / _targetFps;

                timeToWait = (int)Math.Max(nextFrameTime - timeSpentMs, 0);
            }
        }

        private void Render()
        {
            if(!_initialized || !IsRendering)
            {
                return;
            }

            if(_resized)
            {
                _resized = false;

                ResizeRenderTarget();
            }

            lock (GraphicsContext)
            {
                GraphicsContext.MakeCurrent(WindowInfo);

                _surface?.Dispose();
                _renderTarget?.Dispose();

                _surface = null;
                _renderTarget = null;

                using (_surface = GetSurface())
                {
                    var canvas = _surface.Canvas;

                    RenderView(canvas);

                    canvas.Flush();

                    DrawBuffers();

                    if(!IsRendering)
                    {
                        return;
                    }

                    SwapBuffers();

                    canvas.Dispose();
                }

                DeleteBuffers();

                GraphicsContext.MakeCurrent(null);
            }
        }

        private void Renderer_Initialized(object sender, EventArgs e)
        {
            GraphicsContext.MakeCurrent(WindowInfo);

            Initialize();

            QueueRender();
        }

        public void QueueRender()
        {
            if (GraphicsContext != null && IsRendering)
            {
                if (_rendererThread.ThreadState == ThreadState.Unstarted)
                {
                    _rendererThread.Start();
                }

                _targetFps = 60;
            }
        }

        protected override bool OnConfigureEvent(Gdk.EventConfigure evnt)
        {
            var result = base.OnConfigureEvent(evnt);

            _resized = true;
            QueueRender();

            return result;
        }

        private void RenderView(SKCanvas canvas)
        {
            if(!_initialized)
            {
                Initialize();
                _initialized = true;
            }

            DrawSurface(canvas);
        }

        private void DeleteBuffers()
        {
            GL.DeleteFramebuffer(_frameBuffer);
            GL.DeleteTexture(_texture);
            GL.DeleteRenderbuffer(_rbo);
        }

        private void DrawSurface(SKCanvas canvas)
        {
            DrawEventArgs drawEvent = new DrawEventArgs()
            {
                Canvas = canvas
            };

            DrawObjects?.Invoke(this, drawEvent);

            _targetFps = 15;

            if(drawEvent.QueueRender)
            {
                QueueRender();
            }
        }

        public void Initialize()
        {
            if (_initialized)
            {
                return;
            }
            _interface = GRGlInterface.CreateNativeGlInterface();
            _contextOpenGL = GRContext.Create(GRBackend.OpenGL, _interface);

            _initialized = true;

            GraphicsContext.SwapInterval = 0;

            GraphicsContext.MakeCurrent(null);

            IsRendering = true;

            _resetEvent.Set();
        }

        public SKSurface GetSurface()
        {
            _surface?.Dispose();
            _renderTarget?.Dispose();

            Gdk.Monitor monitor = Display.GetMonitorAtWindow(Window);

            _info = new SKImageInfo(AllocatedWidth * monitor.ScaleFactor, AllocatedHeight * monitor.ScaleFactor, ColorType);

            DeleteBuffers();

            _renderTarget = CreateRenderTarget(_info, _contextOpenGL, out _frameBuffer, out _texture, out _rbo);

            Bind();

            return SKSurface.Create(_contextOpenGL, _renderTarget, GRSurfaceOrigin.BottomLeft, ColorType);
        }

        public void Bind()
        {
            GL.BindFramebuffer( FramebufferTarget.Framebuffer, _frameBuffer);
        }

        public void Unbind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void DrawBuffers()
        {
            Unbind();
            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, _frameBuffer);
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);

            GL.BlitFramebuffer(0, 0, _info.Width, _info.Height, 0, 0, _info.Width, _info.Height, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Nearest);
            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, 0);
        }

        public void SwapBuffers()
        {
            OpenTK.Graphics.GraphicsContext.CurrentContext.SwapBuffers();
        }

        public void ResizeRenderTarget()
        {
            if(!_initialized)
            {
                return;
            }
            _surface?.Dispose();
            _renderTarget?.Dispose();

            lock (GraphicsContext)
            {
                GraphicsContext.MakeCurrent(WindowInfo);
                GraphicsContext.Update(WindowInfo);

                GraphicsContext.MakeCurrent(null);

                Resized?.Invoke(null, null);
            }
        }

        public static GRBackendRenderTarget CreateRenderTarget(SKImageInfo info, GRContext context, out int fb, out int texture, out int rbo)
        {
            GL.Enable(EnableCap.Multisample);
            fb = GL.GenFramebuffer();
            texture = GL.GenTexture();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, fb);
            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, info.Width, info.Height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);
            
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, texture, 0);

            rbo = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rbo);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, info.Width, info.Height);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);

            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, rbo);

            var status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);

            var maxSamples = context.GetMaxSurfaceSampleCount(ColorType);
            GL.GetInteger(GetPName.FramebufferBinding, out var framebuffer);
            GL.GetInteger(GetPName.Samples, out var samples);
            samples = samples > maxSamples ? maxSamples : samples;
            GRGlFramebufferInfo glInfo = new GRGlFramebufferInfo((uint)framebuffer, ColorType.ToGlSizedFormat());
            GL.GetInteger(GetPName.StencilBits, out var stencil);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            return new GRBackendRenderTarget(info.Width, info.Height, samples, stencil, glInfo);
        }

        private void Renderer_Destroyed(object sender, EventArgs e)
        {
            CleanUp();
            Dispose();
        }

        public void CleanUp()
        {
            IsRendering = false;
            _resetEvent.Set();

            _rendererThread.Join();

            _contextOpenGL?.Dispose();
            _surface?.Dispose();
            _renderTarget?.Dispose();
        }
    }
}