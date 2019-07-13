using System.Runtime.InteropServices;

namespace Ryujinx.HLE.Input
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ControllerHeader
    {
        public ControllerStatus    Status;
        public int                 IsJoyConHalf;
        public ControllerColorDesc SingleColorDesc;
        public NpadColor           SingleBodyColor;
        public NpadColor           SingleButtonColor;
        public ControllerColorDesc SplitColorDesc;
        public NpadColor           RightBodyColor;
        public NpadColor           RightButtonColor;
        public NpadColor           LeftBodyColor;
        public NpadColor           LeftButtonColor;
    }
}
