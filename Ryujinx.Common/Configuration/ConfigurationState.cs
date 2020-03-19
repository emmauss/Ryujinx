﻿using Ryujinx.Common;
using Ryujinx.Common.Configuration.Hid;
using Ryujinx.Common.Logging;
using Ryujinx.Configuration.Hid;
using Ryujinx.Configuration.System;
using Ryujinx.UI.Input;
using System;
using System.Collections.Generic;

namespace Ryujinx.Configuration
{
    public class ConfigurationState
    {
        /// <summary>
        /// UI configuration section
        /// </summary>
        public class UiSection
        {
            /// <summary>
            /// A list of directories containing games to be used to load games into the games list
            /// </summary>
            public ReactiveObject<List<string>> GameDirs { get; private set; }

            /// <summary>
            /// Enable or disable custom themes in the GUI
            /// </summary>
            public ReactiveObject<bool> EnableCustomTheme { get; private set; }

            /// <summary>
            /// Path to custom GUI theme
            /// </summary>
            public ReactiveObject<string> CustomThemePath { get; private set; }

            public UiSection()
            {
                GameDirs          = new ReactiveObject<List<string>>();
                EnableCustomTheme = new ReactiveObject<bool>();
                CustomThemePath   = new ReactiveObject<string>();
            }
        }

        /// <summary>
        /// Logger configuration section
        /// </summary>
        public class LoggerSection
        {
            /// <summary>
            /// Enables printing debug log messages
            /// </summary>
            public ReactiveObject<bool> EnableDebug { get; private set; }

            /// <summary>
            /// Enables printing stub log messages
            /// </summary>
            public ReactiveObject<bool> EnableStub { get; private set; }

            /// <summary>
            /// Enables printing info log messages
            /// </summary>
            public ReactiveObject<bool> EnableInfo { get; private set; }

            /// <summary>
            /// Enables printing warning log messages
            /// </summary>
            public ReactiveObject<bool> EnableWarn { get; private set; }

            /// <summary>
            /// Enables printing error log messages
            /// </summary>
            public ReactiveObject<bool> EnableError { get; private set; }

            /// <summary>
            /// Enables printing guest log messages
            /// </summary>
            public ReactiveObject<bool> EnableGuest { get; private set; }

            /// <summary>
            /// Enables printing FS access log messages
            /// </summary>
            public ReactiveObject<bool> EnableFsAccessLog { get; private set; }

            /// <summary>
            /// Controls which log messages are written to the log targets
            /// </summary>
            public ReactiveObject<LogClass[]> FilteredClasses { get; private set; }

            /// <summary>
            /// Enables or disables logging to a file on disk
            /// </summary>
            public ReactiveObject<bool> EnableFileLog { get; private set; }

            public LoggerSection()
            {
                EnableDebug       = new ReactiveObject<bool>();
                EnableStub        = new ReactiveObject<bool>();
                EnableInfo        = new ReactiveObject<bool>();
                EnableWarn        = new ReactiveObject<bool>();
                EnableError       = new ReactiveObject<bool>();
                EnableGuest       = new ReactiveObject<bool>();
                EnableFsAccessLog = new ReactiveObject<bool>();
                FilteredClasses   = new ReactiveObject<LogClass[]>();
                EnableFileLog     = new ReactiveObject<bool>();
            }
        }

        /// <summary>
        /// System configuration section
        /// </summary>
        public class SystemSection
        {
            /// <summary>
            /// Change System Language
            /// </summary>
            public ReactiveObject<Language> Language { get; private set; }

            /// <summary>
            /// Enables or disables Docked Mode
            /// </summary>
            public ReactiveObject<bool> EnableDockedMode { get; private set; }

            /// <summary>
            /// Enables or disables multi-core scheduling of threads
            /// </summary>
            public ReactiveObject<bool> EnableMulticoreScheduling { get; private set; }

            /// <summary>
            /// Enables integrity checks on Game content files
            /// </summary>
            public ReactiveObject<bool> EnableFsIntegrityChecks { get; private set; }

            /// <summary>
            /// Enables FS access log output to the console. Possible modes are 0-3
            /// </summary>
            public ReactiveObject<int> FsGlobalAccessLogMode { get; private set; }

            /// <summary>
            /// Enable or disable ignoring missing services
            /// </summary>
            public ReactiveObject<bool> IgnoreMissingServices { get; private set; }

            public SystemSection()
            {
                Language                  = new ReactiveObject<Language>();
                EnableDockedMode          = new ReactiveObject<bool>();
                EnableMulticoreScheduling = new ReactiveObject<bool>();
                EnableFsIntegrityChecks   = new ReactiveObject<bool>();
                FsGlobalAccessLogMode     = new ReactiveObject<int>();
                IgnoreMissingServices     = new ReactiveObject<bool>();
            }
        }

        /// <summary>
        /// Hid configuration section
        /// </summary>
        public class HidSection
        {
            /// <summary>
            ///  The primary controller's type
            /// </summary>
            public ReactiveObject<ControllerType> ControllerType { get; private set; }

            /// <summary>
            /// Enable or disable keyboard support (Independent from controllers binding)
            /// </summary>
            public ReactiveObject<bool> EnableKeyboard { get; private set; }

            /// <summary>
            /// Keyboard control bindings
            /// </summary>
            public ReactiveObject<NpadKeyboard> KeyboardControls { get; private set; }

            /// <summary>
            /// Controller control bindings
            /// </summary>
            public ReactiveObject<NpadController> JoystickControls { get; private set; }

            public HidSection()
            {
                ControllerType   = new ReactiveObject<ControllerType>();
                EnableKeyboard   = new ReactiveObject<bool>();
                KeyboardControls = new ReactiveObject<NpadKeyboard>();
                JoystickControls = new ReactiveObject<NpadController>();
            }
        }

        /// <summary>
        /// Graphics configuration section
        /// </summary>
        public class GraphicsSection
        {
            /// <summary>
            /// Dumps shaders in this local directory
            /// </summary>
            public ReactiveObject<string> ShadersDumpPath { get; private set; }

            /// <summary>
            /// Enables or disables Vertical Sync
            /// </summary>
            public ReactiveObject<bool> EnableVsync { get; private set; }

            public GraphicsSection()
            {
                ShadersDumpPath = new ReactiveObject<string>();
                EnableVsync     = new ReactiveObject<bool>();
            }
        }

        /// <summary>
        /// The default configuration instance
        /// </summary>
        public static ConfigurationState Instance { get; private set; }

        /// <summary>
        /// The Ui section
        /// </summary>
        public UiSection Ui { get; private set; }

        /// <summary>
        /// The Logger section
        /// </summary>
        public LoggerSection Logger { get; private set; }

        /// <summary>
        /// The System section
        /// </summary>
        public SystemSection System { get; private set; }

        /// <summary>
        /// The Graphics section
        /// </summary>
        public GraphicsSection Graphics { get; private set; }

        /// <summary>
        /// The Hid section
        /// </summary>
        public HidSection Hid { get; private set; }

        /// <summary>
        /// Enables or disables Discord Rich Presence
        /// </summary>
        public ReactiveObject<bool> EnableDiscordIntegration { get; private set; }

        private ConfigurationState()
        {
            Ui                       = new UiSection();
            Logger                   = new LoggerSection();
            System                   = new SystemSection();
            Graphics                 = new GraphicsSection();
            Hid                      = new HidSection();
            EnableDiscordIntegration = new ReactiveObject<bool>();
        }

        public ConfigurationFileFormat ToFileFormat()
        {
            ConfigurationFileFormat configurationFile = new ConfigurationFileFormat
            {
                Version                   = 1,
                GraphicsShadersDumpPath   = Graphics.ShadersDumpPath,
                LoggingEnableDebug        = Logger.EnableDebug,
                LoggingEnableStub         = Logger.EnableStub,
                LoggingEnableInfo         = Logger.EnableInfo,
                LoggingEnableWarn         = Logger.EnableWarn,
                LoggingEnableError        = Logger.EnableError,
                LoggingEnableGuest        = Logger.EnableGuest,
                LoggingEnableFsAccessLog  = Logger.EnableFsAccessLog,
                LoggingFilteredClasses    = Logger.FilteredClasses,
                EnableFileLog             = Logger.EnableFileLog,
                SystemLanguage            = System.Language,
                DockedMode                = System.EnableDockedMode,
                EnableDiscordIntegration  = EnableDiscordIntegration,
                EnableVsync               = Graphics.EnableVsync,
                EnableMulticoreScheduling = System.EnableMulticoreScheduling,
                EnableFsIntegrityChecks   = System.EnableFsIntegrityChecks,
                FsGlobalAccessLogMode     = System.FsGlobalAccessLogMode,
                IgnoreMissingServices     = System.IgnoreMissingServices,
                ControllerType            = Hid.ControllerType,
                GameDirs                  = Ui.GameDirs,
                EnableCustomTheme         = Ui.EnableCustomTheme,
                CustomThemePath           = Ui.CustomThemePath,
                EnableKeyboard            = Hid.EnableKeyboard,
                KeyboardControls          = Hid.KeyboardControls,
                JoystickControls          = Hid.JoystickControls
            };

            return configurationFile;
        }

        public void LoadDefault()
        {
            Graphics.ShadersDumpPath.Value         = "";
            Logger.EnableDebug.Value               = false;
            Logger.EnableStub.Value                = true;
            Logger.EnableInfo.Value                = true;
            Logger.EnableWarn.Value                = true;
            Logger.EnableError.Value               = true;
            Logger.EnableGuest.Value               = true;
            Logger.EnableFsAccessLog.Value         = false;
            Logger.FilteredClasses.Value           = new LogClass[] { };
            Logger.EnableFileLog.Value             = true;
            System.Language.Value                  = Language.AmericanEnglish;
            System.EnableDockedMode.Value          = false;
            EnableDiscordIntegration.Value         = true;
            Graphics.EnableVsync.Value             = true;
            System.EnableMulticoreScheduling.Value = true;
            System.EnableFsIntegrityChecks.Value   = true;
            System.FsGlobalAccessLogMode.Value     = 0;
            System.IgnoreMissingServices.Value     = false;
            Hid.ControllerType.Value               = ControllerType.Handheld;
            Ui.GameDirs.Value                      = new List<string>();
            Ui.EnableCustomTheme.Value             = false;
            Ui.CustomThemePath.Value               = "";
            Hid.EnableKeyboard.Value               = false;

            Hid.KeyboardControls.Value = new NpadKeyboard
            {
                LeftJoycon  = new NpadKeyboardLeft
                {
                    StickUp     = Key.W,
                    StickDown   = Key.S,
                    StickLeft   = Key.A,
                    StickRight  = Key.D,
                    StickButton = Key.F,
                    DPadUp      = Key.Up,
                    DPadDown    = Key.Down,
                    DPadLeft    = Key.Left,
                    DPadRight   = Key.Right,
                    ButtonMinus = Key.Minus,
                    ButtonL     = Key.E,
                    ButtonZl    = Key.Q,
                },
                RightJoycon = new NpadKeyboardRight
                {
                    StickUp     = Key.I,
                    StickDown   = Key.K,
                    StickLeft   = Key.J,
                    StickRight  = Key.L,
                    StickButton = Key.H,
                    ButtonA     = Key.Z,
                    ButtonB     = Key.X,
                    ButtonX     = Key.C,
                    ButtonY     = Key.V,
                    ButtonPlus  = Key.Plus,
                    ButtonR     = Key.U,
                    ButtonZr    = Key.O,
                },
                Hotkeys     = new KeyboardHotkeys
                {
                    ToggleVsync = Key.Tab
                }
            };

            Hid.JoystickControls.Value = new NpadController
            {
                Enabled          = true,
                Index            = 0,
                Deadzone         = 0.05f,
                TriggerThreshold = 0.5f,
                LeftJoycon       = new NpadControllerLeft
                {
                    Stick       = ControllerInputId.Axis0,
                    StickButton = ControllerInputId.Button8,
                    DPadUp      = ControllerInputId.Hat0Up,
                    DPadDown    = ControllerInputId.Hat0Down,
                    DPadLeft    = ControllerInputId.Hat0Left,
                    DPadRight   = ControllerInputId.Hat0Right,
                    ButtonMinus = ControllerInputId.Button6,
                    ButtonL     = ControllerInputId.Button4,
                    ButtonZl    = ControllerInputId.Axis2,
                },
                RightJoycon      = new NpadControllerRight
                {
                    Stick       = ControllerInputId.Axis3,
                    StickButton = ControllerInputId.Button9,
                    ButtonA     = ControllerInputId.Button1,
                    ButtonB     = ControllerInputId.Button0,
                    ButtonX     = ControllerInputId.Button3,
                    ButtonY     = ControllerInputId.Button2,
                    ButtonPlus  = ControllerInputId.Button7,
                    ButtonR     = ControllerInputId.Button5,
                    ButtonZr    = ControllerInputId.Axis5,
                }
            };
        }

        public void Load(ConfigurationFileFormat configurationFileFormat)
        {
            if (configurationFileFormat.Version != 1 && configurationFileFormat.Version != 0)
            {
                Common.Logging.Logger.PrintWarning(LogClass.Application, $"Unsupported configuration version {configurationFileFormat.Version}, loading default.");

                LoadDefault();

                return;
            }

            Graphics.ShadersDumpPath.Value         = configurationFileFormat.GraphicsShadersDumpPath;
            Logger.EnableDebug.Value               = configurationFileFormat.LoggingEnableDebug;
            Logger.EnableStub.Value                = configurationFileFormat.LoggingEnableStub;
            Logger.EnableInfo.Value                = configurationFileFormat.LoggingEnableInfo;
            Logger.EnableWarn.Value                = configurationFileFormat.LoggingEnableWarn;
            Logger.EnableError.Value               = configurationFileFormat.LoggingEnableError;
            Logger.EnableGuest.Value               = configurationFileFormat.LoggingEnableGuest;
            Logger.EnableFsAccessLog.Value         = configurationFileFormat.LoggingEnableFsAccessLog;
            Logger.FilteredClasses.Value           = configurationFileFormat.LoggingFilteredClasses;
            Logger.EnableFileLog.Value             = configurationFileFormat.EnableFileLog;
            System.Language.Value                  = configurationFileFormat.SystemLanguage;
            System.EnableDockedMode.Value          = configurationFileFormat.DockedMode;
            System.EnableDockedMode.Value          = configurationFileFormat.DockedMode;
            EnableDiscordIntegration.Value         = configurationFileFormat.EnableDiscordIntegration;
            Graphics.EnableVsync.Value             = configurationFileFormat.EnableVsync;
            System.EnableMulticoreScheduling.Value = configurationFileFormat.EnableMulticoreScheduling;
            System.EnableFsIntegrityChecks.Value   = configurationFileFormat.EnableFsIntegrityChecks;
            System.FsGlobalAccessLogMode.Value     = configurationFileFormat.FsGlobalAccessLogMode;
            System.IgnoreMissingServices.Value     = configurationFileFormat.IgnoreMissingServices;
            Hid.ControllerType.Value               = configurationFileFormat.ControllerType;
            Ui.GameDirs.Value                      = configurationFileFormat.GameDirs;
            Ui.EnableCustomTheme.Value             = configurationFileFormat.EnableCustomTheme;
            Ui.CustomThemePath.Value               = configurationFileFormat.CustomThemePath;
            Hid.EnableKeyboard.Value               = configurationFileFormat.EnableKeyboard;
            Hid.KeyboardControls.Value             = configurationFileFormat.KeyboardControls;
            Hid.JoystickControls.Value             = configurationFileFormat.JoystickControls;
        }

        public static void Initialize()
        {
            if (Instance != null)
            {
                throw new InvalidOperationException("Configuration is already initialized");
            }

            Instance = new ConfigurationState();
        }
    }
}
