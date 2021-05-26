using System;
using System.Drawing;
using System.Numerics;

namespace Ryujinx.Input
{
    public interface IMouseDriver: IGamepadDriver, IDisposable
    {
        public bool[] Buttons { get; }
        public Vector3 LastPosition { get; }
        public Vector3 CurrentPosition { get; }

        public Vector3 GetPointerVelocity();

        public bool IsButtonPressed(MouseButton button);

        public Size GetClientSize();
    }
}