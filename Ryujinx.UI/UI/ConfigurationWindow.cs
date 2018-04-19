using Gtk;
using System;
using System.Reflection;
using Ryujinx.Core.Logging;
using GUI = Gtk.Builder.ObjectAttribute;

namespace Ryujinx.UI.UI
{
    public class ConfigurationWindow : Dialog
    {
        Logger Log;
        [GUI] Notebook OptionNotebook;
        [GUI] Button   OptionAcceptButton;
        [GUI] Button   OptionCancelButton;

        public ConfigurationWindow(Logger Log) : this(new Builder("ConfigurationWindow.glade"))
        {
            this.Log = Log;
        }

        private ConfigurationWindow(Builder builder) : base(builder.GetObject("ConfigurationWindow").Handle)
        {
            builder.Autoconnect(this);

            //Saves current configuration      
            Settings.Write(Log);

            //Loads Parser
            var iniFolder = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var iniPath = System.IO.Path.Combine(iniFolder, "Ryujinx.conf");

            //Add pages
            Label GeneralLabel = new Label("General");
            GeneralPage GeneralPage = new GeneralPage();
            OptionNotebook.AppendPage(GeneralPage.GetWidget(), GeneralLabel);
            Label InputLabel = new Label("Input");
            InputPage InputPage = new InputPage();
            OptionNotebook.AppendPage(InputPage.GetWidget(), InputLabel);

            //Register Events
            OptionAcceptButton.Clicked += OptionAcceptButton_Clicked;
            OptionCancelButton.Clicked += OptionCancelButton_Clicked;
            
        }

        private void OptionCancelButton_Clicked(object sender, EventArgs e)
        {
            this.Respond(ResponseType.Cancel);
        }

        private void OptionAcceptButton_Clicked(object sender, EventArgs e)
        {
            this.Respond(ResponseType.Accept);
        }
    }
}
