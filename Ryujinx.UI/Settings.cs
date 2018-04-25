using System;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Ryujinx.Core;
using Ryujinx.Core.Input;
using Newtonsoft.Json;
using Ryujinx.Core.Logging;

namespace Ryujinx.UI
{
    public static class Settings
    {
        private static string ConfigPath;

        static Settings()
        {
            var IniFolder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            ConfigPath = Path.Combine(IniFolder, "RyujinxUI.json");
        }

        public static void Read(Logger Log)
        {
            JsonParser JsonParser = new JsonParser(ConfigPath);
            var Configuration = JsonParser.Load();

            Config.FakeJoyCon = Configuration.EmulatedJoyCon;

            AOptimizations.DisableMemoryChecks = !Configuration.EnableMemoryChecks;

            Log.SetEnable(LogLevel.Info, Configuration.LoggingEnableInfo);
            Log.SetEnable(LogLevel.Debug, Configuration.LoggingEnableDebug);
            Log.SetEnable(LogLevel.Error, Configuration.LoggingEnableError);
            Log.SetEnable(LogLevel.Warning, Configuration.LoggingEnableWarn);
            Log.SetEnable(LogLevel.Stub, Configuration.LoggingEnableStub);

            Configuration.LoggingFilteredClasses = Configuration.LoggingFilteredClasses != null ?
                Configuration.LoggingFilteredClasses : string.Empty;
            string[] FilteredLogClasses = Configuration.LoggingFilteredClasses.Trim().Split('\n',StringSplitOptions.RemoveEmptyEntries);

            //When the classes are specified on the list, we only
            //enable the classes that are on the list.
            //So, first disable everything, then enable
            //the classes that the user added to the list.
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

        public static void Write(Logger Logger)
        {
            if (Logger == null)
                Logger = new Logger();
            Configuration Configuration = new Configuration
            {
                EmulatedJoyCon     = Config.FakeJoyCon,
                EnableMemoryChecks = !AOptimizations.DisableMemoryChecks,
                LoggingEnableDebug = (bool)Logger?.IsEnabled(LogLevel.Debug),
                LoggingEnableInfo  = (bool)Logger?.IsEnabled(LogLevel.Info),
                LoggingEnableWarn  = (bool)Logger?.IsEnabled(LogLevel.Warning),
                LoggingEnableError = (bool)Logger?.IsEnabled(LogLevel.Error),
                LoggingEnableStub  = (bool)Logger?.IsEnabled(LogLevel.Stub)
            };

            lock (ConfigPath)
            {
                JsonParser JsonParser = new JsonParser(ConfigPath);
                JsonParser.Save(Configuration);
            }
        }

        public static void LoadDefault()
        {
            Configuration configuration = new Configuration();
            Config.FakeJoyCon = new JoyCon();
            var Joycon = configuration.EmulatedJoyCon;

            Joycon.Left.StickUp      = (int)OpenTK.Input.Key.W;
            Joycon.Left.StickDown    = (int)OpenTK.Input.Key.S;
            Joycon.Left.StickLeft    = (int)OpenTK.Input.Key.A;
            Joycon.Left.StickRight   = (int)OpenTK.Input.Key.D;
            Joycon.Left.StickButton  = (int)OpenTK.Input.Key.F;
            Joycon.Left.DPadUp       = (int)OpenTK.Input.Key.Up;
            Joycon.Left.DPadDown     = (int)OpenTK.Input.Key.Down;
            Joycon.Left.DPadLeft     = (int)OpenTK.Input.Key.Left;
            Joycon.Left.DPadRight    = (int)OpenTK.Input.Key.Right;
            Joycon.Left.ButtonMinus  = (int)OpenTK.Input.Key.Minus;
            Joycon.Left.ButtonL      = (int)OpenTK.Input.Key.E;
            Joycon.Left.ButtonZL     = (int)OpenTK.Input.Key.Q;
            Joycon.Right.StickUp     = (int)OpenTK.Input.Key.I;
            Joycon.Right.StickDown   = (int)OpenTK.Input.Key.K;
            Joycon.Right.StickLeft   = (int)OpenTK.Input.Key.J;
            Joycon.Right.StickRight  = (int)OpenTK.Input.Key.L;
            Joycon.Right.StickButton = (int)OpenTK.Input.Key.H;
            Joycon.Right.ButtonA     = (int)OpenTK.Input.Key.Z;
            Joycon.Right.ButtonB     = (int)OpenTK.Input.Key.X;
            Joycon.Right.ButtonX     = (int)OpenTK.Input.Key.C;
            Joycon.Right.ButtonY     = (int)OpenTK.Input.Key.V;
            Joycon.Right.ButtonPlus  = (int)OpenTK.Input.Key.Plus;
            Joycon.Right.ButtonR     = (int)OpenTK.Input.Key.U;
            Joycon.Right.ButtonZR    = (int)OpenTK.Input.Key.O;

            Config.FakeJoyCon = Joycon;

            Write(null);
        }
    }

    public class JsonParser
    {
        private string ConfigPath;

        public JsonParser(string Path)
        {
            ConfigPath = Path;
            if (!File.Exists(ConfigPath))
            {
                File.CreateText(ConfigPath).Close();
                Settings.LoadDefault();
            }
        }

        private string Serialize<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj, typeof(T),Formatting.Indented ,null);
        }

        private T Deserialize<T>(string serialized_string)
        {
            return (T)JsonConvert.DeserializeObject<T>(serialized_string);
        }

        public void Save(Configuration Configuration)
        {
            string SerializeText = Serialize(Configuration);
            File.WriteAllText(ConfigPath, SerializeText);
        }

        public Configuration Load()
        {
            if (!File.Exists(ConfigPath))
                File.Create(ConfigPath).Close();
            string Config = File.ReadAllText(ConfigPath);
            return Deserialize<Configuration>(Config);
        }
    }
}
