using Ryujinx.HLE.Utilities;
using System;
using System.IO;
using System.Linq;

namespace Ryujinx.HLE.FileSystem.Content
{
    public class NcaId
    {
        public byte[] Bytes { get; private set; }

        private string NcaIdHex;

        public NcaId(long Low, long High)
        {
            byte[] Bytes = new byte[16];

            int Index = Bytes.Length;

            void WriteBytes(long Value)
            {
                for (int Byte = 0; Byte < 8; Byte++)
                {
                    Bytes[--Index] = (byte)(Value >> Byte * 8);
                }
            }

            WriteBytes(Low);
            WriteBytes(High);

            NcaIdHex = string.Empty;

            foreach (byte Byte in Bytes)
            {
                NcaIdHex += Byte.ToString("X2");
            }

            this.Bytes = Bytes;
        }

        public NcaId(byte[] Bytes)
        {
            NcaIdHex = string.Empty;

            foreach (byte Byte in Bytes)
            {
                NcaIdHex += Byte.ToString("X2");
            }

            this.Bytes = Bytes;
        }

        public NcaId(string NcaIdHex)
        {
            if(NcaIdHex.Contains("."))
            {
                NcaIdHex = NcaIdHex.Substring(0, NcaIdHex.IndexOf("."));
            }

            if (NcaIdHex == null || NcaIdHex.Length != 32 || !NcaIdHex.All("0123456789abcdefABCDEF".Contains))
            {
                throw new ArgumentException("Invalid ncaid!", nameof(NcaIdHex));
            }

            this.NcaIdHex = NcaIdHex.ToUpper();

            Bytes = StringUtils.HexToBytes(NcaIdHex);
        }

        internal void Write(BinaryWriter Writer)
        {
            for (int Index = Bytes.Length - 1; Index >= 0; Index--)
            {
                Writer.Write(Bytes[Index]);
            }
        }

        public override string ToString()
        {
            return NcaIdHex;
        }
    }
}
