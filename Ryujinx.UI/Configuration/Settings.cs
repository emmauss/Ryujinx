using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using Ryujinx.HLE;
using Ryujinx.HLE;
using Ryujinx.HLE.Logging;
using Ryujinx.UI.Input;
using System;
using System.Globalization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using static Ryujinx.Config;

namespace Ryujinx.UI.Configuration
{
    public partial class Settings
    {
        private string Path;

        private Dictionary<string, object> SettingsDictionary { get; set; }

        public Settings(string Path)
        {
            if (!File.Exists(Path))
            {
                SettingsDictionary = DefaultSettings;
            }
            else
            {
                string SettingsString = File.ReadAllText(Path);

                SettingsDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(SettingsString);

                foreach((string Key, object Value) in DefaultSettings)
                {
                    if (!SettingsDictionary.ContainsKey(Key))
                    {
                        SettingsDictionary.TryAdd(Key, Value);
                    }
                }
            }

            this.Path = Path;
        }

        public T GetValue<T>(string Key)
        {
            object Value = null;

            if (!SettingsDictionary.TryGetValue(Key, out Value))
            {
                if (DefaultSettings.TryGetValue(Key, out Value))
                {
                    SettingsDictionary.TryAdd(Key, Value);
                }
            }

            return (T)Value;
        }

        public void Apply(Switch Device)
        {
            if (Device != null)
            {
                GraphicsConfig.ShadersDumpPath = GetValue<string>("Graphics_Shaders_Dump_Path");

                Device.Log.SetEnable(LogLevel.Debug,   GetValue<bool>("Logging_Enable_Debug"));
                Device.Log.SetEnable(LogLevel.Stub,    GetValue<bool>("Logging_Enable_Stub"));
                Device.Log.SetEnable(LogLevel.Info,    GetValue<bool>("Logging_Enable_Info"));
                Device.Log.SetEnable(LogLevel.Warning, GetValue<bool>("Logging_Enable_Warn"));
                Device.Log.SetEnable(LogLevel.Error,   GetValue<bool>("Logging_Enable_Error"));

                string[] FilteredLogClasses = GetValue<string>("Logging_Filtered_Classes").Split(',', StringSplitOptions.RemoveEmptyEntries);

                //When the classes are specified on the list, we only
                //enable the classes that are on the list.
                //So, first disable everything, then enable
                //the classes that the user added to the list.
                if (FilteredLogClasses.Length > 0)
                {
                    foreach (LogClass Class in Enum.GetValues(typeof(LogClass)))
                    {
                        Device.Log.SetEnable(Class, false);
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
                                Device.Log.SetEnable(Class, true);
                            }
                        }
                    }
                }

                Device.System.State.DockedMode = GetValue<bool>("Docked_Mode");
                Device.EnableDeviceVsync       = GetValue<bool>("Enable_Vsync");

                if (GetValue<bool>("Enable_MultiCore_Scheduling"))
                {
                    Device.System.EnableMultiCoreScheduling();
                }

                Device.System.EnableFsIntegrityChecks = GetValue<bool>("Enable_FS_Integrity_Checks");

                Config.JoyConKeyboard = new JoyConKeyboard(

                    new JoyConKeyboardLeft
                    {
                        StickUp     = GetValue<short>("Controls_Left_JoyConKeyboard_Stick_Up"),
                        StickDown   = GetValue<short>("Controls_Left_JoyConKeyboard_Stick_Down"),
                        StickLeft   = GetValue<short>("Controls_Left_JoyConKeyboard_Stick_Left"),
                        StickRight  = GetValue<short>("Controls_Left_JoyConKeyboard_Stick_Right"),
                        StickButton = GetValue<short>("Controls_Left_JoyConKeyboard_Stick_Button"),
                        DPadUp      = GetValue<short>("Controls_Left_JoyConKeyboard_DPad_Up"),
                        DPadDown    = GetValue<short>("Controls_Left_JoyConKeyboard_DPad_Down"),
                        DPadLeft    = GetValue<short>("Controls_Left_JoyConKeyboard_DPad_Left"),
                        DPadRight   = GetValue<short>("Controls_Left_JoyConKeyboard_DPad_Right"),
                        ButtonMinus = GetValue<short>("Controls_Left_JoyConKeyboard_Button_Minus"),
                        ButtonL     = GetValue<short>("Controls_Left_JoyConKeyboard_Button_L"),
                        ButtonZL    = GetValue<short>("Controls_Left_JoyConKeyboard_Button_ZL")
                    },

                    new JoyConKeyboardRight
                    {
                        StickUp     = GetValue<short>("Controls_Right_JoyConKeyboard_Stick_Up"),
                        StickDown   = GetValue<short>("Controls_Right_JoyConKeyboard_Stick_Down"),
                        StickLeft   = GetValue<short>("Controls_Right_JoyConKeyboard_Stick_Left"),
                        StickRight  = GetValue<short>("Controls_Right_JoyConKeyboard_Stick_Right"),
                        StickButton = GetValue<short>("Controls_Right_JoyConKeyboard_Stick_Button"),
                        ButtonA     = GetValue<short>("Controls_Right_JoyConKeyboard_Button_A"),
                        ButtonB     = GetValue<short>("Controls_Right_JoyConKeyboard_Button_B"),
                        ButtonX     = GetValue<short>("Controls_Right_JoyConKeyboard_Button_X"),
                        ButtonY     = GetValue<short>("Controls_Right_JoyConKeyboard_Button_Y"),
                        ButtonPlus  = GetValue<short>("Controls_Right_JoyConKeyboard_Button_Plus"),
                        ButtonR     = GetValue<short>("Controls_Right_JoyConKeyboard_Button_R"),
                        ButtonZR    = GetValue<short>("Controls_Right_JoyConKeyboard_Button_ZR")
                    });

                Config.JoyConController = new JoyConController(

                           GetValue<bool>("GamePad_Enable"),
                           GetValue<int>("GamePad_Index"),
                    (float)Convert.ToDouble(GetValue<float>("GamePad_Deadzone"), CultureInfo.InvariantCulture),
                    (float)Convert.ToDouble(GetValue<float>("GamePad_Trigger_Threshold"), CultureInfo.InvariantCulture),

                    new JoyConControllerLeft
                    {
                        Stick   = ToID(GetValue<string>("Controls_Left_JoyConController_Stick")),
                        StickButton = ToID(GetValue<string>("Controls_Left_JoyConController_Stick_Button")),
                        DPadUp      = ToID(GetValue<string>("Controls_Left_JoyConController_DPad_Up")),
                        DPadDown    = ToID(GetValue<string>("Controls_Left_JoyConController_DPad_Down")),
                        DPadLeft    = ToID(GetValue<string>("Controls_Left_JoyConController_DPad_Left")),
                        DPadRight   = ToID(GetValue<string>("Controls_Left_JoyConController_DPad_Right")),
                        ButtonMinus = ToID(GetValue<string>("Controls_Left_JoyConController_Button_Minus")),
                        ButtonL     = ToID(GetValue<string>("Controls_Left_JoyConController_Button_L")),
                        ButtonZL    = ToID(GetValue<string>("Controls_Left_JoyConController_Button_ZL"))
                    },

                    new JoyConControllerRight
                    {
                        Stick       = ToID(GetValue<string>("Controls_Right_JoyConController_Stick")),
                        StickButton = ToID(GetValue<string>("Controls_Right_JoyConController_Stick_Button")),
                        ButtonA     = ToID(GetValue<string>("Controls_Right_JoyConController_Button_A")),
                        ButtonB     = ToID(GetValue<string>("Controls_Right_JoyConController_Button_B")),
                        ButtonX     = ToID(GetValue<string>("Controls_Right_JoyConController_Button_X")),
                        ButtonY     = ToID(GetValue<string>("Controls_Right_JoyConController_Button_Y")),
                        ButtonPlus  = ToID(GetValue<string>("Controls_Right_JoyConController_Button_Plus")),
                        ButtonR     = ToID(GetValue<string>("Controls_Right_JoyConController_Button_R")),
                        ButtonZR    = ToID(GetValue<string>("Controls_Right_JoyConController_Button_ZR"))
                    });
            }
        }

        internal void SetValue(string Key, object Value)
        {
            SettingsDictionary[Key] = Value;
        }

        public void Commit()
        {
            lock (Path)
            {
                string SettingsString = JsonConvert.SerializeObject(SettingsDictionary);

                File.WriteAllText(Path, SettingsString);
            }
        }
    }
}
