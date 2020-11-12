using System;

namespace Ryujinx.Skia.App
{
    public class ApplicationAddedEventArgs : EventArgs
    {
        public ApplicationData AppData { get; set; }
    }
}