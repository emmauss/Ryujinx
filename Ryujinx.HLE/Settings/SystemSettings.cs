using System.Collections.Generic;
using Ryujinx.HLE.OsHle;

namespace Ryujinx.HLE.Settings
{
    public class SystemSettings
    {
        public Profile       ActiveUser { get; set; }
        public ColorSet      ThemeColor { get; set; }
        public List<Profile> UserProfiles { get; set; }
        public int           DefaultUserIndex { get; set; }
    }
}
