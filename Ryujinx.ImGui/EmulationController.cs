using System;
using System.Collections.Generic;
using System.Text;
using Ryujinx.HLE;

namespace Ryujinx.UI
{
    class EmulationController
    {
        public bool IsLoaded = false;
        private Switch Ns;

        public EmulationController(Switch Ns)
        {
            this.Ns = Ns;
        }

        public void Resume()
        {
            lock(Ns)
            Ns.Os.ResumeAllProcesses();
        }

        public void Pause()
        {
            lock(Ns)
            Ns.Os.PauseAllProcesses();
        }

        public void ShutDown()
        {
            lock (Ns)
                Ns.Dispose();

        }
    }
}
