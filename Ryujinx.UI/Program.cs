using Gtk;
using System;
using System.Reflection;
using System.Linq;
using Ryujinx.Core;
using Ryujinx.Core.Logging;

namespace Ryujinx.UI
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Application.Init();

            Console.Title = "Ryujinx Console";

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            AppDomain.CurrentDomain.AppendPrivatePath(@"Dependencies\");

            Console.SetOut(UI.Debugging.LogPage.LogWriter);

            var resourceNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            var app = new Application("org.Ryujinx.UI.Ryujinx.UI", GLib.ApplicationFlags.None);
            app.Register(GLib.Cancellable.Current);

            var win = new MainWindow();
            app.AddWindow(win);

            win.Show();
            Application.Run();
        }
    }
}
