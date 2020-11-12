using System;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Ryujinx.Common.Configuration.Hid;
using Ryujinx.Configuration;
using Ryujinx.HLE.HOS.Services.Hid;

namespace Ryujinx.Skia.Ui
{
    [Flags]
    public enum HotkeyButtons
    {
        ToggleVSync = 1 << 0,
    }

    public class KeyboardController
    {
        private readonly KeyboardConfig _config;
        private readonly KeyboardState _keyboard;

        public KeyboardController(KeyboardConfig config, KeyboardState keyboard)
        {
            _config = config;
            _keyboard = keyboard;
        }

        public ControllerKeys GetButtons()
        {
            ControllerKeys buttons = 0;

            if (CheckBounded(_config.LeftJoycon.StickButton)) buttons |= ControllerKeys.LStick;
            if (CheckBounded(_config.LeftJoycon.DPadUp))      buttons |= ControllerKeys.DpadUp;
            if (CheckBounded(_config.LeftJoycon.DPadDown))    buttons |= ControllerKeys.DpadDown;
            if (CheckBounded(_config.LeftJoycon.DPadLeft))    buttons |= ControllerKeys.DpadLeft;
            if (CheckBounded(_config.LeftJoycon.DPadRight))   buttons |= ControllerKeys.DpadRight;
            if (CheckBounded(_config.LeftJoycon.ButtonMinus)) buttons |= ControllerKeys.Minus;
            if (CheckBounded(_config.LeftJoycon.ButtonL))     buttons |= ControllerKeys.L;
            if (CheckBounded(_config.LeftJoycon.ButtonZl))    buttons |= ControllerKeys.Zl;
            if (CheckBounded(_config.LeftJoycon.ButtonSl))    buttons |= ControllerKeys.SlLeft;
            if (CheckBounded(_config.LeftJoycon.ButtonSr))    buttons |= ControllerKeys.SlRight;
            
            if (CheckBounded(_config.RightJoycon.StickButton)) buttons |= ControllerKeys.RStick;
            if (CheckBounded(_config.RightJoycon.ButtonA))     buttons |= ControllerKeys.A;
            if (CheckBounded(_config.RightJoycon.ButtonB))     buttons |= ControllerKeys.B;
            if (CheckBounded(_config.RightJoycon.ButtonX))     buttons |= ControllerKeys.X;
            if (CheckBounded(_config.RightJoycon.ButtonY))     buttons |= ControllerKeys.Y;
            if (CheckBounded(_config.RightJoycon.ButtonPlus))  buttons |= ControllerKeys.Plus;
            if (CheckBounded(_config.RightJoycon.ButtonR))     buttons |= ControllerKeys.R;
            if (CheckBounded(_config.RightJoycon.ButtonZr))    buttons |= ControllerKeys.Zr;
            if (CheckBounded(_config.RightJoycon.ButtonSl))    buttons |= ControllerKeys.SrLeft;
            if (CheckBounded(_config.RightJoycon.ButtonSr))    buttons |= ControllerKeys.SrRight;

            return buttons;
        }

        public bool CheckBounded(Configuration.Hid.Key key)
        {
            if (key == Configuration.Hid.Key.Unbound)
            {
                return false;
            }
            // if ((int)key > (int)Keys.Menu)
            // {
            //     key = (Configuration.Hid.Key)((int)key + 1);
            // }
            return _keyboard[(Keys)key];
        }

        public (short, short) GetLeftStick()
        {
            short dx = 0;
            short dy = 0;

            if (CheckBounded(_config.LeftJoycon.StickUp))    dy =  short.MaxValue;
            if (CheckBounded(_config.LeftJoycon.StickDown))  dy = -short.MaxValue;
            if (CheckBounded(_config.LeftJoycon.StickLeft))  dx = -short.MaxValue;
            if (CheckBounded(_config.LeftJoycon.StickRight)) dx =  short.MaxValue;

            return (dx, dy);
        }

        public (short, short) GetRightStick()
        {
            short dx = 0;
            short dy = 0;

            if (CheckBounded(_config.RightJoycon.StickUp))    dy =  short.MaxValue;
            if (CheckBounded(_config.RightJoycon.StickDown))  dy = -short.MaxValue;
            if (CheckBounded(_config.RightJoycon.StickLeft))  dx = -short.MaxValue;
            if (CheckBounded(_config.RightJoycon.StickRight)) dx =  short.MaxValue;

            return (dx, dy);
        }

        public static HotkeyButtons GetHotkeyButtons(KeyboardState keyboard)
        {
            HotkeyButtons buttons = 0;

            if (keyboard[(Keys)ConfigurationState.Instance.Hid.Hotkeys.Value.ToggleVsync])
            {
                buttons |= HotkeyButtons.ToggleVSync;
            }

            return buttons;
        }

        class KeyMappingEntry
        {
            public Keys TargetKey;
            public byte Target;
        }

        private static readonly KeyMappingEntry[] KeyMapping = new KeyMappingEntry[]
        {
            new KeyMappingEntry { TargetKey = Keys.A, Target = 0x4  },
            new KeyMappingEntry { TargetKey = Keys.B, Target = 0x5  },
            new KeyMappingEntry { TargetKey = Keys.C, Target = 0x6  },
            new KeyMappingEntry { TargetKey = Keys.D, Target = 0x7  },
            new KeyMappingEntry { TargetKey = Keys.E, Target = 0x8  },
            new KeyMappingEntry { TargetKey = Keys.F, Target = 0x9  },
            new KeyMappingEntry { TargetKey = Keys.G, Target = 0xA  },
            new KeyMappingEntry { TargetKey = Keys.H, Target = 0xB  },
            new KeyMappingEntry { TargetKey = Keys.I, Target = 0xC  },
            new KeyMappingEntry { TargetKey = Keys.J, Target = 0xD  },
            new KeyMappingEntry { TargetKey = Keys.K, Target = 0xE  },
            new KeyMappingEntry { TargetKey = Keys.L, Target = 0xF  },
            new KeyMappingEntry { TargetKey = Keys.M, Target = 0x10 },
            new KeyMappingEntry { TargetKey = Keys.N, Target = 0x11 },
            new KeyMappingEntry { TargetKey = Keys.O, Target = 0x12 },
            new KeyMappingEntry { TargetKey = Keys.P, Target = 0x13 },
            new KeyMappingEntry { TargetKey = Keys.Q, Target = 0x14 },
            new KeyMappingEntry { TargetKey = Keys.R, Target = 0x15 },
            new KeyMappingEntry { TargetKey = Keys.S, Target = 0x16 },
            new KeyMappingEntry { TargetKey = Keys.T, Target = 0x17 },
            new KeyMappingEntry { TargetKey = Keys.U, Target = 0x18 },
            new KeyMappingEntry { TargetKey = Keys.V, Target = 0x19 },
            new KeyMappingEntry { TargetKey = Keys.W, Target = 0x1A },
            new KeyMappingEntry { TargetKey = Keys.X, Target = 0x1B },
            new KeyMappingEntry { TargetKey = Keys.Y, Target = 0x1C },
            new KeyMappingEntry { TargetKey = Keys.Z, Target = 0x1D },

            new KeyMappingEntry { TargetKey = Keys.D1, Target = 0x1E },
            new KeyMappingEntry { TargetKey = Keys.D2, Target = 0x1F },
            new KeyMappingEntry { TargetKey = Keys.D3, Target = 0x20 },
            new KeyMappingEntry { TargetKey = Keys.D4, Target = 0x21 },
            new KeyMappingEntry { TargetKey = Keys.D5, Target = 0x22 },
            new KeyMappingEntry { TargetKey = Keys.D6, Target = 0x23 },
            new KeyMappingEntry { TargetKey = Keys.D7, Target = 0x24 },
            new KeyMappingEntry { TargetKey = Keys.D8, Target = 0x25 },
            new KeyMappingEntry { TargetKey = Keys.D9, Target = 0x26 },
            new KeyMappingEntry { TargetKey = Keys.D0, Target = 0x27 },

            new KeyMappingEntry { TargetKey = Keys.Enter,        Target = 0x28 },
            new KeyMappingEntry { TargetKey = Keys.Escape,       Target = 0x29 },
            new KeyMappingEntry { TargetKey = Keys.Backspace,    Target = 0x2A },
            new KeyMappingEntry { TargetKey = Keys.Tab,          Target = 0x2B },
            new KeyMappingEntry { TargetKey = Keys.Space,        Target = 0x2C },
            new KeyMappingEntry { TargetKey = Keys.Minus,        Target = 0x2D },
            new KeyMappingEntry { TargetKey = Keys.KeyPadAdd,    Target = 0x2E },
            new KeyMappingEntry { TargetKey = Keys.LeftBracket,  Target = 0x2F },
            new KeyMappingEntry { TargetKey = Keys.RightBracket, Target = 0x30 },
            new KeyMappingEntry { TargetKey = Keys.Backslash,    Target = 0x31 },
            new KeyMappingEntry { TargetKey = Keys.GraveAccent,  Target = 0x32 },
            new KeyMappingEntry { TargetKey = Keys.Semicolon,    Target = 0x33 },
            new KeyMappingEntry { TargetKey = Keys.Apostrophe,   Target = 0x34 },
            new KeyMappingEntry { TargetKey = Keys.GraveAccent,  Target = 0x35 },
            new KeyMappingEntry { TargetKey = Keys.Comma,        Target = 0x36 },
            new KeyMappingEntry { TargetKey = Keys.Period,       Target = 0x37 },
            new KeyMappingEntry { TargetKey = Keys.Slash,        Target = 0x38 },
            new KeyMappingEntry { TargetKey = Keys.CapsLock,     Target = 0x39 },

            new KeyMappingEntry { TargetKey = Keys.F1,  Target = 0x3a },
            new KeyMappingEntry { TargetKey = Keys.F2,  Target = 0x3b },
            new KeyMappingEntry { TargetKey = Keys.F3,  Target = 0x3c },
            new KeyMappingEntry { TargetKey = Keys.F4,  Target = 0x3d },
            new KeyMappingEntry { TargetKey = Keys.F5,  Target = 0x3e },
            new KeyMappingEntry { TargetKey = Keys.F6,  Target = 0x3f },
            new KeyMappingEntry { TargetKey = Keys.F7,  Target = 0x40 },
            new KeyMappingEntry { TargetKey = Keys.F8,  Target = 0x41 },
            new KeyMappingEntry { TargetKey = Keys.F9,  Target = 0x42 },
            new KeyMappingEntry { TargetKey = Keys.F10, Target = 0x43 },
            new KeyMappingEntry { TargetKey = Keys.F11, Target = 0x44 },
            new KeyMappingEntry { TargetKey = Keys.F12, Target = 0x45 },

            new KeyMappingEntry { TargetKey = Keys.PrintScreen, Target = 0x46 },
            new KeyMappingEntry { TargetKey = Keys.ScrollLock,  Target = 0x47 },
            new KeyMappingEntry { TargetKey = Keys.Pause,       Target = 0x48 },
            new KeyMappingEntry { TargetKey = Keys.Insert,      Target = 0x49 },
            new KeyMappingEntry { TargetKey = Keys.Home,        Target = 0x4A },
            new KeyMappingEntry { TargetKey = Keys.PageUp,      Target = 0x4B },
            new KeyMappingEntry { TargetKey = Keys.Delete,      Target = 0x4C },
            new KeyMappingEntry { TargetKey = Keys.End,         Target = 0x4D },
            new KeyMappingEntry { TargetKey = Keys.PageDown,    Target = 0x4E },
            new KeyMappingEntry { TargetKey = Keys.Right,       Target = 0x4F },
            new KeyMappingEntry { TargetKey = Keys.Left,        Target = 0x50 },
            new KeyMappingEntry { TargetKey = Keys.Down,        Target = 0x51 },
            new KeyMappingEntry { TargetKey = Keys.Up,          Target = 0x52 },

            new KeyMappingEntry { TargetKey = Keys.NumLock,        Target = 0x53 },
            new KeyMappingEntry { TargetKey = Keys.KeyPadDivide,   Target = 0x54 },
            new KeyMappingEntry { TargetKey = Keys.KeyPadMultiply, Target = 0x55 },
            new KeyMappingEntry { TargetKey = Keys.Minus,          Target = 0x56 },
            new KeyMappingEntry { TargetKey = Keys.KeyPadAdd,      Target = 0x57 },
            new KeyMappingEntry { TargetKey = Keys.KeyPadEnter,    Target = 0x58 },
            new KeyMappingEntry { TargetKey = Keys.KeyPad1,        Target = 0x59 },
            new KeyMappingEntry { TargetKey = Keys.KeyPad2,        Target = 0x5A },
            new KeyMappingEntry { TargetKey = Keys.KeyPad3,        Target = 0x5B },
            new KeyMappingEntry { TargetKey = Keys.KeyPad4,        Target = 0x5C },
            new KeyMappingEntry { TargetKey = Keys.KeyPad5,        Target = 0x5D },
            new KeyMappingEntry { TargetKey = Keys.KeyPad6,        Target = 0x5E },
            new KeyMappingEntry { TargetKey = Keys.KeyPad7,        Target = 0x5F },
            new KeyMappingEntry { TargetKey = Keys.KeyPad8,        Target = 0x60 },
            new KeyMappingEntry { TargetKey = Keys.KeyPad9,        Target = 0x61 },
            new KeyMappingEntry { TargetKey = Keys.KeyPad0,        Target = 0x62 },
            new KeyMappingEntry { TargetKey = Keys.Period,         Target = 0x63 },

            new KeyMappingEntry { TargetKey = Keys.Backslash, Target = 0x64 },

            new KeyMappingEntry { TargetKey = Keys.F13, Target = 0x68 },
            new KeyMappingEntry { TargetKey = Keys.F14, Target = 0x69 },
            new KeyMappingEntry { TargetKey = Keys.F15, Target = 0x6A },
            new KeyMappingEntry { TargetKey = Keys.F16, Target = 0x6B },
            new KeyMappingEntry { TargetKey = Keys.F17, Target = 0x6C },
            new KeyMappingEntry { TargetKey = Keys.F18, Target = 0x6D },
            new KeyMappingEntry { TargetKey = Keys.F19, Target = 0x6E },
            new KeyMappingEntry { TargetKey = Keys.F20, Target = 0x6F },
            new KeyMappingEntry { TargetKey = Keys.F21, Target = 0x70 },
            new KeyMappingEntry { TargetKey = Keys.F22, Target = 0x71 },
            new KeyMappingEntry { TargetKey = Keys.F23, Target = 0x72 },
            new KeyMappingEntry { TargetKey = Keys.F24, Target = 0x73 },

            new KeyMappingEntry { TargetKey = Keys.LeftControl,  Target = 0xE0 },
            new KeyMappingEntry { TargetKey = Keys.LeftShift,    Target = 0xE1 },
            new KeyMappingEntry { TargetKey = Keys.LeftAlt,      Target = 0xE2 },
            new KeyMappingEntry { TargetKey = Keys.LeftSuper,    Target = 0xE3 },
            new KeyMappingEntry { TargetKey = Keys.RightControl, Target = 0xE4 },
            new KeyMappingEntry { TargetKey = Keys.RightShift,   Target = 0xE5 },
            new KeyMappingEntry { TargetKey = Keys.RightAlt,     Target = 0xE6 },
            new KeyMappingEntry { TargetKey = Keys.RightSuper,   Target = 0xE7 },
        };

        private static readonly KeyMappingEntry[] KeyModifierMapping = new KeyMappingEntry[]
        {
            new KeyMappingEntry { TargetKey = Keys.LeftControl,  Target = 0 },
            new KeyMappingEntry { TargetKey = Keys.LeftShift,    Target = 1 },
            new KeyMappingEntry { TargetKey = Keys.LeftAlt,      Target = 2 },
            new KeyMappingEntry { TargetKey = Keys.LeftSuper,    Target = 3 },
            new KeyMappingEntry { TargetKey = Keys.RightControl, Target = 4 },
            new KeyMappingEntry { TargetKey = Keys.RightShift,   Target = 5 },
            new KeyMappingEntry { TargetKey = Keys.RightAlt,     Target = 6 },
            new KeyMappingEntry { TargetKey = Keys.RightSuper,   Target = 7 },
            new KeyMappingEntry { TargetKey = Keys.CapsLock,     Target = 8 },
            new KeyMappingEntry { TargetKey = Keys.ScrollLock,   Target = 9 },
            new KeyMappingEntry { TargetKey = Keys.NumLock,      Target = 10 },
        };

        public KeyboardInput GetKeysDown()
        {
            KeyboardInput hidKeyboard = new KeyboardInput
            {
                Modifier = 0,
                Keys = new int[0x8]
            };

            foreach (KeyMappingEntry entry in KeyMapping)
            {
                int value = _keyboard[entry.TargetKey] ? 1 : 0;

                hidKeyboard.Keys[entry.Target / 0x20] |= (value << (entry.Target % 0x20));
            }

            foreach (KeyMappingEntry entry in KeyModifierMapping)
            {
                int value = _keyboard[entry.TargetKey] ? 1 : 0;

                hidKeyboard.Modifier |= value << entry.Target;
            }

            return hidKeyboard;
        }
    }
}
