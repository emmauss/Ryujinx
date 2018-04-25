using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ryujinx.Graphics.Gal;

namespace Ryujinx.Core
{
    public class EmutionController
    {
        private Thread EmulationThread;
        private Switch Ns;
        private IGalRenderer Renderer;
        private bool IsPaused   = false;
        private bool IsShutDown = false;

        public EmutionController(Switch Ns, IGalRenderer Renderer)
        {
            this.Ns = Ns;
            this.Renderer = Renderer;
        }

        public void Start()
        {
            EmulationThread = new Thread(new ThreadStart(() =>
            {
                using (GLScreen Screen = new GLScreen(Ns, Renderer))
                {
                    Ns.Finish += (Sender, Args) =>
                    {
                        Screen?.Exit();
                    };

                    Screen.Closed += (Sender, Args) =>
                    {
                        if(!IsShutDown)
                        Stop();
                    };

                    Screen.Run(60.0);
                }
            }));

            EmulationThread.Start();
        }

        public void Stop()
        {
            IsPaused = false;
            IsShutDown = true;
            Ns.Os.ShutDown();
        }

        public async void Pause()
        {
            IsPaused = true;
            lock (Ns)
            {
                while (IsPaused)
                {
                    Thread.Sleep(1000);
                }                
            }

        }

        public void Continue()
        {
            IsPaused = false;
        }
    }
}
