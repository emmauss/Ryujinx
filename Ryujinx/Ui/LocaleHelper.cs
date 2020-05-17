using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using System.Threading;
using System.Text;
using System.Reflection;

namespace Ryujinx.Ui
{
    public static class LocaleHelper
    {
        private static ResourceManager _resourceManager;

        static LocaleHelper() {
            _resourceManager = new ResourceManager("Ryujinx.UI.locale.strings", Assembly.GetExecutingAssembly());
        }

        public static string GetText(string text)
        {
            var resource =  _resourceManager.GetString(text);

            return string.IsNullOrWhiteSpace(resource) ? text : resource;
        }
    }
}
