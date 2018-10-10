using System;
using System.Collections.Generic;
using System.Text;

namespace Ryujinx.UI.Configuration
{
    partial class Settings
    {
        public Dictionary<string, object> DefaultSettings
        {
            get
            {
                Dictionary<string, object> Defaults = new Dictionary<string, object>()
                {
                    { "Enable_Memory_Checks", false},
                    { "Graphics_Shaders_Dump_Path", string.Empty },
                    { "Logging_Enable_Debug" , false},
                    { "Logging_Enable_Stub" , true},
                    { "Logging_Enable_Info", true },
                    { "Logging_Enable_Warn", true},
                    { "Logging_Enable_Error", true },
                    { "Logging_Filtered_Classes", string.Empty},
                    { "Docked_Mode", false},
                    { "Enable_Vsync", true},
                    { "Enable_MultiCore_Scheduling", false},
                    { "Enable_FS_Integrity_Checks", true},
                    { "GamePad_Index", 0},
                    { "GamePad_Deadzone", 0.05},
                    { "GamePad_Trigger_Threshold", 0.5},
                    { "GamePad_Enable", true},
                    { "Controls_Left_JoyConKeyboard_Stick_Up", 105 },
                    { "Controls_Left_JoyConKeyboard_Stick_Down", 101 },
                    { "Controls_Left_JoyConKeyboard_Stick_Left", 83 },
                    { "Controls_Left_JoyConKeyboard_Stick_Right", 86 },
                    { "Controls_Left_JoyConKeyboard_Stick_Button", 88 },
                    { "Controls_Left_JoyConKeyboard_DPad_Up", 45 },
                    { "Controls_Left_JoyConKeyboard_DPad_Down", 46 },
                    { "Controls_Left_JoyConKeyboard_DPad_Left", 47 },
                    { "Controls_Left_JoyConKeyboard_DPad_Right", 48 },
                    { "Controls_Left_JoyConKeyboard_Button_Minus", 120 },
                    { "Controls_Left_JoyConKeyboard_Button_L", 87 },
                    { "Controls_Left_JoyConKeyboard_Button_ZL", 99 },
                    { "Controls_Right_JoyConKeyboard_Stick_Up", 91 },
                    { "Controls_Right_JoyConKeyboard_Stick_Down", 93 },
                    { "Controls_Right_JoyConKeyboard_Stick_Left", 92 },
                    { "Controls_Right_JoyConKeyboard_Stick_Right", 94 },
                    { "Controls_Right_JoyConKeyboard_Stick_Button", 90 },
                    { "Controls_Right_JoyConKeyboard_Button_A", 108 },
                    { "Controls_Right_JoyConKeyboard_Button_B", 106 },
                    { "Controls_Right_JoyConKeyboard_Button_X", 85 },
                    { "Controls_Right_JoyConKeyboard_Button_Y", 104 },
                    { "Controls_Right_JoyConKeyboard_Button_Plus", 121 },
                    { "Controls_Right_JoyConKeyboard_Button_R", 103 },
                    { "Controls_Right_JoyConKeyboard_Button_ZR", 97 },
                    { "Controls_Left_JoyConController_Stick_Button", "LStick"},
                    { "Controls_Left_JoyConController_DPad_Up", "DPadUp" },
                    { "Controls_Left_JoyConController_DPad_Down", "DPadDown" },
                    { "Controls_Left_JoyConController_DPad_Left", "DPadLeft" },
                    { "Controls_Left_JoyConController_DPad_Right", "DPadRight" },
                    { "Controls_Left_JoyConController_Button_Minus", "Back" },
                    { "Controls_Left_JoyConController_Button_L", "LShoulder" },
                    { "Controls_Left_JoyConController_Button_ZL", "LTrigger" },
                    { "Controls_Right_JoyConController_Stick_Button", "RStick" },
                    { "Controls_Right_JoyConController_Button_A", "B" },
                    { "Controls_Right_JoyConController_Button_B", "A" },
                    { "Controls_Right_JoyConController_Button_X", "Y" },
                    { "Controls_Right_JoyConController_Button_Y", "X" },
                    { "Controls_Right_JoyConController_Button_Plus", "Start" },
                    { "Controls_Right_JoyConController_Button_R", "RShoulder" },
                    { "Controls_Right_JoyConController_Button_ZR", "RTrigger" },
                    { "Controls_Left_JoyConController_Stick", "LJoystick" },
                    { "Controls_Right_JoyConController_Stick", "RJoystick" }
                };

                return Defaults;
            }
        }
    }
}
