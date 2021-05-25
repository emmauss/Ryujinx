using Ryujinx.HLE;
using Ryujinx.HLE.HOS.Services.Hid;
using System;

namespace Ryujinx.Input.HLE
{
    public class TouchScreenManager : IDisposable
    {
        private const int SwitchPanelWidth = 1280;
        private const int SwitchPanelHeight = 720;
        
        private readonly IMouseDriver _mouseDriver;
        private Switch _device;

        public TouchScreenManager(IMouseDriver mouseDriver)
        {
            _mouseDriver = mouseDriver;
        }
        
        public void Initialize(Switch device)
        {
            _device = device;
        }

        public bool Update(bool isFocused, float aspectRatio = 0)
        {
            if (!isFocused)
            {
                _device.Hid.Touchscreen.Update();
                
                return false;
            }
            
            var position = _mouseDriver.CurrentPosition;
            var clientSize = _mouseDriver.GetClientSize();

            float mouseX = position.X;
            float mouseY = position.Y;

            float aspectWidth = SwitchPanelHeight * aspectRatio;//ConfigurationState.Instance.Graphics.AspectRatio.Value.ToFloat();

            int screenWidth = clientSize.Width;
            int screenHeight = clientSize.Height;

            if (clientSize.Width > clientSize.Height * aspectWidth / SwitchPanelHeight)
            {
                screenWidth = (int)(clientSize.Height * aspectWidth) / SwitchPanelHeight;
            }
            else
            {
                screenHeight = (clientSize.Width * SwitchPanelHeight) / (int)aspectWidth;
            }

            int startX = (clientSize.Width - screenWidth) >> 1;
            int startY = (clientSize.Height - screenHeight) >> 1;

            int endX = startX + screenWidth;
            int endY = startY + screenHeight;

            if (mouseX >= startX &&
                mouseY >= startY &&
                mouseX < endX &&
                mouseY < endY)
            {
                int screenMouseX = (int)mouseX - startX;
                int screenMouseY = (int)mouseY - startY;

                int mX = (screenMouseX * (int)aspectWidth) / screenWidth;
                int mY = (screenMouseY * SwitchPanelHeight) / screenHeight;

                TouchPoint currentPoint = new TouchPoint
                {
                    X = (uint)mX,
                    Y = (uint)mY,

                    // Placeholder values till more data is acquired
                    DiameterX = 10,
                    DiameterY = 10,
                    Angle = 90
                };

                _device.Hid.Touchscreen.Update(currentPoint);

                return true;
            }

            return false;
        }
        
        public void Dispose()
        {
        }
    }
}