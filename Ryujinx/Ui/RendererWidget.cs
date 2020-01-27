using Gtk;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;
using OpenTK.Platform.Windows;
using Ryujinx.Configuration;
using Ryujinx.Graphics.OpenGL;
using Ryujinx.HLE;
using Ryujinx.HLE.Input;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using GUI = Gtk.Builder.ObjectAttribute;

namespace Ryujinx.Ui
{
    public class RendererWidget : GLArea, IDisposable
    {
        public ManualResetEvent HoldEvent{ get; set; }
        private Gdk.GLContext _rendererContext;

        private GraphicsContext _tkContext;

        private Thread _mainThread;
        private Thread _renderThread;

        private bool _isRendering;
        private bool _isInit;
        private int _contextCount = 0;

        private Ryujinx.HLE.Switch _device;

        private Renderer _renderer;
        public RendererWidget(int width, int height, ManualResetEvent holdEvent)
        {
            HoldEvent = holdEvent;

            Toolkit.Init();
            
            HeightRequest = 720;
            WidthRequest = 1280;

            SetRequiredVersion(3, 3);

            this.Realized += Renderer_Realized;
            this.Resize += Renderer_Resize;

            this.AddSignalHandler("create-context", new Func<Gdk.GLContext>(() =>
             {
                 _rendererContext = new Gdk.GLContext(Window.Handle);
                 _rendererContext.ForwardCompatible = true;
                 _rendererContext.SetRequiredVersion(3, 3);

                 _rendererContext.Realize();

                 _rendererContext.MakeCurrent();

                 GrabContext();

                 return _rendererContext;
             }));

            this.Show();
        }

        public void Initialize(Ryujinx.HLE.Switch device)
        {
            _device = device;

            if (!(device.Gpu.Renderer is Renderer))
            {
                throw new NotSupportedException($"GPU renderer must be an OpenGL renderer when using GlScreen!");
            }

            _renderer = (Renderer)device.Gpu.Renderer;

            _renderer.Window.SetSize(AllocatedWidth, AllocatedHeight);
        }

        public void Renderer_Resize(object sender, ResizeArgs args)
        {
            _renderer.Window.SetSize(AllocatedWidth, AllocatedHeight);
        }

        protected override bool OnDestroyEvent(Gdk.Event evnt)
        {
            _isRendering = false;
            HoldEvent.Set();

            return base.OnDestroyEvent(evnt);
        }

        public void Start()
        {
            _isRendering = true;

            HoldEvent.Reset();
        }

        public void Close()
        {
            _isRendering = false;            
            HoldEvent.Set();

            this.Dispose();
        }

        protected override bool OnRender(Gdk.GLContext context)
        {        
            if (!_isInit)
            {
                //GrabContext();
                
            _renderer.Initialize();

            _isInit = true;
            }

            if (!_isRendering || !_isInit)
            {
                return true;
            }

            Stopwatch chrono = new Stopwatch();

            chrono.Start();

            long ticksPerFrame = Stopwatch.Frequency / 60;

            long ticks = 0;

            if (_device.WaitFifo())
            {
                _device.ProcessFrame();
            }

            ticks += chrono.ElapsedTicks;

            chrono.Restart();

            if (ticks >= ticksPerFrame)
            {
                RenderFrame();

                // Queue max. 1 vsync
                ticks = Math.Min(ticks - ticksPerFrame, ticksPerFrame);
            }

            QueueRender();

            return true;
        }

        private void RenderFrame()
        {
            _device.PresentFrame(QueueRender);

            _device.Statistics.RecordSystemFrameTime();

            double hostFps = _device.Statistics.GetSystemFrameRate();
            double gameFps = _device.Statistics.GetGameFrameRate();

            string titleNameSection = string.IsNullOrWhiteSpace(_device.System.TitleName) ? string.Empty
                : " | " + _device.System.TitleName;

            string titleIdSection = string.IsNullOrWhiteSpace(_device.System.TitleIdText) ? string.Empty
                : " | " + _device.System.TitleIdText.ToUpper();

           /* _newTitle = $"Ryujinx{titleNameSection}{titleIdSection} | Host FPS: {hostFps:0.0} | Game FPS: {gameFps:0.0} | " +
                $"Game Vsync: {(_device.EnableDeviceVsync ? "On" : "Off")}";

            _titleEvent = true;*/

            _device.System.SignalVsync();

            _device.VsyncEvent.Set();
        }

       /* protected override Gdk.GLContext OnCreateContext()
        {
            _rendererContext = new Gdk.GLContext(this.Parent.Handle);
            _rendererContext.ForwardCompatible = true;
            _rendererContext.SetRequiredVersion(3, 3);

            _rendererContext.Realize();

           /* var info = Utilities.CreateWindowsWindowInfo(this.Handle);

            _tkContext = new GraphicsContext(new ContextHandle(_rendererContext.Handle), info);

            //GrabContext();

            return _rendererContext;
        }*/

        public void GrabContext()
        {
            Context.MakeCurrent();
            // Make the GDK GL context current
            // _rendererContext.MakeCurrent();

            // Create a dummy context that will grab the GdkGLContext that is current on the thread
            _tkContext = new GraphicsContext(ContextHandle.Zero, null);

            _tkContext.ErrorChecking = true;

            if (GraphicsContext.ShareContexts)
            {
                Interlocked.Increment(ref _contextCount);

                ((IGraphicsContextInternal)_tkContext).LoadAll();
            }
            else
            {
                ((IGraphicsContextInternal)_tkContext).LoadAll();
            }
            
        }

        private void Renderer_Realized(object sender, EventArgs e)
        {
           // _rendererContext = Context;
           // _rendererContext.ForwardCompatible = true;

            _renderer.Window.SetSize(AllocatedWidth, AllocatedHeight);
        }
    }
}