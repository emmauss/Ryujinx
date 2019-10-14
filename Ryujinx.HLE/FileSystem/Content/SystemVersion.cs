using System;
using System.IO;
using System.Text;

namespace Ryujinx.HLE.FileSystem.Content
{
    public class SystemVersion
    {
        public byte   Major          { get; set; }
        public byte   Minor          { get; set; }
        public byte   Micro          { get; set; }
        public byte   RevisionMajor  { get; set; }
        public byte   RevisionMinor  { get; set; }
        public string PlatformString { get; set; }
        public string Hex            { get; set; }
        public string VersionString  { get; set; }
        public string VersionTitle   { get; set; }

        public SystemVersion(Stream systemVersionFile)
        {
            using(BinaryReader reader = new BinaryReader(systemVersionFile))
            {
                Major = reader.ReadByte();
                Minor = reader.ReadByte();
                Micro = reader.ReadByte();

                reader.ReadByte(); // Padding

                RevisionMajor = reader.ReadByte();
                RevisionMinor = reader.ReadByte();

                reader.ReadBytes(2); // Padding

                PlatformString = Encoding.ASCII.GetString(reader.ReadBytes(0x20)).TrimEnd('\0');
                Hex            = Encoding.ASCII.GetString(reader.ReadBytes(0x40)).TrimEnd('\0');
                VersionString  = Encoding.ASCII.GetString(reader.ReadBytes(0x18)).TrimEnd('\0');
                VersionTitle   = Encoding.ASCII.GetString(reader.ReadBytes(0x80)).TrimEnd('\0');
            }
        }
    }
}