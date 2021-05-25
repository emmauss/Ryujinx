using System;

namespace Ryujinx.Input.HLE
{
    public class InputManager : IDisposable
    {
        public IGamepadDriver KeyboardDriver { get; private set; }
        public IGamepadDriver GamepadDriver { get; private set; }
        public IMouseDriver MouseDriver { get; private set; }

        public InputManager(IGamepadDriver keyboardDriver, IGamepadDriver gamepadDriver, IMouseDriver mouseDriver)
        {
            KeyboardDriver = keyboardDriver;
            GamepadDriver = gamepadDriver;
            MouseDriver = mouseDriver;
        }

        public NpadManager CreateNpadManager()
        {
            return new NpadManager(KeyboardDriver, GamepadDriver);
        }
        
        public TouchScreenManager CreateTouchScreenManager()
        {
            return new TouchScreenManager(MouseDriver);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                KeyboardDriver?.Dispose();
                GamepadDriver?.Dispose();
                MouseDriver?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
