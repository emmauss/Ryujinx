using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Ryujinx.UI
{
    public class ControlArchive
    {
        public LanguageEntry[] LanguageEntries { get; set; }
        public long            ApplicationTitleID { get; set; }
        public long            BaseTitleID { get; set; }
        public long            ProductCode { get; set; }
        public string          ApplicationVersion { get; set; }

        public ControlArchive(Stream Input)
        {
            BinaryReader Reader = new BinaryReader(Input);

            byte[] LanguageEntryData = Reader.ReadBytes(0x3000);

            Input.Seek(0x3060, SeekOrigin.Begin);
            ApplicationVersion = Encoding.ASCII.GetString(Reader.ReadBytes(0x10));
            BaseTitleID        = Reader.ReadInt64();
            ApplicationTitleID = Reader.ReadInt64();

            Input.Seek(0x30a8, SeekOrigin.Begin);
            ProductCode = Reader.ReadInt64();

            LanguageEntries = new LanguageEntry[16];

            using (MemoryStream LanguageStream = new MemoryStream(LanguageEntryData))
            {
                BinaryReader LanguageReader = new BinaryReader(LanguageStream);
                for (int index = 0; index < 16; index++)
                {
                    LanguageEntries[index] = new LanguageEntry()
                    {
                        AplicationName = Encoding.ASCII.GetString(LanguageReader.ReadBytes(0x200)).Trim('\0'),
                        DeveloperName  = Encoding.ASCII.GetString(LanguageReader.ReadBytes(0x100)).Trim('\0')
                    };
                }
            }
        }
    }


    public struct LanguageEntry
    {
        public string AplicationName;
        public string DeveloperName;
    }
}
