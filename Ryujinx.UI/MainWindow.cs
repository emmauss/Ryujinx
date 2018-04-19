using System;
using System.Threading.Tasks;
using Gtk;
using GUI = Gtk.Builder.ObjectAttribute;
using Ryujinx.Audio;
using Ryujinx.Audio.OpenAL;
using Ryujinx.Core;
using System.Threading;
using Ryujinx.Graphics.Gal;
using Ryujinx.Graphics.Gal.OpenGL;
using System.IO;
using System.Reflection;
using Ryujinx.Core.Logging;

namespace Ryujinx.UI
{
    class MainWindow : Window
    {
        //UI Controls
        [GUI] MenuItem LoadFileMenuItem;
        [GUI] MenuItem LoadFolderMenuItem;
        [GUI] MenuItem ExitMenuItem;
        [GUI] MenuItem OptionMenuItem;
        [GUI] MenuItem ShowDebugMenuItem;
        [GUI] MenuItem ContinueMenuItem;
        [GUI] MenuItem PauseMenuItem;
        [GUI] MenuItem StopMenuItem;
        [GUI] MenuItem AboutMenuItem;

        bool DebugWindowActive = false;

        Core.Switch Ns;

        IAalOutput AudioOut = new OpenALAudioOut();

        IGalRenderer Renderer;

        EmutionController EmulationController;

        public MainWindow() : this(new Builder("MainWindow.glade")) { }

        private MainWindow(Builder builder) : base(builder.GetObject("MainWindow").Handle)
        {
            builder.Autoconnect(this);

            //Load Icon
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Ryujinx.UI.ryujinxicon.png"))
            using (StreamReader reader = new StreamReader(stream))
            {
                Icon = new Gdk.Pixbuf(stream);
            }

            InitializeSwitch();
           
            //Register Events
            DeleteEvent                  += Window_DeleteEvent;
            LoadFileMenuItem.Activated   += LoadFileMenuItem_Activated;
            LoadFolderMenuItem.Activated += LoadFolderMenuItem_Activated;
            ExitMenuItem.Activated       += ExitMenuItem_Activated;
            OptionMenuItem.Activated     += OptionMenuItem_Activated;
            ShowDebugMenuItem.Activated  += ShowDebugMenuItem_Activated;
            ContinueMenuItem.Activated   += ContinueMenuItem_Activated;
            PauseMenuItem.Activated      += PauseMenuItem_Activated;
            StopMenuItem.Activated       += StopMenuItem_Activated;
            AboutMenuItem.Activated      += AboutMenuItem_Activated;

            PauseMenuItem.Sensitive    = false;
            ContinueMenuItem.Sensitive = false;
            StopMenuItem.Sensitive     = false;

            //Initialize Ryujinx
            Console.Title = "Ryujinx Console";
        }

        private void AboutMenuItem_Activated(object sender, EventArgs e)
        {
            AboutDialog AboutDialog = new AboutDialog
            {
                Title = "Ryujinx",
                Logo = Icon,
                Comments = "This is a free switch emulator",
                Copyright = "2018 - Ryujinx Team"
            };

            AboutDialog.Run();
            AboutDialog.Destroy();
        }

        private void LoadFolderMenuItem_Activated(object sender, EventArgs e)
        {
            FileChooserDialog ContentLoader = new FileChooserDialog("Open Game Folder", this, FileChooserAction.SelectFolder,
                "Cancel", Gtk.ResponseType.Cancel,
                "Open", Gtk.ResponseType.Accept
                );
            if (ContentLoader.Run() == (int)Gtk.ResponseType.Accept)
            {
                if (Directory.Exists(ContentLoader.Filename))
                {
                    InitializeSwitch();

                    string FolderName = ContentLoader.Filename;

                    ContentLoader.Destroy();

                    string[] RomFsFiles = Directory.GetFiles(FolderName, "*.istorage");

                    if (RomFsFiles.Length == 0)
                    {
                        RomFsFiles = Directory.GetFiles(FolderName, "*.romfs");
                    }

                    if (RomFsFiles.Length > 0)
                    {
                        Console.WriteLine("Loading as cart with RomFS.");

                        Ns.LoadCart(FolderName, RomFsFiles[0]);
                    }
                    else
                    {
                        Console.WriteLine("Loading as cart WITHOUT RomFS.");

                        Ns.LoadCart(FolderName);
                    }
                    
                    Start();
                }
            }
            else
                ContentLoader.Destroy();
        }

        void InitializeSwitch()
        {
            Renderer = new OpenGLRenderer();

            IAalOutput AudioOut = new OpenALAudioOut();

            Ns = new Core.Switch(Renderer, AudioOut);

            Settings.Read(Ns.Log);
        }

        private void StopMenuItem_Activated(object sender, EventArgs e)
        {
            EmulationController?.Stop();
            PauseMenuItem.Sensitive = false;
            ContinueMenuItem.Sensitive = false;
            StopMenuItem.Sensitive = false;
            EmulationController = null;
        }

        private void PauseMenuItem_Activated(object sender, EventArgs e)
        {
            new Thread(()=>EmulationController?.Pause()).Start();
            PauseMenuItem.Sensitive = false;
            ContinueMenuItem.Sensitive = true;
            StopMenuItem.Sensitive = true;
        }

        private void ContinueMenuItem_Activated(object sender, EventArgs e)
        {
            EmulationController?.Continue();
            PauseMenuItem.Sensitive = true;
            ContinueMenuItem.Sensitive = false;
            StopMenuItem.Sensitive = true;
        }

        private void ShowDebugMenuItem_Activated(object sender, EventArgs e)
        {
            UI.Debugging.Debugger debugger = new UI.Debugging.Debugger();
            debugger.DeleteEvent += Debugger_DeleteEvent;
            DebugWindowActive = true;
            debugger.Show();
        }

        private void Debugger_DeleteEvent(object o, DeleteEventArgs args)
        {
            DebugWindowActive = false;
        }

        private void OptionMenuItem_Activated(object sender, EventArgs e)
        {
            UI.ConfigurationWindow configurationWindow = new UI.ConfigurationWindow(Ns.Log);

            if (configurationWindow.Run() == (int)ResponseType.Accept)
            {
                Settings.Write(Ns.Log);                
            }
            else
                Settings.Read(Ns.Log);

            configurationWindow.Destroy();
        }

        private void ExitMenuItem_Activated(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void LoadFileMenuItem_Activated(object sender, EventArgs e)
        {
            FileChooserDialog ContentLoader = new FileChooserDialog("Open Package", this, FileChooserAction.Open,
                "Cancel", Gtk.ResponseType.Cancel,
                "Open", Gtk.ResponseType.Accept
                );
            if (ContentLoader.Run() == (int)Gtk.ResponseType.Accept)
            {
                if (File.Exists(ContentLoader.Filename))
                {
                    InitializeSwitch();

                    Ns.LoadProgram(ContentLoader.Filename);

                    ContentLoader.Destroy();
                    Start();
                }
            }
            else
                ContentLoader.Destroy();
        }

        private void Window_DeleteEvent(object sender, DeleteEventArgs a)
        {
            EmulationController?.Stop();
            Environment.Exit(0);
        }

        void Start()
        {
            EmulationController = new EmutionController(Ns, Renderer);
            EmulationController.Start();
            PauseMenuItem.Sensitive = true;
            ContinueMenuItem.Sensitive = false;
            StopMenuItem.Sensitive = true;
        }
    }
}
