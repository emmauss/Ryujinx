using OpenTK.Windowing.GraphicsLibraryFramework;
using SkiaSharp;
using System;

namespace Ryujinx.Skia.Ui.Skia.Widget
{
    public interface IInput
    {
        bool IsInputGrabbed { get; set; }

        event EventHandler<InputEventArgs> Input;

        void OnGrabInput();
        void OnLeaveInput();
        void HandleMouse(SKPoint position, InputMode inputMode);

        void HandleKeyboard(Keys key, KeyModifiers modifiers, InputMode inputMode);

        void HandleText(string text);

        public class InputEventArgs : EventArgs
        {
            public Keys Key { get; set; }
            public KeyModifiers Modifiers { get; set; }
            public InputMode InputMode { get; set; }
        }
    }
}
