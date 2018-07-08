using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Input;

namespace Ryujinx.UI
{
    public struct InputDevice
    {
        public int Index;
        public IInputDevice Device;
        public DeviceType DeviceType;
        public string Name;
    }

    public enum DeviceType
    {
        GamePad,
        Keyboard
    }
}
