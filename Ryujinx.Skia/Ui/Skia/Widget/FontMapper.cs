using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using SkiaSharp;
using Topten.RichTextKit;
using Typography.OpenFont;

namespace Ryujinx.Skia.Ui.Skia.Widget
{
    public class FontMapper : Topten.RichTextKit.FontMapper
    {
        public static Dictionary<string, ushort> IconMap { get; set; }
        private readonly SKTypeface _typeface;
        private static Dictionary<ushort, uint> _unicodeMap;

        public FontMapper()
        {
            string resourceID = "Ryujinx.Skia.Ui.Assets.ionicons.ttf";
            Assembly assembly = GetType().GetTypeInfo().Assembly;

            Stream stream = assembly.GetManifestResourceStream(resourceID);

            IconMap = new Dictionary<string, ushort>();

            var font = new OpenFontReader();

            var glyphTypeface = font.Read(stream);

            var unicodes = new List<uint>();

            glyphTypeface.CollectUnicode(unicodes);

            _unicodeMap = new Dictionary<ushort, uint>();

            foreach (uint unicode in unicodes)
            {
                var index = glyphTypeface.GetGlyphIndex((int)unicode);

                _unicodeMap.TryAdd(index, unicode);
            }

            foreach (GlyphNameMap glyphNameMap in glyphTypeface.GetGlyphNameIter())
            {
                IconMap.Add(glyphNameMap.glyphName, glyphNameMap.glyphIndex);
            };

            stream = assembly.GetManifestResourceStream(resourceID);

            _typeface = SKTypeface.FromStream(stream);
        }

        public static string GetGlyphUnicodeCodepoint(string name)
        {
            if (IconMap.TryGetValue(name, out ushort index))
            {
                try
                {
                    if (_unicodeMap.TryGetValue(index, out uint unicode))
                    {
                        return char.ConvertFromUtf32((int)unicode);
                    }
                }
                catch (Exception)
                {
                    // index not found
                }
            }

            return "\0";
        }
        public override SKTypeface TypefaceFromStyle(IStyle style, bool ignoreFontVariants)
        {
            if (style.FontFamily == "IonIcon")
                return _typeface;

            return base.TypefaceFromStyle(style, ignoreFontVariants);
        }
    }
}