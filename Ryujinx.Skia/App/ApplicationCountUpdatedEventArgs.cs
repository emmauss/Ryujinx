using System;

namespace Ryujinx.Skia.App
{
    public class ApplicationCountUpdatedEventArgs : EventArgs
    {
        public int NumAppsFound  { get; set; }
        public int NumAppsLoaded { get; set; }
    }
}