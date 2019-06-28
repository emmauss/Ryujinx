namespace Ryujinx.HLE.Input
{
    public struct ControllerState
    {
        public long                SamplesTimestamp;
        public long                SamplesTimestamp2;
        public ControllerButtons   ButtonState;
        public JoystickPosition    LeftStick;
        public JoystickPosition    RightStick;
        public ControllerConnState ConnectionState;
    }
}
