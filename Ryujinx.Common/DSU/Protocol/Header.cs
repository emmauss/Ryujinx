using System.Runtime.InteropServices;

namespace Ryujinx.Common.DSU
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Header
    {
        public uint MagicString;
        public ushort Version;
        public ushort Length;
        public uint CRC32;
        public uint ID;
    }
}
