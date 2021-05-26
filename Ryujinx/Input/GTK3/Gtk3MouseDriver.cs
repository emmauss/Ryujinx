using Gdk;
using Gtk;
using System;
using System.Numerics;
using Size = System.Drawing.Size;

namespace Ryujinx.Input.GTK3
{
    public class Gtk3MouseDriver : IMouseDriver
    {
        private const int GyroIncrement = 8;
        
        private Widget _parent;
        private Widget _client;
        private bool _isDisposed;

        public bool[] Buttons { get; }
        
        public Vector3 LastPosition { get; private set; }
        public Vector3 CurrentPosition { get; private set; }

        public Gtk3MouseDriver(Widget parent)
        {
            _parent = parent;
            
            _parent.MotionNotifyEvent += Parent_MotionNotifyEvent;
            _parent.ButtonPressEvent += Parent_ButtonPressEvent;
            _parent.ButtonReleaseEvent += Parent_ButtonReleaseEvent;
            _parent.ScrollEvent += Parent_ScrollEvent;

            Buttons  = new bool[(int) MouseButton.Count];
        }

        private void Parent_ScrollEvent(object o, ScrollEventArgs args)
        {
            if (args.Event.Device.InputSource == InputSource.Mouse )
            {
                switch (args.Event.Direction)
                {
                    case ScrollDirection.Up:
                        CurrentPosition = new Vector3(CurrentPosition.X, CurrentPosition.Y, CurrentPosition.Z - GyroIncrement);
                        break;
                    case ScrollDirection.Down:
                        CurrentPosition = new Vector3(CurrentPosition.X, CurrentPosition.Y, CurrentPosition.Z + GyroIncrement);
                        break;
                }
            }
        }

        public void SetClientWidget(Widget client)
        {
            _client = client;
        }

        [GLib.ConnectBefore]
        private void Parent_ButtonReleaseEvent(object o, ButtonReleaseEventArgs args)
        {
            Buttons[args.Event.Button - 1] = false;
        }

        [GLib.ConnectBefore]
        private void Parent_ButtonPressEvent(object o, ButtonPressEventArgs args)
        {
            Buttons[args.Event.Button - 1] = true;
        }

        [GLib.ConnectBefore]
        private void Parent_MotionNotifyEvent(object o, MotionNotifyEventArgs args)
        {
            if (args.Event.Device.InputSource == InputSource.Mouse)
            {
                LastPosition =  LastPosition.Length() == 0 ? CurrentPosition : LastPosition;

                CurrentPosition = new Vector3((float)args.Event.X, (float)args.Event.Y, CurrentPosition.Z);
            }
        }
        
        public Vector3 GetPointerVelocity()
        {
            var difference = Vector3.Subtract(LastPosition, CurrentPosition);
            LastPosition = Vector3.Lerp(LastPosition,CurrentPosition, 0.1f);
            return Vector3.Multiply(difference, GyroIncrement);
        }

        public bool IsButtonPressed(MouseButton button)
        {
            return Buttons[(int) button];
        }

        public Size GetClientSize()
        {
            return new Size(_client.AllocatedWidth, _client.AllocatedHeight);
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            
            _parent.MotionNotifyEvent -= Parent_MotionNotifyEvent;
            _parent.ButtonPressEvent -= Parent_ButtonPressEvent;
            _parent.ButtonReleaseEvent -= Parent_ButtonReleaseEvent;

            _client = null;
        }

        public string DriverName => "GTK3";
        
        public event Action<string> OnGamepadConnected
        {
            add    { }
            remove { }
        }

        public event Action<string> OnGamepadDisconnected
        {
            add    { }
            remove { }
        }

        public ReadOnlySpan<string> GamepadsIds => new[] {"0"};
        
        public IGamepad GetGamepad(string id)
        {
            throw new NotImplementedException();
        }
    }
}