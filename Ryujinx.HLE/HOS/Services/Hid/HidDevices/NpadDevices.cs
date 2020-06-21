using System;
using System.Runtime.InteropServices;
using Ryujinx.Common.Logging;
using Ryujinx.HLE.HOS.Kernel.Threading;

namespace Ryujinx.HLE.HOS.Services.Hid
{
    public class NpadDevices : BaseDevice
    {
        internal NpadJoyHoldType JoyHold = NpadJoyHoldType.Vertical;
        internal bool SixAxisActive = false; // TODO: link to hidserver when implemented

        private enum FilterState
        {
            Unconfigured = 0,
            Configured   = 1,
            Accepted     = 2
        }

        private struct NpadConfig
        {
            public ControllerType ConfiguredType;
            public FilterState State;
        }

        private const int _maxControllers = 9; // Players1-8 and Handheld
        private NpadConfig[] _configuredNpads;

        private ControllerType _supportedStyleSets = ControllerType.ProController |
                                                     ControllerType.JoyconPair |
                                                     ControllerType.JoyconLeft |
                                                     ControllerType.JoyconRight |
                                                     ControllerType.Handheld;

        public ControllerType SupportedStyleSets
        {
            get => _supportedStyleSets;
            set
            {
                if (_supportedStyleSets != value) // Deal with spamming
                {
                    _supportedStyleSets = value;
                    MatchControllers();
                }
            }
        }

        public PlayerIndex PrimaryController { get; set; } = PlayerIndex.Unknown;

        private KEvent[] _styleSetUpdateEvents;

        private static readonly Array3<BatteryCharge> _fullBattery;

        public NpadDevices(Switch device, bool active = true) : base(device, active)
        {
            _configuredNpads = new NpadConfig[_maxControllers];

            _styleSetUpdateEvents = new KEvent[_maxControllers];

            for (int i = 0; i < _styleSetUpdateEvents.Length; ++i)
            {
                _styleSetUpdateEvents[i] = new KEvent(_device.System.KernelContext);
            }

            _fullBattery[0] = _fullBattery[1] = _fullBattery[2] = BatteryCharge.Percent100;
        }

        public void AddControllers(params ControllerConfig[] configs)
        {
            for (int i = 0; i < configs.Length; ++i)
            {
                PlayerIndex    player         = configs[i].Player;
                ControllerType controllerType = configs[i].Type;

                if (player > PlayerIndex.Handheld)
                {
                    throw new ArgumentOutOfRangeException("Player must be Player1-8 or Handheld");
                }

                if (controllerType == ControllerType.Handheld)
                {
                    player = PlayerIndex.Handheld;
                }

                _configuredNpads[(int)player] = new NpadConfig { ConfiguredType = controllerType, State = FilterState.Configured };
            }

            MatchControllers();
        }

        private void MatchControllers()
        {
            PrimaryController = PlayerIndex.Unknown;

            for (int i = 0; i < _configuredNpads.Length; ++i)
            {
                ref NpadConfig config = ref _configuredNpads[i];

                if (config.State == FilterState.Unconfigured)
                {
                    continue; // Ignore unconfigured
                }

                if ((config.ConfiguredType & _supportedStyleSets) == 0)
                {
                    Logger.PrintWarning(LogClass.Hid, $"ControllerType {config.ConfiguredType} (connected to {(PlayerIndex)i}) not supported by game. Removing...");

                    config.State = FilterState.Configured;
                    _device.Hid.SharedMemory.Npads[i] = new ShMemNpad(); // Zero it

                    continue;
                }

                InitController((PlayerIndex)i, config.ConfiguredType);
            }

            // Couldn't find any matching configuration. Reassign to something that works.
            if (PrimaryController == PlayerIndex.Unknown)
            {
                ControllerType[] npadsTypeList = (ControllerType[])Enum.GetValues(typeof(ControllerType));

                // Skip None Type
                for (int i = 1; i < npadsTypeList.Length; ++i)
                {
                    ControllerType controllerType = npadsTypeList[i];
                    if ((controllerType & _supportedStyleSets) != 0)
                    {
                        Logger.PrintWarning(LogClass.Hid, $"No matching controllers found. Reassigning input as ControllerType {controllerType}...");

                        InitController(controllerType == ControllerType.Handheld ? PlayerIndex.Handheld : PlayerIndex.Player1, controllerType);

                        return;
                    }
                }

                Logger.PrintError(LogClass.Hid, "Couldn't find any appropriate controller.");
            }
        }

        internal ref KEvent GetStyleSetUpdateEvent(PlayerIndex player)
        {
            return ref _styleSetUpdateEvents[(int)player];
        }

        private void InitController(PlayerIndex player, ControllerType type)
        {
            if (type == ControllerType.Handheld)
            {
                player = PlayerIndex.Handheld;
            }

            ref ShMemNpad controller = ref _device.Hid.SharedMemory.Npads[(int)player];

            controller = new ShMemNpad(); // Zero it

            // TODO: Allow customizing colors at config
            NpadStateHeader defaultHeader = new NpadStateHeader
            {
                IsHalf             = false,
                SingleColorBody    = NpadColor.BodyGray,
                SingleColorButtons = NpadColor.ButtonGray,
                LeftColorBody      = NpadColor.BodyNeonBlue,
                LeftColorButtons   = NpadColor.ButtonGray,
                RightColorBody     = NpadColor.BodyNeonRed,
                RightColorButtons  = NpadColor.ButtonGray
            };

            controller.SystemProperties = NpadSystemProperties.PowerInfo0Connected |
                                          NpadSystemProperties.PowerInfo1Connected |
                                          NpadSystemProperties.PowerInfo2Connected;

            controller.BatteryState = _fullBattery;

            switch (type)
            {
                case ControllerType.ProController:
                    defaultHeader.Type           = ControllerType.ProController;
                    controller.DeviceType        = DeviceType.FullKey;
                    controller.SystemProperties |= NpadSystemProperties.AbxyButtonOriented |
                                                   NpadSystemProperties.PlusButtonCapability |
                                                   NpadSystemProperties.MinusButtonCapability;
                    break;
                case ControllerType.Handheld:
                    defaultHeader.Type           = ControllerType.Handheld;
                    controller.DeviceType        = DeviceType.HandheldLeft |
                                                   DeviceType.HandheldRight;
                    controller.SystemProperties |= NpadSystemProperties.AbxyButtonOriented |
                                                   NpadSystemProperties.PlusButtonCapability |
                                                   NpadSystemProperties.MinusButtonCapability;
                    break;
                case ControllerType.JoyconPair:
                    defaultHeader.Type           = ControllerType.JoyconPair;
                    controller.DeviceType        = DeviceType.JoyLeft |
                                                   DeviceType.JoyRight;
                    controller.SystemProperties |= NpadSystemProperties.AbxyButtonOriented |
                                                   NpadSystemProperties.PlusButtonCapability |
                                                   NpadSystemProperties.MinusButtonCapability;
                    break;
                case ControllerType.JoyconLeft:
                    defaultHeader.Type           = ControllerType.JoyconLeft;
                    defaultHeader.IsHalf         = true;
                    controller.DeviceType        = DeviceType.JoyLeft;
                    controller.SystemProperties |= NpadSystemProperties.SlSrButtonOriented |
                                                   NpadSystemProperties.MinusButtonCapability;
                    break;
                case ControllerType.JoyconRight:
                    defaultHeader.Type           = ControllerType.JoyconRight;
                    defaultHeader.IsHalf         = true;
                    controller.DeviceType        = DeviceType.JoyRight;
                    controller.SystemProperties |= NpadSystemProperties.SlSrButtonOriented |
                                                   NpadSystemProperties.PlusButtonCapability;
                    break;
                case ControllerType.Pokeball:
                    defaultHeader.Type    = ControllerType.Pokeball;
                    controller.DeviceType = DeviceType.Palma;
                    break;
            }

            controller.Header = defaultHeader;

            if (PrimaryController == PlayerIndex.Unknown)
            {
                PrimaryController = player;
            }

            _configuredNpads[(int)player].State = FilterState.Accepted;

            _styleSetUpdateEvents[(int)player].ReadableEvent.Signal();

            Logger.PrintInfo(LogClass.Hid, $"Connected ControllerType {type} to PlayerIndex {player}");
        }

        private static NpadLayoutsIndex ControllerTypeToNpadLayout(ControllerType controllerType)
        => controllerType switch
        {
            ControllerType.ProController => NpadLayoutsIndex.ProController,
            ControllerType.Handheld      => NpadLayoutsIndex.Handheld,
            ControllerType.JoyconPair    => NpadLayoutsIndex.JoyDual,
            ControllerType.JoyconLeft    => NpadLayoutsIndex.JoyLeft,
            ControllerType.JoyconRight   => NpadLayoutsIndex.JoyRight,
            ControllerType.Pokeball      => NpadLayoutsIndex.Pokeball,
            _                            => NpadLayoutsIndex.SystemExternal
        };

        private static SixAxixLayoutsIndex ControllerTypeToSixAxisLayout(ControllerType controllerType)
        => controllerType switch
        {
            ControllerType.ProController => SixAxixLayoutsIndex.ProController,
            ControllerType.Handheld      => SixAxixLayoutsIndex.Handheld,
            ControllerType.JoyconPair    => SixAxixLayoutsIndex.JoyDualLeft,
            ControllerType.JoyconLeft    => SixAxixLayoutsIndex.JoyLeft,
            ControllerType.JoyconRight   => SixAxixLayoutsIndex.JoyRight,
            ControllerType.Pokeball      => SixAxixLayoutsIndex.Pokeball,
            _                            => SixAxixLayoutsIndex.SystemExternal
        };

        public void SetGamepadsInput(params GamepadInput[] states)
        {
            UpdateAllEntries();

            for (int i = 0; i < states.Length; ++i)
            {
                SetGamepadState(states[i].PlayerId, states[i].Buttons, states[i].LStick, states[i].RStick);
            }
        }

        public void SetSixAxisInput(params SixAxisInput[] states)
        {
            for (int i = 0; i < states.Length; ++i)
            {
                HidVector accel = new HidVector()
                {
                    X = states[i].Accelerometer.X,
                    Y = states[i].Accelerometer.Y,
                    Z = states[i].Accelerometer.Z
                };

                HidVector gyro = new HidVector()
                {
                    X = states[i].Gyroscope.X,
                    Y = states[i].Gyroscope.Y,
                    Z = states[i].Gyroscope.Z
                };

                HidVector rotation = new HidVector()
                {
                    X = states[i].Rotation.X,
                    Y = states[i].Rotation.Y,
                    Z = states[i].Rotation.Z
                };

                if (SetSixAxisState(states[i].PlayerId, accel, gyro, rotation, states[i].Orientation))
                {
                    SetSixAxisState(states[i + 1].PlayerId, accel, gyro, rotation, states[i + 1].Orientation, true);
                    i++;
                }
            }
        }

        private void SetGamepadState(PlayerIndex player, ControllerKeys buttons,
            JoystickPosition leftJoystick, JoystickPosition rightJoystick)
        {
            if (player == PlayerIndex.Auto)
            {
                player = PrimaryController;
            }

            if (player == PlayerIndex.Unknown)
            {
                return;
            }

            if (_configuredNpads[(int)player].State != FilterState.Accepted)
            {
                return;
            }

            ref ShMemNpad  currentNpad   = ref _device.Hid.SharedMemory.Npads[(int)player];
            ref NpadLayout currentLayout = ref currentNpad.Layouts[(int)ControllerTypeToNpadLayout(currentNpad.Header.Type)];
            ref NpadState  currentEntry  = ref currentLayout.Entries[(int)currentLayout.Header.LatestEntry];

            currentEntry.Buttons = buttons;
            currentEntry.LStickX = leftJoystick.Dx;
            currentEntry.LStickY = leftJoystick.Dy;
            currentEntry.RStickX = rightJoystick.Dx;
            currentEntry.RStickY = rightJoystick.Dy;

            // Mirror data to Default layout just in case
            ref NpadLayout mainLayout = ref currentNpad.Layouts[(int)NpadLayoutsIndex.SystemExternal];
            mainLayout.Entries[(int)mainLayout.Header.LatestEntry] = currentEntry;
        }

        private unsafe bool SetSixAxisState(PlayerIndex player, HidVector accel,
            HidVector gyro, HidVector rotation, float[] orientation, bool isRightPair = false)
        {
            if (player == PlayerIndex.Auto)
            {
                player = PrimaryController;
            }

            if (player == PlayerIndex.Unknown)
            {
                return false;
            }

            if (_configuredNpads[(int)player].State != FilterState.Accepted)
            {
                return false;
            }

            ref ShMemNpad currentNpad = ref _device.Hid.SharedMemory.Npads[(int)player];
            ref NpadSixAxis currentLayout = ref currentNpad.Sixaxis[(int)ControllerTypeToSixAxisLayout(currentNpad.Header.Type) + (isRightPair ? 1 : 0)];
            ref SixAxisState currentEntry = ref currentLayout.Entries[(int)currentLayout.Header.LatestEntry];

            int previousEntryIndex = (int)(currentLayout.Header.LatestEntry == 0 ?
                                            currentLayout.Header.MaxEntryIndex : currentLayout.Header.LatestEntry - 1);

            ref SixAxisState previousEntry = ref currentLayout.Entries[previousEntryIndex];

            currentEntry.Accelerometer = accel;
            currentEntry.Gyroscope     = gyro;
            currentEntry.Rotations     = rotation;

            for (int i = 0; i < 9; i++)
            {
                currentEntry.Orientation[i] = orientation[i];
            }

            return currentNpad.Header.Type == ControllerType.JoyconPair && !isRightPair;
        }

        private void UpdateAllEntries()
        {
            ref Array10<ShMemNpad> controllers = ref _device.Hid.SharedMemory.Npads;
            for (int i = 0; i < controllers.Length; ++i)
            {
                ref Array7<NpadLayout> layouts = ref controllers[i].Layouts;
                for (int l = 0; l < layouts.Length; ++l)
                {
                    ref NpadLayout currentLayout = ref layouts[l];
                    int currentIndex = UpdateEntriesHeader(ref currentLayout.Header, out int previousIndex);

                    ref NpadState currentEntry = ref currentLayout.Entries[currentIndex];
                    NpadState previousEntry    = currentLayout.Entries[previousIndex];

                    currentEntry.SampleTimestamp  = previousEntry.SampleTimestamp + 1;
                    currentEntry.SampleTimestamp2 = previousEntry.SampleTimestamp2 + 1;

                    if (controllers[i].Header.Type == ControllerType.None)
                    {
                        continue;
                    }

                    currentEntry.ConnectionState = NpadConnectionState.ControllerStateConnected;

                    switch (controllers[i].Header.Type)
                    {
                        case ControllerType.Handheld:
                        case ControllerType.ProController:
                            currentEntry.ConnectionState |= NpadConnectionState.ControllerStateWired;
                            break;
                        case ControllerType.JoyconPair:
                            currentEntry.ConnectionState |= NpadConnectionState.JoyLeftConnected |
                                                            NpadConnectionState.JoyRightConnected;
                            break;
                        case ControllerType.JoyconLeft:
                            currentEntry.ConnectionState |= NpadConnectionState.JoyLeftConnected;
                            break;
                        case ControllerType.JoyconRight:
                            currentEntry.ConnectionState |= NpadConnectionState.JoyRightConnected;
                            break;
                    }
                }

                ref Array6<NpadSixAxis> sixaxis = ref controllers[i].Sixaxis;
                for (int l = 0; l < sixaxis.Length; ++l)
                {
                    ref NpadSixAxis currentLayout = ref sixaxis[l];
                    int currentIndex = UpdateEntriesHeader(ref currentLayout.Header, out int previousIndex);

                    ref SixAxisState currentEntry = ref currentLayout.Entries[currentIndex];
                    SixAxisState previousEntry = currentLayout.Entries[previousIndex];

                    currentEntry.SampleTimestamp = previousEntry.SampleTimestamp + 1;
                    currentEntry.SampleTimestamp2 = previousEntry.SampleTimestamp2 + 1;

                    currentEntry._unknown2 = 1;
                }
            }
        }
    }
}