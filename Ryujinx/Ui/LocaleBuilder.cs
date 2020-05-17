using Gtk;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

using static Ryujinx.Ui.LocaleHelper;

namespace Ryujinx.Ui
{
    public class LocaleBuilder : Builder
    {
        public LocaleBuilder(string resourceName, string translationDomain = "ryujinx") : base()
        {
            var resourceStream = Assembly.GetCallingAssembly().GetManifestResourceStream(resourceName);

            if(resourceStream == null)
            {
                throw new ArgumentException("Cannot get resource file '" + resourceName + "'",
                                             "resourceName");
            }

            if (!string.IsNullOrWhiteSpace(translationDomain))
            {
                TranslateDocument(resourceStream, out Stream translated);

                resourceStream.Dispose();
                resourceStream = translated;
            }

            AddFromStream(resourceStream);
            TranslationDomain = translationDomain;
        }

        // https://github.com/GtkSharp/GtkSharp/blob/develop/Source/Libs/GtkSharp/Builder.cs
        private uint AddFromStream(Stream stream)
        {
            var size = (int)stream.Length;
            var buffer = (stream as MemoryStream).ToArray();
            stream.Close();

            // If buffer contains a BOM, omit it while reading, otherwise AddFromString(text) crashes
            var offset = 0;
            if (size >= 3 && buffer[0] == 0xEF && buffer[1] == 0xBB && buffer[2] == 0xBF)
            {
                offset = 3;
            }

            var text = Encoding.UTF8.GetString(buffer, offset, size - offset);
             return AddFromString(text);
        }

        private void TranslateDocument(Stream source, out Stream stream)
        {
            XDocument doc = XDocument.Load(source);

            var elements = doc.Descendants("property");

            foreach(var element in elements)
            {
                var attribute = element.Attribute("translatable");

                if(attribute!=null && attribute.Value == "yes")
                {
                    element.Value = GetText(element.Value);
                }
            }

            stream = new MemoryStream();

            doc.Save(stream);
        }
    }
}
