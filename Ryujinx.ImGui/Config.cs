using Ryujinx.HLE;
using Ryujinx.HLE.Input;
using Ryujinx.HLE.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Ryujinx
{
    public static class Config
    {
        public static JoyCon FakeJoyCon { get; internal set; }

        public static string IniPath { get; set; }

        public static string DefaultGameDirectory { get; set; }

        public static void Read(Logger Log)
        {
            string IniFolder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            IniPath = Path.Combine(IniFolder, "RyujinxUI.conf");

            IniParser Parser = new IniParser(IniPath);

            AOptimizations.DisableMemoryChecks = !Convert.ToBoolean(Parser.GetValue("Enable_Memory_Checks"));

            Log.SetEnable(LogLevel.Debug,   Convert.ToBoolean(Parser.GetValue("Logging_Enable_Debug")));
            Log.SetEnable(LogLevel.Stub,    Convert.ToBoolean(Parser.GetValue("Logging_Enable_Stub")));
            Log.SetEnable(LogLevel.Info,    Convert.ToBoolean(Parser.GetValue("Logging_Enable_Info")));
            Log.SetEnable(LogLevel.Warning, Convert.ToBoolean(Parser.GetValue("Logging_Enable_Warn")));
            Log.SetEnable(LogLevel.Error,   Convert.ToBoolean(Parser.GetValue("Logging_Enable_Error")));

            string[] FilteredLogClasses = Parser.GetValue("Logging_Filtered_Classes").Split(',', StringSplitOptions.RemoveEmptyEntries);

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

            FakeJoyCon = new JoyCon
            {
                Left = new JoyConLeft
                {
                    StickUp     = Convert.ToInt16(Parser.GetValue("Controls_Left_FakeJoycon_Stick_Up")),
                    StickDown   = Convert.ToInt16(Parser.GetValue("Controls_Left_FakeJoycon_Stick_Down")),
                    StickLeft   = Convert.ToInt16(Parser.GetValue("Controls_Left_FakeJoycon_Stick_Left")),
                    StickRight  = Convert.ToInt16(Parser.GetValue("Controls_Left_FakeJoycon_Stick_Right")),
                    StickButton = Convert.ToInt16(Parser.GetValue("Controls_Left_FakeJoycon_Stick_Button")),
                    DPadUp      = Convert.ToInt16(Parser.GetValue("Controls_Left_FakeJoycon_DPad_Up")),
                    DPadDown    = Convert.ToInt16(Parser.GetValue("Controls_Left_FakeJoycon_DPad_Down")),
                    DPadLeft    = Convert.ToInt16(Parser.GetValue("Controls_Left_FakeJoycon_DPad_Left")),
                    DPadRight   = Convert.ToInt16(Parser.GetValue("Controls_Left_FakeJoycon_DPad_Right")),
                    ButtonMinus = Convert.ToInt16(Parser.GetValue("Controls_Left_FakeJoycon_Button_Minus")),
                    ButtonL     = Convert.ToInt16(Parser.GetValue("Controls_Left_FakeJoycon_Button_L")),
                    ButtonZL    = Convert.ToInt16(Parser.GetValue("Controls_Left_FakeJoycon_Button_ZL"))
                },

                Right = new JoyConRight
                {
                    StickUp     = Convert.ToInt16(Parser.GetValue("Controls_Right_FakeJoycon_Stick_Up")),
                    StickDown   = Convert.ToInt16(Parser.GetValue("Controls_Right_FakeJoycon_Stick_Down")),
                    StickLeft   = Convert.ToInt16(Parser.GetValue("Controls_Right_FakeJoycon_Stick_Left")),
                    StickRight  = Convert.ToInt16(Parser.GetValue("Controls_Right_FakeJoycon_Stick_Right")),
                    StickButton = Convert.ToInt16(Parser.GetValue("Controls_Right_FakeJoycon_Stick_Button")),
                    ButtonA     = Convert.ToInt16(Parser.GetValue("Controls_Right_FakeJoycon_Button_A")),
                    ButtonB     = Convert.ToInt16(Parser.GetValue("Controls_Right_FakeJoycon_Button_B")),
                    ButtonX     = Convert.ToInt16(Parser.GetValue("Controls_Right_FakeJoycon_Button_X")),
                    ButtonY     = Convert.ToInt16(Parser.GetValue("Controls_Right_FakeJoycon_Button_Y")),
                    ButtonPlus  = Convert.ToInt16(Parser.GetValue("Controls_Right_FakeJoycon_Button_Plus")),
                    ButtonR     = Convert.ToInt16(Parser.GetValue("Controls_Right_FakeJoycon_Button_R")),
                    ButtonZR    = Convert.ToInt16(Parser.GetValue("Controls_Right_FakeJoycon_Button_ZR"))
                }
            };

            DefaultGameDirectory = Parser.GetValue("Default_Game_Directory");

            if (string.IsNullOrWhiteSpace(DefaultGameDirectory))
            {
                VirtualFileSystem FS = new HLE.VirtualFileSystem();
                DefaultGameDirectory = Path.Combine(FS.GetSdCardPath(), "switch");
            }
        }

        public static void Save(Logger Log)
        {
            IniParser Parser = new IniParser(IniPath);

            Parser.SetValue("Enable_Memory_Checks", (!AOptimizations.DisableMemoryChecks).ToString());

            Parser.SetValue("Logging_Enable_Debug", Log.IsEnabled(LogLevel.Debug).ToString());
            Parser.SetValue("Logging_Enable_Stub",  Log.IsEnabled(LogLevel.Stub).ToString());
            Parser.SetValue("Logging_Enable_Info",  Log.IsEnabled(LogLevel.Info).ToString());
            Parser.SetValue("Logging_Enable_Warn",  Log.IsEnabled(LogLevel.Warning).ToString());
            Parser.SetValue("Logging_Enable_Error", Log.IsEnabled(LogLevel.Error).ToString());


            List<string> FilteredClasses = new List<string>();

            foreach(LogClass LogClass in Enum.GetValues(typeof(LogClass)))
            {
                if (Log.IsFiltered(LogClass))
                    FilteredClasses.Add(LogClass.ToString());
            }

            Parser.SetValue("Logging_Filtered_Classes", string.Join(',', FilteredClasses));

            Parser.SetValue("Controls_Left_FakeJoycon_Stick_Up", FakeJoyCon.Left.StickUp.ToString());
            Parser.SetValue("Controls_Left_FakeJoycon_Stick_Down", FakeJoyCon.Left.StickDown.ToString());
            Parser.SetValue("Controls_Left_FakeJoycon_Stick_Left", FakeJoyCon.Left.StickLeft.ToString());
            Parser.SetValue("Controls_Left_FakeJoycon_Stick_Right", FakeJoyCon.Left.StickRight.ToString());
            Parser.SetValue("Controls_Left_FakeJoycon_Stick_Button", FakeJoyCon.Left.StickButton.ToString());
            Parser.SetValue("Controls_Left_FakeJoycon_DPad_Up", FakeJoyCon.Left.DPadUp.ToString());
            Parser.SetValue("Controls_Left_FakeJoycon_DPad_Down", FakeJoyCon.Left.DPadDown.ToString());
            Parser.SetValue("Controls_Left_FakeJoycon_DPad_Left", FakeJoyCon.Left.DPadLeft.ToString());
            Parser.SetValue("Controls_Left_FakeJoycon_DPad_Right", FakeJoyCon.Left.DPadRight.ToString());
            Parser.SetValue("Controls_Left_FakeJoycon_Button_Minus", FakeJoyCon.Left.ButtonMinus.ToString());
            Parser.SetValue("Controls_Left_FakeJoycon_Button_L", FakeJoyCon.Left.ButtonL.ToString());
            Parser.SetValue("Controls_Left_FakeJoycon_Button_ZL", FakeJoyCon.Left.ButtonZL.ToString());

            Parser.SetValue("Controls_Right_FakeJoycon_Stick_Up", FakeJoyCon.Right.StickUp.ToString());
            Parser.SetValue("Controls_Right_FakeJoycon_Stick_Down", FakeJoyCon.Right.StickDown.ToString());
            Parser.SetValue("Controls_Right_FakeJoycon_Stick_Left", FakeJoyCon.Right.StickLeft.ToString());
            Parser.SetValue("Controls_Right_FakeJoycon_Stick_Right", FakeJoyCon.Right.StickRight.ToString());
            Parser.SetValue("Controls_Right_FakeJoycon_Stick_Button", FakeJoyCon.Right.StickButton.ToString());
            Parser.SetValue("Controls_Right_FakeJoycon_Button_A", FakeJoyCon.Right.StickUp.ToString());
            Parser.SetValue("Controls_Right_FakeJoycon_Button_B", FakeJoyCon.Right.StickUp.ToString());
            Parser.SetValue("Controls_Right_FakeJoycon_Button_X", FakeJoyCon.Right.StickUp.ToString());
            Parser.SetValue("Controls_Right_FakeJoycon_Button_Y", FakeJoyCon.Right.StickUp.ToString());
            Parser.SetValue("Controls_Right_FakeJoycon_Button_Plus", FakeJoyCon.Right.StickUp.ToString());
            Parser.SetValue("Controls_Right_FakeJoycon_Button_R", FakeJoyCon.Right.StickUp.ToString());
            Parser.SetValue("Controls_Right_FakeJoycon_Button_ZR", FakeJoyCon.Right.StickUp.ToString());

            Parser.SetValue("Default_Game_Directory", DefaultGameDirectory);

            Parser.Save();
        }
    }

    // https://stackoverflow.com/a/37772571
    public class IniParser
    {
        private readonly Dictionary<string, string> Values;
        private readonly Dictionary<string, string> Comments;
        private string Path;

        public IniParser(string Path)
        {
            this.Path = Path;
            string[] Lines = File.ReadAllLines(Path);
            Values = Lines
                .Where(Line => !string.IsNullOrWhiteSpace(Line) && !Line.StartsWith('#'))
                .Select(Line => Line.Split('=', 2))
                .ToDictionary(Parts => Parts[0].Trim(), Parts => Parts.Length > 1 ? Parts[1].Trim() : null);

            string CurrentComment = string.Empty;
            Comments = new Dictionary<string, string>();
            foreach (string Line in Lines)
            {
                if (Line.StartsWith("#"))
                    CurrentComment += Line + Environment.NewLine;
                else if (!string.IsNullOrWhiteSpace(Line))
                {
                    string key = Line.Split("=", 2).First().Trim();
                    Comments.Add(key, CurrentComment.TrimEnd());
                    CurrentComment = string.Empty;
                }
                else CurrentComment = string.Empty;
            }
        }

        public string GetValue(string Name)
        {
            return Values.TryGetValue(Name, out string Value) ? Value : null;
        }

        public void SetValue(string Name, string Value)
        {
            lock (Values)
                if (Values.ContainsKey(Name))
                {
                    Values[Name] = Value;
                }
                else
                {
                    Values.Add(Name, Value);
                }
        }

        public bool Save()
        {
            bool result = false;
            List<string> Records = new List<string>();
            foreach (var Record in Values)
            {
                if (Comments.ContainsKey(Record.Key))
                {
                    string Comment = Comments[Record.Key];
                    if (!string.IsNullOrWhiteSpace(Comment))
                    {
                        Records.Add(Environment.NewLine);
                        Records.Add(Comments[Record.Key]);
                    }
                }
                Records.Add(string.Format("{0} = {1}", Record.Key, Record.Value));
            }
            File.WriteAllLines(Path, Records);
            return result;
        }
    }
}
