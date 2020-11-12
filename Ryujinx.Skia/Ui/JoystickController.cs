using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Ryujinx.Common.Configuration.Hid;
using Ryujinx.HLE.HOS.Services.Hid;
using System;
using System.Numerics;
using ControllerConfig = Ryujinx.Common.Configuration.Hid.ControllerConfig;

namespace Ryujinx.Skia.Ui
{
    public class JoystickController
    {
        private readonly JoystickState _joystick;
        private readonly ControllerConfig _config;

        public JoystickController(ControllerConfig config, JoystickState joystick)
        {
            _joystick = joystick;
            _config = config;
        }

        private bool IsEnabled()
        {
            return true;//_joystick.IsConnected;
        }

        public ControllerKeys GetButtons()
        {
            ControllerKeys buttons = 0;

            if (IsActivated(_config.LeftJoycon.DPadUp))       buttons |= ControllerKeys.DpadUp;
            if (IsActivated(_config.LeftJoycon.DPadDown))     buttons |= ControllerKeys.DpadDown;
            if (IsActivated(_config.LeftJoycon.DPadLeft))     buttons |= ControllerKeys.DpadLeft;
            if (IsActivated(_config.LeftJoycon.DPadRight))    buttons |= ControllerKeys.DpadRight;
            if (IsActivated(_config.LeftJoycon.StickButton))  buttons |= ControllerKeys.LStick;
            if (IsActivated(_config.LeftJoycon.ButtonMinus))  buttons |= ControllerKeys.Minus;
            if (IsActivated(_config.LeftJoycon.ButtonL))      buttons |= ControllerKeys.L;
            if (IsActivated(_config.LeftJoycon.ButtonZl))     buttons |= ControllerKeys.Zl;
            if (IsActivated(_config.LeftJoycon.ButtonSl))     buttons |= ControllerKeys.SlLeft;
            if (IsActivated(_config.LeftJoycon.ButtonSr))     buttons |= ControllerKeys.SrLeft;

            if (IsActivated(_config.RightJoycon.ButtonA))     buttons |= ControllerKeys.A;
            if (IsActivated(_config.RightJoycon.ButtonB))     buttons |= ControllerKeys.B;
            if (IsActivated(_config.RightJoycon.ButtonX))     buttons |= ControllerKeys.X;
            if (IsActivated(_config.RightJoycon.ButtonY))     buttons |= ControllerKeys.Y;
            if (IsActivated(_config.RightJoycon.StickButton)) buttons |= ControllerKeys.RStick;
            if (IsActivated(_config.RightJoycon.ButtonPlus))  buttons |= ControllerKeys.Plus;
            if (IsActivated(_config.RightJoycon.ButtonR))     buttons |= ControllerKeys.R;
            if (IsActivated(_config.RightJoycon.ButtonZr))    buttons |= ControllerKeys.Zr;
            if (IsActivated(_config.RightJoycon.ButtonSl))    buttons |= ControllerKeys.SlRight;
            if (IsActivated(_config.RightJoycon.ButtonSr))    buttons |= ControllerKeys.SrRight;

            return buttons;
        }

        private bool IsActivated(ControllerInputId controllerInputId)
        {
            if (controllerInputId <= ControllerInputId.Button20)
            {
                return _joystick.IsButtonDown((int)controllerInputId);
            }
            else if (controllerInputId <= ControllerInputId.Axis5)
            {
                int axis = controllerInputId - ControllerInputId.Axis0;

                return _joystick.GetAxis(axis) > _config.TriggerThreshold;
            }
            else if (controllerInputId <= ControllerInputId.Hat2Right)
            {
                int hat = (controllerInputId - ControllerInputId.Hat0Up) / 4;

                int baseHatId = (int)ControllerInputId.Hat0Up + (hat * 4);

                Hat hatState = _joystick.GetHat(hat);

                if (hatState.HasFlag(Hat.Up) && ((int)controllerInputId % baseHatId == 0)) return true;
                if (hatState.HasFlag(Hat.Down)  && ((int)controllerInputId % baseHatId == 1)) return true;
                if (hatState.HasFlag(Hat.Left)  && ((int)controllerInputId % baseHatId == 2)) return true;
                if (hatState.HasFlag(Hat.Right) && ((int)controllerInputId % baseHatId == 3)) return true;
            }

            return false;
        }

        public (short, short) GetLeftStick()
        {
            if (!IsEnabled())
            {
                return (0, 0);
            }

            return GetStick(_config.LeftJoycon.StickX, _config.LeftJoycon.StickY, _config.DeadzoneLeft);
        }

        public (short, short) GetRightStick()
        {
            if (!IsEnabled())
            {
                return (0, 0);
            }

            return GetStick(_config.RightJoycon.StickX, _config.RightJoycon.StickY, _config.DeadzoneRight);
        }

        private (short, short) GetStick(ControllerInputId stickXInputId, ControllerInputId stickYInputId, float deadzone)
        {
            if (stickXInputId < ControllerInputId.Axis0 || stickXInputId > ControllerInputId.Axis5 || 
                stickYInputId < ControllerInputId.Axis0 || stickYInputId > ControllerInputId.Axis5)
            {
                return (0, 0);
            }

            int xAxis = stickXInputId - ControllerInputId.Axis0;
            int yAxis = stickYInputId - ControllerInputId.Axis0;

            float xValue =  _joystick.GetAxis(xAxis);
            float yValue = -_joystick.GetAxis(yAxis); // Invert Y-axis

            return ApplyDeadzone(new Vector2(xValue, yValue), deadzone);
        }

        private (short, short) ApplyDeadzone(Vector2 axis, float deadzone)
        {
            return (ClampAxis(MathF.Abs(axis.X) > deadzone ? axis.X : 0f),
                    ClampAxis(MathF.Abs(axis.Y) > deadzone ? axis.Y : 0f));
        }

        private static short ClampAxis(float value)
        {
            if (value <= -short.MaxValue)
            {
                return -short.MaxValue;
            }
            else
            {
                return (short)(value * short.MaxValue);
            }
        }
    }
}
