using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gtk;

namespace RyujinxUI
{

    partial class MainWindow : Gtk.Window
    {
        private Builder builder;

        public static MainWindow CreateWindow()
        {
            var builder = new Builder(null,"RyujinxUI.UI.MainWindow.glade",null);
            return new MainWindow(builder, builder.GetObject("mainWindow").Handle);
        }

        protected MainWindow(Builder builder,IntPtr handle): base(handle)
        {
            this.builder = builder;
            builder.Autoconnect(this);
            SetupHandler();
        }

        void SetupHandler()
        {
            DeleteEvent += MainWindow_DeleteEvent;
        }

        private void MainWindow_DeleteEvent(object o, DeleteEventArgs args)
        {
            Application.Quit();
        }
    }
}
