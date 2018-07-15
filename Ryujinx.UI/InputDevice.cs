using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Input;

namespace Ryujinx.UI
{
    public struct InputDevice
    {
        public int          Index;
        public string       Name;
        public IInputDevice Device;
        public DeviceType   DeviceType;
    }

    public enum DeviceType
    {
        GamePad,
        Keyboard
    }
}
