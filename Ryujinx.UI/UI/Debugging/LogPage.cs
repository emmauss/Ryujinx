using Gtk;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using Ryujinx.Core.Logging;
using GUI = Gtk.Builder.ObjectAttribute;
namespace Ryujinx.UI.UI.Debugging
{
    public class LogPage : Box
    {
        Logger Log; 

        public static LogWriter LogWriter = new LogWriter();

        Builder builder = new Builder("LogPage.glade");

        Box LogBox;

        //UI elements
        [GUI] Button      SaveButton;
        [GUI] TextView    LogTextView;
        [GUI] TextView    LogClassesBox;
        [GUI] CheckButton InfoLogEnable;
        [GUI] CheckButton DebugLogEnable;
        [GUI] CheckButton ErrorLogEnable;
        [GUI] CheckButton StubLogEnable;
        [GUI] CheckButton WarnLogEnable;

        public LogPage() : base(Orientation.Horizontal, 0)
        {
            //Load styles
            CssProvider provider = new CssProvider();
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Ryujinx.UI.UI.style.css"))
            using (StreamReader reader = new StreamReader(stream))
            {
                provider.LoadFromData(reader.ReadToEnd());
            }

            builder.Autoconnect(this);

            LogBox = (Box)builder.GetObject("MainBox");

            //Style the log text box
            LogTextView.StyleContext.AddProvider(provider,1000);

            //Register Events
            InfoLogEnable.Toggled           += InfoLogEnable_Toggled;
            DebugLogEnable.Toggled          += DebugLogEnable_Toggled;
            ErrorLogEnable.Toggled          += ErrorLogEnable_Toggled;
            WarnLogEnable.Toggled           += WarnLogEnable_Toggled;
            StubLogEnable.Toggled           += StubLogEnable_Toggled;
            SaveButton.Clicked              += SaveButton_Clicked;
            LogClassesBox.Buffer.InsertText += Buffer_InsertText;

            //Set values            
            LogTextView.Buffer        = LogWriter.LogBuffer;
        }

        private void SaveButton_Clicked(object sender, EventArgs e)
        {
            if (Log != null)
                Settings.Write(Log);
        }

        public void UpdateSettings(Logger Log)
        {
            this.Log = Log;

            InfoLogEnable.Active  = Log.IsEnabled(LogLevel.Info);
            DebugLogEnable.Active = Log.IsEnabled(LogLevel.Debug);
            ErrorLogEnable.Active = Log.IsEnabled(LogLevel.Error);
            WarnLogEnable.Active  = Log.IsEnabled(LogLevel.Warning);
            StubLogEnable.Active  = Log.IsEnabled(LogLevel.Stub);

            string EnabledClasses = string.Empty;
            foreach(var logClass in Enum.GetNames(typeof(LogClass)))
            {
                if (Log.IsEnabled(Enum.Parse<LogClass>(logClass)))
                    EnabledClasses += logClass + Environment.NewLine;
            }

            LogClassesBox.Buffer.Text = EnabledClasses;
        }

        private void Buffer_InsertText(object o, InsertTextArgs args)
        {
            string[] FilteredLogClasses = LogClassesBox.Buffer.Text.Split('\n');

            if (FilteredLogClasses.Length > 0)
            {
                foreach (LogClass Class in Enum.GetValues(typeof(LogClass)))
                {
                    Log.SetEnable(Class, false);
                }
            }

            foreach (string LogClass in FilteredLogClasses)
            {
                if (!string.IsNullOrEmpty(LogClass.Trim()))
                {
                    foreach (LogClass Class in Enum.GetValues(typeof(LogClass)))
                    {
                        if (Class.ToString().ToLower().Contains(LogClass.Trim().ToLower()))
                        {
                            Log.SetEnable(Class, true);
                        }
                    }
                }
            }
        }

        private void StubLogEnable_Toggled(object sender, EventArgs e)
        {
            if (sender is CheckButton LogCheckButton)
            {
                Log.SetEnable(LogLevel.Stub,LogCheckButton.Active);
            }
        }

        private void WarnLogEnable_Toggled(object sender, EventArgs e)
        {
            if (sender is CheckButton LogCheckButton)
            {
                Log.SetEnable(LogLevel.Warning, LogCheckButton.Active);
            }
        }

        private void ErrorLogEnable_Toggled(object sender, EventArgs e)
        {
            if (sender is CheckButton LogCheckButton)
            {
                Log.SetEnable(LogLevel.Error, LogCheckButton.Active);
            }
        }

        private void DebugLogEnable_Toggled(object sender, EventArgs e)
        {
            if (sender is CheckButton LogCheckButton)
            {
                Log.SetEnable(LogLevel.Debug, LogCheckButton.Active);
            }
        }

        private void InfoLogEnable_Toggled(object sender, EventArgs e)
        {
            if (sender is CheckButton LogCheckButton)
            {
                Log.SetEnable(LogLevel.Info, LogCheckButton.Active);
            }
        }

        public Widget Widget
        {
            get => LogBox;
        }
    }

    public class LogWriter : TextWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
        public TextBuffer LogBuffer { get; private set; }
        private TextIter EndIter;

        public LogWriter()
        {
            LogBuffer = new TextBuffer(null);
            EndIter = LogBuffer.EndIter;

            //Add color tags
            LogBuffer.TagTable.Add(new TextTag("Red")        { Foreground = "red" });
            LogBuffer.TagTable.Add(new TextTag("Gray")       { Foreground = "grey" });
            LogBuffer.TagTable.Add(new TextTag("Yellow")     { Foreground = "yellow" });
            LogBuffer.TagTable.Add(new TextTag("Magenta")    { Foreground = "magenta" });
            LogBuffer.TagTable.Add(new TextTag("White")      { Foreground = "white" });
            LogBuffer.TagTable.Add(new TextTag("DarkYellow") { Foreground = "orange" });
            LogBuffer.TagTable.Add(new TextTag("DarkGray")   { Foreground = "darkgray" });
        }

        public override void Write(string value)
        {
            string consoleColor = Console.ForegroundColor.ToString();
            Gtk.Application.Invoke(delegate
            {
                LogBuffer.InsertWithTagsByName(ref EndIter, value, consoleColor);
            });
        }

        public override void WriteLine(string value)
        {
            string consoleColor = Console.ForegroundColor.ToString();
            Gtk.Application.Invoke(delegate
            {
                LogBuffer.InsertWithTagsByName(ref EndIter, value + Environment.NewLine, consoleColor);
            });
        }

        
    }
}
