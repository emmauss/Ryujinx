using System;
using System.Drawing;
using System.IO;
using System.Text;

namespace Ryujinx.HLE.Loaders.Executables
{
    class Nro : IExecutable
    {
        public string Name { get; private set; }

        public byte[] Text          { get; private set; }
        public byte[] RO            { get; private set; }
        public byte[] Data          { get; private set; }
        public byte[] AssetRomfData { get; set; }
        public byte[] IconData      { get; set; }
        public byte[] NCAPData      { get; set; }

        public int Mod0Offset  { get; private set; }
        public int TextOffset  { get; private set; }
        public int ROOffset    { get; private set; }
        public int DataOffset  { get; private set; }
        public int BssSize     { get; private set; }
        public int AssetOffset { get; set; }

        public Nro(Stream Input, string Name)
        {
            this.Name = Name;

            BinaryReader Reader = new BinaryReader(Input);

            Input.Seek(4, SeekOrigin.Begin);

            int Mod0Offset = Reader.ReadInt32();
            int Padding8   = Reader.ReadInt32();
            int Paddingc   = Reader.ReadInt32();
            int NroMagic   = Reader.ReadInt32();
            int Unknown14  = Reader.ReadInt32();
            int FileSize   = Reader.ReadInt32();
            int Unknown1c  = Reader.ReadInt32();
            int TextOffset = Reader.ReadInt32();
            int TextSize   = Reader.ReadInt32();
            int ROOffset   = Reader.ReadInt32();
            int ROSize     = Reader.ReadInt32();
            int DataOffset = Reader.ReadInt32();
            int DataSize   = Reader.ReadInt32();
            int BssSize    = Reader.ReadInt32();

            this.Mod0Offset  = Mod0Offset;
            this.TextOffset  = TextOffset;
            this.ROOffset    = ROOffset;
            this.DataOffset  = DataOffset;
            this.BssSize     = BssSize;
            this.AssetOffset = FileSize;

            byte[] Read(long Position, int Size)
            {
                Input.Seek(Position, SeekOrigin.Begin);

                return Reader.ReadBytes(Size);
            }

            Text = Read(TextOffset, TextSize);
            RO   = Read(ROOffset,   ROSize);
            Data = Read(DataOffset, DataSize);

            if (Input.Length > FileSize)
            {
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
                    NCAPData = Reader.ReadBytes((int)NACPSize);

                    Input.Seek(AssetOffset + RomfOffset, SeekOrigin.Begin);
                    AssetRomfData = Reader.ReadBytes((int)RomfSize);
                }
            }
        }
    }
}