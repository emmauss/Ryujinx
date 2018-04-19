using Gtk;
using System;
using System.Reflection;
using GUI = Gtk.Builder.ObjectAttribute;


namespace Ryujinx.UI.UI.Debugging
{
    public class Debugger : Gtk.Window
    {
        [GUI] Notebook DebuggerNotebook;

        public Debugger() : this(new Builder("Debugger.glade")) { }

        public Debugger(Builder builder) : base(builder.GetObject("Debugger").Handle)
        {
            builder.Autoconnect(this);

            //Add Pages
            Label LogLabel = new Label("Log");
            LogPage LogPage = new LogPage();
            DebuggerNotebook.AppendPage(LogPage.Widget, LogLabel);
        }
    }
}
