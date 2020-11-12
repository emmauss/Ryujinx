using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK;
using Ryujinx.Common;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Ryujinx.Graphics.OpenGL
{
    unsafe class BackgroundContextWorker : IDisposable
    {
        [ThreadStatic]
        public static bool InBackground;

        private OpenTK.Windowing.GraphicsLibraryFramework.Window* _window;
        private Thread _thread;
        private bool _running;

        private AutoResetEvent _signal;
        private Queue<Action> _work;
        private ObjectPool<ManualResetEventSlim> _invokePool;

        public BackgroundContextWorker(OpenTK.Windowing.GraphicsLibraryFramework.Window* baseContext)
        {
            GLFW.WindowHint(WindowHintBool.Visible, false);
            GLFW.WindowHint(WindowHintBool.Resizable, false);
            GLFW.WindowHint(WindowHintContextApi.ContextCreationApi, ContextApi.NativeContextApi);
            GLFW.WindowHint(WindowHintOpenGlProfile.OpenGlProfile, OpenGlProfile.Core);
            GLFW.WindowHint(WindowHintClientApi.ClientApi, ClientApi.OpenGlApi);
            GLFW.WindowHint(WindowHintInt.ContextVersionMajor, 3);
            GLFW.WindowHint(WindowHintInt.ContextVersionMinor, 3);
            GLFW.WindowHint(WindowHintBool.OpenGLForwardCompat, true);

            _window = GLFW.CreateWindow(100, 100, "Background Window", null, baseContext);

            _running = true;

            _signal = new AutoResetEvent(false);
            _work = new Queue<Action>();
            _invokePool = new ObjectPool<ManualResetEventSlim>(() => new ManualResetEventSlim(), 10);

            _thread = new Thread(Run);
            _thread.Start();
        }

        private void Run()
        {
            InBackground = true;

            GLFW.MakeContextCurrent(_window);

            while (_running)
            {
                Action action;

                lock (_work)
                {
                    _work.TryDequeue(out action);
                }

                if (action != null)
                {
                    action();
                }
                else
                {
                    _signal.WaitOne();
                }
            }

            GLFW.DestroyWindow(_window);
        }

        public void Invoke(Action action)
        {
            ManualResetEventSlim actionComplete = _invokePool.Allocate();

            lock (_work)
            {
                _work.Enqueue(() =>
                {
                    action();
                    actionComplete.Set();
                });
            }

            _signal.Set();

            actionComplete.Wait();
            actionComplete.Reset();

            _invokePool.Release(actionComplete);
        }

        public void Dispose()
        {
            _running = false;
            _signal.Set();

            _thread.Join();
            _signal.Dispose();
        }
    }
}