using System;
using System.Collections.Generic;
using System.Threading;
using Ryujinx.HLE;
using Ryujinx.Graphics.OpenGL;
using System.Text;
using OpenTK.Input;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Gdk;

namespace Ryujinx.Ui
{
    public class GLRenderer : GLWidget
    {
        public ManualResetEvent waitEvent { get; set; }

        public bool IsActive  { get; set; }
        public bool IsStopped { get; set; }

        private const int TargetFps = 60;

        private readonly long _ticksPerFrame;

        private long _ticks = 0;

        private System.Diagnostics.Stopwatch _chrono;

        private Switch _device;

        private Renderer _renderer;

        public GLRenderer(Switch device) :base (new GraphicsMode(), 3, 3, GraphicsContextFlags.ForwardCompatible)
        {
            waitEvent = new ManualResetEvent(false);

            _device = device;

            this.Initialized += GLRenderer_Initialized;
            this.Destroyed += GLRenderer_Destroyed;

            Initialize();

            _chrono = new System.Diagnostics.Stopwatch();

            _ticksPerFrame = System.Diagnostics.Stopwatch.Frequency / TargetFps;
        }

        private void GLRenderer_Destroyed(object sender, EventArgs e)
        {
            Exit();

            this.Dispose();
        }

        private void GLRenderer_Initialized(object sender, EventArgs e)
        {
            Start();
        }

        protected override bool OnConfigureEvent(EventConfigure evnt)
        {
            var result  = base.OnConfigureEvent(evnt);

            _renderer.Window.SetSize(AllocatedWidth, AllocatedHeight);

            return result;
        }

        public void Start()
        {
            _renderer.Initialize();

            _chrono.Restart();

            IsActive = true;

             GLib.Idle.Add(Render, GLib.Priority.DefaultIdle);
        }

        public void Exit()
        {
            _device.DisposeGpu();

            IsStopped = true;

            waitEvent.Set();
        }

        public void Initialize()
        {
            if (!(_device.Gpu.Renderer is Renderer))
            {
                throw new NotSupportedException($"GPU renderer must be an OpenGL renderer when using GLRenderer!");
            }

            _renderer = (Renderer)_device.Gpu.Renderer;
        }

        public bool Render()
        {
            if (!IsActive)
            {
                return true;
            }

            if (IsStopped)
            {
                return false;
            }

            GL.ClearColor(Color4.Black);

            _ticks += _chrono.ElapsedTicks;

            _chrono.Restart();

            if (_device.WaitFifo())
            {
                _device.ProcessFrame();
            }

            if (_ticks >= _ticksPerFrame)
            {
                _device.PresentFrame(SwapBuffers);

                _device.Statistics.RecordSystemFrameTime();

                double hostFps = _device.Statistics.GetSystemFrameRate();
                double gameFps = _device.Statistics.GetGameFrameRate();

                string titleNameSection = string.IsNullOrWhiteSpace(_device.System.TitleName) ? string.Empty
                    : " | " + _device.System.TitleName;

                string titleIdSection = string.IsNullOrWhiteSpace(_device.System.TitleIdText) ? string.Empty
                    : " | " + _device.System.TitleIdText.ToUpper();

                string  newTitle = $"Ryujinx{titleNameSection}{titleIdSection} | Host FPS: {hostFps:0.0} | Game FPS: {gameFps:0.0} | " +
                    $"Game Vsync: {(_device.EnableDeviceVsync ? "On" : "Off")}";

                this.ParentWindow.Title = newTitle;

                _device.System.SignalVsync();

                _device.VsyncEvent.Set();

                _ticks = Math.Min(_ticks - _ticksPerFrame, _ticksPerFrame);
            }

            return true;
        }

        public void SwapBuffers()
        {
            OpenTK.Graphics.GraphicsContext.CurrentContext.SwapBuffers();
        }
    }
}
