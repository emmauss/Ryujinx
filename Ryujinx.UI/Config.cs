using Ryujinx.Common.Input;
using Ryujinx.HLE;
using Ryujinx.HLE.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Ryujinx
{
    public static class Config
    {
        public static JoyConKeyboard   JoyConKeyboard   { get; set; }
        public static JoyConController JoyConController { get; set; }

        public static float GamePadDeadzone             { get; private set; }
        public static bool  GamePadEnable               { get; private set; }
        public static int   GamePadIndex                { get; private set; }
        public static float GamePadTriggerThreshold     { get; private set; }

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

            GamePadEnable            =        Convert.ToBoolean(Parser.GetValue("GamePad_Enable"));
            GamePadIndex             =        Convert.ToInt32  (Parser.GetValue("GamePad_Index"));
            GamePadDeadzone          = (float)Convert.ToDouble (Parser.GetValue("GamePad_Deadzone"),          CultureInfo.InvariantCulture);
            GamePadTriggerThreshold  = (float)Convert.ToDouble (Parser.GetValue("GamePad_Trigger_Threshold"), CultureInfo.InvariantCulture);

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

            JoyConKeyboard = new JoyConKeyboard
            {
                Left = new JoyConKeyboardLeft
                {
                    StickUp     = Convert.ToInt16(Parser.GetValue("Controls_Left_JoyConKeyboard_Stick_Up")),
                    StickDown   = Convert.ToInt16(Parser.GetValue("Controls_Left_JoyConKeyboard_Stick_Down")),
                    StickLeft   = Convert.ToInt16(Parser.GetValue("Controls_Left_JoyConKeyboard_Stick_Left")),
                    StickRight  = Convert.ToInt16(Parser.GetValue("Controls_Left_JoyConKeyboard_Stick_Right")),
                    StickButton = Convert.ToInt16(Parser.GetValue("Controls_Left_JoyConKeyboard_Stick_Button")),
                    DPadUp      = Convert.ToInt16(Parser.GetValue("Controls_Left_JoyConKeyboard_DPad_Up")),
                    DPadDown    = Convert.ToInt16(Parser.GetValue("Controls_Left_JoyConKeyboard_DPad_Down")),
                    DPadLeft    = Convert.ToInt16(Parser.GetValue("Controls_Left_JoyConKeyboard_DPad_Left")),
                    DPadRight   = Convert.ToInt16(Parser.GetValue("Controls_Left_JoyConKeyboard_DPad_Right")),
                    ButtonMinus = Convert.ToInt16(Parser.GetValue("Controls_Left_JoyConKeyboard_Button_Minus")),
                    ButtonL     = Convert.ToInt16(Parser.GetValue("Controls_Left_JoyConKeyboard_Button_L")),
                    ButtonZL    = Convert.ToInt16(Parser.GetValue("Controls_Left_JoyConKeyboard_Button_ZL"))
                },

                Right = new JoyConKeyboardRight
                {
                    StickUp     = Convert.ToInt16(Parser.GetValue("Controls_Right_JoyConKeyboard_Stick_Up")),
                    StickDown   = Convert.ToInt16(Parser.GetValue("Controls_Right_JoyConKeyboard_Stick_Down")),
                    StickLeft   = Convert.ToInt16(Parser.GetValue("Controls_Right_JoyConKeyboard_Stick_Left")),
                    StickRight  = Convert.ToInt16(Parser.GetValue("Controls_Right_JoyConKeyboard_Stick_Right")),
                    StickButton = Convert.ToInt16(Parser.GetValue("Controls_Right_JoyConKeyboard_Stick_Button")),
                    ButtonA     = Convert.ToInt16(Parser.GetValue("Controls_Right_JoyConKeyboard_Button_A")),
                    ButtonB     = Convert.ToInt16(Parser.GetValue("Controls_Right_JoyConKeyboard_Button_B")),
                    ButtonX     = Convert.ToInt16(Parser.GetValue("Controls_Right_JoyConKeyboard_Button_X")),
                    ButtonY     = Convert.ToInt16(Parser.GetValue("Controls_Right_JoyConKeyboard_Button_Y")),
                    ButtonPlus  = Convert.ToInt16(Parser.GetValue("Controls_Right_JoyConKeyboard_Button_Plus")),
                    ButtonR     = Convert.ToInt16(Parser.GetValue("Controls_Right_JoyConKeyboard_Button_R")),
                    ButtonZR    = Convert.ToInt16(Parser.GetValue("Controls_Right_JoyConKeyboard_Button_ZR"))
                }
            };

            JoyConController = new JoyConController
            {
                Left = new JoyConControllerLeft
                {
                    Stick       = Parser.GetValue("Controls_Left_JoyConController_Stick"),
                    StickButton = Parser.GetValue("Controls_Left_JoyConController_Stick_Button"),
                    DPadUp      = Parser.GetValue("Controls_Left_JoyConController_DPad_Up"),
                    DPadDown    = Parser.GetValue("Controls_Left_JoyConController_DPad_Down"),
                    DPadLeft    = Parser.GetValue("Controls_Left_JoyConController_DPad_Left"),
                    DPadRight   = Parser.GetValue("Controls_Left_JoyConController_DPad_Right"),
                    ButtonMinus = Parser.GetValue("Controls_Left_JoyConController_Button_Minus"),
                    ButtonL     = Parser.GetValue("Controls_Left_JoyConController_Button_L"),
                    ButtonZL    = Parser.GetValue("Controls_Left_JoyConController_Button_ZL")
                },

                Right = new JoyConControllerRight
                {
                    Stick       = Parser.GetValue("Controls_Right_JoyConController_Stick"),
                    StickButton = Parser.GetValue("Controls_Right_JoyConController_Stick_Button"),
                    ButtonA     = Parser.GetValue("Controls_Right_JoyConController_Button_A"),
                    ButtonB     = Parser.GetValue("Controls_Right_JoyConController_Button_B"),
                    ButtonX     = Parser.GetValue("Controls_Right_JoyConController_Button_X"),
                    ButtonY     = Parser.GetValue("Controls_Right_JoyConController_Button_Y"),
                    ButtonPlus  = Parser.GetValue("Controls_Right_JoyConController_Button_Plus"),
                    ButtonR     = Parser.GetValue("Controls_Right_JoyConController_Button_R"),
                    ButtonZR    = Parser.GetValue("Controls_Right_JoyConController_Button_ZR")
                }
            };

            DefaultGameDirectory = Parser.GetValue("Default_Game_Directory");

            VirtualFileSystem FS = new HLE.VirtualFileSystem();

            if (string.IsNullOrWhiteSpace(DefaultGameDirectory))
            {                
                DefaultGameDirectory = Path.Combine(FS.GetSdCardPath(), "switch");
            }

            if (!Directory.Exists(DefaultGameDirectory))
            {
                if (!Directory.CreateDirectory(DefaultGameDirectory).Exists)
                {
                    DefaultGameDirectory = Path.Combine(FS.GetSdCardPath(), "switch");
                    Directory.CreateDirectory(DefaultGameDirectory);
                }
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

            Parser.SetValue("Controls_Left_JoyConKeyboard_Stick_Up",     JoyConKeyboard.Left.StickUp.ToString());
            Parser.SetValue("Controls_Left_JoyConKeyboard_Stick_Down",   JoyConKeyboard.Left.StickDown.ToString());
            Parser.SetValue("Controls_Left_JoyConKeyboard_Stick_Left",   JoyConKeyboard.Left.StickLeft.ToString());
            Parser.SetValue("Controls_Left_JoyConKeyboard_Stick_Right",  JoyConKeyboard.Left.StickRight.ToString());
            Parser.SetValue("Controls_Left_JoyConKeyboard_Stick_Button", JoyConKeyboard.Left.StickButton.ToString());
            Parser.SetValue("Controls_Left_JoyConKeyboard_DPad_Up",      JoyConKeyboard.Left.DPadUp.ToString());
            Parser.SetValue("Controls_Left_JoyConKeyboard_DPad_Down",    JoyConKeyboard.Left.DPadDown.ToString());
            Parser.SetValue("Controls_Left_JoyConKeyboard_DPad_Left",    JoyConKeyboard.Left.DPadLeft.ToString());
            Parser.SetValue("Controls_Left_JoyConKeyboard_DPad_Right",   JoyConKeyboard.Left.DPadRight.ToString());
            Parser.SetValue("Controls_Left_JoyConKeyboard_Button_Minus", JoyConKeyboard.Left.ButtonMinus.ToString());
            Parser.SetValue("Controls_Left_JoyConKeyboard_Button_L",     JoyConKeyboard.Left.ButtonL.ToString());
            Parser.SetValue("Controls_Left_JoyConKeyboard_Button_ZL",    JoyConKeyboard.Left.ButtonZL.ToString());

            Parser.SetValue("Controls_Right_JoyConKeyboard_Stick_Up",     JoyConKeyboard.Right.StickUp.ToString());
            Parser.SetValue("Controls_Right_JoyConKeyboard_Stick_Down",   JoyConKeyboard.Right.StickDown.ToString());
            Parser.SetValue("Controls_Right_JoyConKeyboard_Stick_Left",   JoyConKeyboard.Right.StickLeft.ToString());
            Parser.SetValue("Controls_Right_JoyConKeyboard_Stick_Right",  JoyConKeyboard.Right.StickRight.ToString());
            Parser.SetValue("Controls_Right_JoyConKeyboard_Stick_Button", JoyConKeyboard.Right.StickButton.ToString());
            Parser.SetValue("Controls_Right_JoyConKeyboard_Button_A",     JoyConKeyboard.Right.ButtonA.ToString());
            Parser.SetValue("Controls_Right_JoyConKeyboard_Button_B",     JoyConKeyboard.Right.ButtonB.ToString());
            Parser.SetValue("Controls_Right_JoyConKeyboard_Button_X",     JoyConKeyboard.Right.ButtonX.ToString());
            Parser.SetValue("Controls_Right_JoyConKeyboard_Button_Y",     JoyConKeyboard.Right.ButtonY.ToString());
            Parser.SetValue("Controls_Right_JoyConKeyboard_Button_Plus",  JoyConKeyboard.Right.ButtonPlus.ToString());
            Parser.SetValue("Controls_Right_JoyConKeyboard_Button_R",     JoyConKeyboard.Right.ButtonR.ToString());
            Parser.SetValue("Controls_Right_JoyConKeyboard_Button_ZR",    JoyConKeyboard.Right.ButtonZR.ToString());

            Parser.SetValue("Controls_Left_JoyConController_Stick",        JoyConController.Left.Stick.ToString());
            Parser.SetValue("Controls_Left_JoyConController_Stick_Button", JoyConController.Left.StickButton.ToString());
            Parser.SetValue("Controls_Left_JoyConController_DPad_Up",      JoyConController.Left.DPadUp.ToString());
            Parser.SetValue("Controls_Left_JoyConController_DPad_Down",    JoyConController.Left.DPadDown.ToString());
            Parser.SetValue("Controls_Left_JoyConController_DPad_Left",    JoyConController.Left.DPadLeft.ToString());
            Parser.SetValue("Controls_Left_JoyConController_DPad_Right",   JoyConController.Left.DPadRight.ToString());
            Parser.SetValue("Controls_Left_JoyConController_Button_Minus", JoyConController.Left.ButtonMinus.ToString());
            Parser.SetValue("Controls_Left_JoyConController_Button_L",     JoyConController.Left.ButtonL.ToString());
            Parser.SetValue("Controls_Left_JoyConController_Button_ZL",    JoyConController.Left.ButtonZL.ToString());

            Parser.SetValue("Controls_Right_JoyConController_Stick_Up",     JoyConController.Right.Stick.ToString());
            Parser.SetValue("Controls_Right_JoyConController_Stick_Button", JoyConController.Right.StickButton.ToString());
            Parser.SetValue("Controls_Right_JoyConController_Button_A",     JoyConController.Right.ButtonA.ToString());
            Parser.SetValue("Controls_Right_JoyConController_Button_B",     JoyConController.Right.ButtonB.ToString());
            Parser.SetValue("Controls_Right_JoyConController_Button_X",     JoyConController.Right.ButtonX.ToString());
            Parser.SetValue("Controls_Right_JoyConController_Button_Y",     JoyConController.Right.ButtonY.ToString());
            Parser.SetValue("Controls_Right_JoyConController_Button_Plus",  JoyConController.Right.ButtonPlus.ToString());
            Parser.SetValue("Controls_Right_JoyConController_Button_R",     JoyConController.Right.ButtonR.ToString());
            Parser.SetValue("Controls_Right_JoyConController_Button_ZR",    JoyConController.Right.ButtonZR.ToString());

            Parser.SetValue("GamePad_Enable",            GamePadEnable.ToString());
            Parser.SetValue("GamePad_Index",             GamePadIndex.ToString());
            Parser.SetValue("GamePad_Deadzone",          Convert.ToString(GamePadDeadzone,CultureInfo.InvariantCulture));
            Parser.SetValue("GamePad_Trigger_Threshold", GamePadTriggerThreshold.ToString());

            Parser.SetValue("Default_Game_Directory", DefaultGameDirectory);

            Parser.Save();
        }
    }

    // https://stackoverflow.com/a/37772571
    public class IniParser
    {
        private readonly Dictionary<string, string> Values;
        private readonly Dictionary<string, string> Comments;
        private readonly string Path;

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
