using Gtk;
using System;
using GUI = Gtk.Builder.ObjectAttribute;

namespace Ryujinx.UI.UI
{
    public class GeneralPage : Alignment
    {
        Alignment Alignment;

        [GUI] CheckButton MemoryChecksToggle;

        Builder builder = new Builder("GeneralPage.glade");

        public GeneralPage(): base(0.5f, 0.5f, 1, 1)
        {
            builder.Autoconnect(this);

            Alignment = (Alignment)builder.GetObject("GeneralLayout");

            MemoryChecksToggle.Toggled += MemoryChecksToggle_Toggled;

            MemoryChecksToggle.Active = !AOptimizations.DisableMemoryChecks;
        }

        private void MemoryChecksToggle_Toggled(object sender, EventArgs e)
        {
            AOptimizations.DisableMemoryChecks = !MemoryChecksToggle.Active;
        }

        public Widget GetWidget()
        {
            return Alignment;
        }
    }
}
