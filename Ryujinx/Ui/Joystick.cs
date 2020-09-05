using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ryujinx.Ui
{
    public static class Joystick
    {
        public const int JoystickCount = 16;
        private static JoystickState[] JoystickStates;

        static Joystick()
        {
            JoystickStates = new JoystickState[JoystickCount];
        }

        public unsafe static void Initialize()
        {
            GLFW.Init();

            GLFW.SetJoystickCallback((joy, eventCode) =>
            {
                if (eventCode == ConnectedState.Connected)
                {
                    GLFW.GetJoystickHatsRaw(joy, out var hatCount);
                    GLFW.GetJoystickAxesRaw(joy, out var axisCount);
                    GLFW.GetJoystickButtonsRaw(joy, out var buttonCount);
                    var name = GLFW.GetJoystickName(joy);

                    JoystickStates[joy] = new JoystickState(hatCount, axisCount, buttonCount, joy, name);
                }
                else
                {
                    JoystickStates[joy] = default;
                }
            });
        }

        public static bool IsConnected(int index)
        {
            return GLFW.JoystickPresent(index);
        }

        public unsafe static JoystickCapabilities GetCapabilities(int index)
        {
            GLFW.PollEvents();

            if (!IsConnected(index))
            {
                return default;
            }

            lock (JoystickStates)
            {
                GLFW.GetJoystickHatsRaw(index, out var hatCount);
                GLFW.GetJoystickAxesRaw(index, out var axisCount);
                GLFW.GetJoystickButtonsRaw(index, out var buttonCount);
                var name = GLFW.GetJoystickName(index);

                return new JoystickCapabilities(axisCount, buttonCount, hatCount, name);
            }
        }

        public unsafe static void UpdateStates()
        {
            GLFW.PollEvents();

            for (int index = 0; index < JoystickCount; index++)
            {
                if (!GLFW.JoystickPresent(index))
                {
                    JoystickStates[index] = default;
                }
                else
                {
                    var h = GLFW.GetJoystickHatsRaw(index, out var count);
                    var hats = new Hat[count];
                    for (var j = 0; j < count; j++)
                    {
                        hats[j] = (Hat)h[j];
                    }

                    var axes = GLFW.GetJoystickAxes(index);

                    float[] normalizedAxis = new float[axes.Length];

                    for(int axis = 0; axis < axes.Length; axis++)
                    {
                        float value = axes[axis];

                        if(axis < 4)
                        {
                            // axis is normal axis. let add a natural deadzone to avoid very tiny values
                            normalizedAxis[axis] = Math.Abs(value) < 0.05 ? 0 : value;
                        }
                        else
                        {
                            // axis is usually a trigger, normalize the value to between 0 and 1
                            normalizedAxis[axis] = (value + 1) / 2;
                        }
                    }

                    var b = GLFW.GetJoystickButtonsRaw(index, out count);
                    var buttons = new bool[count];
                    for (var j = 0; j < buttons.Length; j++)
                    {
                        buttons[j] = b[j] == JoystickInputAction.Press;
                    }

                    var name = GLFW.GetJoystickName(index);

                    JoystickStates[index] = new JoystickState(hats, normalizedAxis, buttons, index, name);
                }
            }
        }

        public static JoystickState GetState(int index)
        {
            return JoystickStates[index];
        }
    }
}
