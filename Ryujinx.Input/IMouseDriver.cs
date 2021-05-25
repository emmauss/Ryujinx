using System;
using System.Drawing;
using System.Numerics;

namespace Ryujinx.Input
{
    public interface IMouseDriver: IGamepadDriver, IDisposable
    {
        public bool[] Buttons { get; }
        public Vector2 LastPosition { get; }
        public Vector2 CurrentPosition { get; }

        public Vector2 GetVelocity();

        public bool IsButtonPressed(MouseButton button);

        public Size GetClientSize();
    }
}