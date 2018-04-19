using Ryujinx.Core.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Ryujinx.Core.Logging;

namespace Ryujinx.UI
{
    public class Configuration
    {
        public bool EnableMemoryChecks { get; set; }
        public bool LoggingEnableInfo { get; set; }
        public bool LoggingEnableDebug { get; set; }
        public bool LoggingEnableWarn { get; set; }
        public bool LoggingEnableError { get; set; }
        public bool LoggingEnableStub { get; set; }

        public string LoggingFilteredClasses { get; set; }

        public JoyCon EmulatedJoyCon { get; set; }
    }
}