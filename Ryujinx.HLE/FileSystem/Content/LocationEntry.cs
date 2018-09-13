using System;
using System.Collections.Generic;
using System.Text;

namespace Ryujinx.HLE.FileSystem.Content
{
    public class LocationEntry
    {
        public LocationEntry PreviousEntry;
        public LocationEntry NextEntry;
        public string        ContentPath;
        public int           Flag;
        public long          TitleId;
    }
}
