using Qml.Net;
using Ryujinx.Audio;
using Ryujinx.Audio.OpenAL;
using Ryujinx.Graphics.Gal;
using Ryujinx.Graphics.Gal.OpenGL;
using Ryujinx.HLE;
using System;
using System.IO;
using System.Threading;

namespace Ryujinx.UI.Emulation
{
    [Signal("failed", NetVariantType.String)]
    [Signal("success")]
    [Signal("loaded")]
    [Signal("unloaded")]
    class EmulationController
    {
        public static Switch Device;

        private static GLScreen     RenderScreen;
        private static IAalOutput   AudioOut;
        private static IGalRenderer Renderer;

        private bool IsClosing;

        private bool isLoaded;

        public bool IsLoaded
        {
            get
            {
                return isLoaded;
            }

            set
            {
                isLoaded = value;

                if (value)
                {
                    this.ActivateSignal("loaded");
                }
                else
                {
                    this.ActivateSignal("unloaded");
                }
            }
        }

        public EmulationController()
        {
            RenderScreen = null;
            Device       = null;
            AudioOut     = null;

            IsLoaded = false;
        }


        public void LoadGameFile(string GameFile)
        {
            GameFile = GameFile.Replace("file:///", string.Empty);

            if (!File.Exists(GameFile))
            {
                this.ActivateSignal("failed", $"File {GameFile} does not exist.");

                return;
            }

            ShutdownEmulation();

            InitializeEmulator();

            switch (Path.GetExtension(GameFile).ToLowerInvariant())
            {
                case ".xci":
                    Console.WriteLine("Loading as XCI.");
                    Device.LoadXci(GameFile);
                    break;
                case ".nca":
                    Console.WriteLine("Loading as NCA.");
                    Device.LoadNca(GameFile);
                    break;
                case ".nsp":
                    Console.WriteLine("Loading as NSP.");
                    Device.LoadNsp(GameFile);
                    break;
                default:
                    Console.WriteLine("Loading as homebrew.");
                    Device.LoadProgram(GameFile);
                    break;
            }

            StartRenderer();

            IsLoaded = true;

            this.ActivateSignal("success");
        }

        public void LoadGameFolder(string ExeFsPath)
        {
            if (!Directory.Exists(ExeFsPath))
            {
                this.ActivateSignal("failed", $"Directory {ExeFsPath} does not exist.");

                return;
            }

            ShutdownEmulation();

            InitializeEmulator();

            string[] RomFsFiles = Directory.GetFiles(ExeFsPath, "*.istorage");

            if (RomFsFiles.Length == 0)
            {
                RomFsFiles = Directory.GetFiles(ExeFsPath, "*.romfs");
            }

            if (RomFsFiles.Length > 0)
            {
                Console.WriteLine("Loading as cart with RomFS.");

                Device.LoadCart(ExeFsPath, RomFsFiles[0]);
            }
            else
            {
                Console.WriteLine("Loading as cart WITHOUT RomFS.");

                Device.LoadCart(ExeFsPath);
            }

            StartRenderer();

            IsLoaded = true;

            this.ActivateSignal("success");
        }

        public void ShutdownEmulation()
        {
            if(IsClosing || !IsLoaded)
            {
                return;
            }

            IsClosing = true;

            Renderer?.ClearActions();

            RenderScreen?.Close();

            RenderScreen?.Dispose();

            Device?.Dispose();

            AudioOut?.Dispose();

            ConsoleLog.Stop();

            while (RenderScreen!=null && RenderScreen.Exists)
            {
                Thread.Sleep(5000);
            }

            RenderScreen = null;
            Device       = null;
            AudioOut     = null;
            Renderer     = null;

            IsLoaded  = false;
            IsClosing = false;
        }

        public void InitializeEmulator()
        {
            Renderer = new OGLRenderer();

            AudioOut = new OpenALAudioOut();

            Device   = new Switch(Renderer, AudioOut);

            Config.Read(Device);

            ConsoleLog.Start();

            Device.Log.Updated += ConsoleLog.Log;
        }

        public void StartRenderer()
        {
            Thread RendererThread = new Thread(() => 
            {
                using (RenderScreen = new GLScreen(Device, Renderer))
                {
                    RenderScreen.Closed += (o, e) => { ShutdownEmulation(); };
                    RenderScreen.MainLoop();
                }
            });

            RendererThread.Start();
        }
    }
}
