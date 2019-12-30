using System;
using System.Reflection;
using System.Runtime;
using Gtk;
using Ryujinx.Debugger.UI;

namespace Ryujinx.Debugger
{
    public class Debugger
    {
        private DebuggerWidget _widget;

        public Debugger()
        {
            Widget = new DebuggerWidget();
        }

        public void Enable()
        {
            Widget.Enable();
        }

        public void Disable()
        {
            Widget.Disable();
        }

        public DebuggerWidget Widget { get => _widget; set => _widget = value; }
    }
}
