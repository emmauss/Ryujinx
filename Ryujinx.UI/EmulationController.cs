using Ryujinx.HLE;
using System;
using System.IO;

namespace Ryujinx.UI
{
    class EmulationController
    {
        public bool IsLoaded = false;

        private Switch Ns;

        public event EventHandler IsShutDown;

        public EmulationController(Switch Ns)
        {
            this.Ns = Ns;
        }

        public void Resume()
        {
            Ns.Os.ResumeAllProcesses();
        }

        public void Pause()
        {
            Ns.Os.PauseAllProcesses();
        }

        public void ShutDown()
        {
            Ns.Dispose();

            Ns       = null;
            IsLoaded = false;

            IsShutDown.Invoke(null,null);
        }

        public void Load(string Path)
        {
            if (Directory.Exists(Path))
            {
                string[] RomFsFiles = Directory.GetFiles(Path, "*.istorage");

                if (RomFsFiles.Length == 0)
                {
                    RomFsFiles = Directory.GetFiles(Path, "*.romfs");
                }

                if (RomFsFiles.Length > 0)
                {
                    Console.WriteLine("Loading as cart with RomFS.");

                    Ns.LoadCart(Path, RomFsFiles[0]);
                }
                else
                {
                    Console.WriteLine("Loading as cart WITHOUT RomFS.");

                    Ns.LoadCart(Path);
                }
            }
            else if (File.Exists(Path))
            {
                Console.WriteLine("Loading as homebrew.");

                Ns.LoadProgram(Path);
            }
            IsLoaded = true;
        }
    }
}
