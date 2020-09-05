using System;
using System.Collections.Generic;
using System.Text;

namespace Ryujinx.Ui
{
    public struct JoystickCapabilities
    {
        public int AxisCount { get; }
        public int ButtonCount { get; }
        public int HatCount { get; }

        public string Name { get; }

        public JoystickCapabilities(int axisCount, int buttonCount, int hatCount, string name) : this()
        {
            AxisCount = axisCount;
            ButtonCount = buttonCount;
            HatCount = hatCount;
            Name = name;
        }
    }
}
