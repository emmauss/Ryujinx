using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Ryujinx.HLE.Loaders;

namespace Ryujinx.UI
{
    class Nro : HLE.Loaders.Executables.Nro
    {
        public  byte[] AssetRomfData { get; set; }
        public  byte[] IconData      { get; set; }
        private byte[] NACPData      { get; set; }
        public  int    AssetOffset   { get; set; }

        public ControlArchive ControlArchive { get; set; }


        public Nro(Stream Input, string Name) : base(Input, Name)
        {
            BinaryReader Reader = new BinaryReader(Input);

            byte[] Read(long Position, int Size)
            {
                Input.Seek(Position, SeekOrigin.Begin);

                return Reader.ReadBytes(Size);
            }

            if (Input.Length > FileSize)
            {
                AssetOffset = FileSize;

                string AssetMagic = Encoding.ASCII.GetString(Read(AssetOffset, 4));

                if (AssetMagic == "ASET")
                {
                    Input.Seek(AssetOffset, SeekOrigin.Begin);

                    int AssetMagic0             = Reader.ReadInt32();
                    int AssetFormat             = Reader.ReadInt32();
                    byte[] IconSectionInfo      = Reader.ReadBytes(0x10);
                    byte[] NACPSectionInfo      = Reader.ReadBytes(0x10);
                    byte[] AssetRomfSectionInfo = Reader.ReadBytes(0x10);

                    long IconOffset = BitConverter.ToInt64(IconSectionInfo, 0);
                    long IconSize   = BitConverter.ToInt64(IconSectionInfo, 8);
                    long NACPOffset = BitConverter.ToInt64(NACPSectionInfo, 0);
                    long NACPSize   = BitConverter.ToInt64(NACPSectionInfo, 8);
                    long RomfOffset = BitConverter.ToInt64(AssetRomfSectionInfo, 0);
                    long RomfSize   = BitConverter.ToInt64(AssetRomfSectionInfo, 8);

                    Input.Seek(AssetOffset + IconOffset, SeekOrigin.Begin);
                    IconData = Reader.ReadBytes((int)IconSize);

                    Input.Seek(AssetOffset + NACPOffset, SeekOrigin.Begin);
                    NACPData = Reader.ReadBytes((int)NACPSize);

                    Input.Seek(AssetOffset + RomfOffset, SeekOrigin.Begin);
                    AssetRomfData = Reader.ReadBytes((int)RomfSize);
                }
            }

            if (NACPData != null)
                using (MemoryStream NACPStream = new MemoryStream(NACPData))
                {
                    ControlArchive = new ControlArchive(NACPStream);
                }
        }
    }
}
