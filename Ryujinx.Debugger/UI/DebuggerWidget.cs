using Gtk;
using System;
using GUI = Gtk.Builder.ObjectAttribute;

namespace Ryujinx.Debugger.UI
{
    public class DebuggerWidget : Box
    {
        public event EventHandler DebuggerEnabled;
        public event EventHandler DebuggerDisabled;

        [GUI] Notebook _widgetNotebook;

        public DebuggerWidget() : this(new Builder("Ryujinx.Debugger.UI.DebbugerWidget.glade")) { }

        public DebuggerWidget(Builder builder) : base(builder.GetObject("_debbugerBox").Handle)
        {
            builder.Autoconnect(this);

            LoadProfiler();
        }

        public void LoadProfiler()
        {
            ProfilerWidget widget = new ProfilerWidget();

            widget.RegisterParentDebugger(this);

            _widgetNotebook.AppendPage(widget, new Label("Profiler"));

            Notebook codeNotebook = new Notebook();

            ArmCodeWidget armCodeWidget = new ArmCodeWidget();

            armCodeWidget.RegisterParentDebugger(this);

            _widgetNotebook.AppendPage(codeNotebook, new Label("Code"));

            codeNotebook.AppendPage(armCodeWidget, new Label("Arm"));
        }

        public void Enable()
        {
            DebuggerEnabled.Invoke(this, null);
        }

        public void Disable()
        {
            DebuggerDisabled.Invoke(this, null);
        }
    }
}
