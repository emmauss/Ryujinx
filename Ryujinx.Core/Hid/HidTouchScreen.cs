using System.Runtime.InteropServices;

namespace Ryujinx.Core
{
    [StructLayout(LayoutKind.Sequential, Size = 0x28)]
    public struct HidTouchScreenHeader
    {
        public ulong TimestampTicks;
        public ulong NumEntries;
        public ulong LatestEntry;
        public ulong MaxEntryIndex;
        public ulong Timestamp;
    }

    [StructLayout(LayoutKind.Sequential, Size = 0x10)]
    public struct HidTouchScreenEntryHeader
    {
        public ulong Timestamp;
        public ulong NumTouches;
    }

    [StructLayout(LayoutKind.Sequential, Size = 0x28)]
    public struct HidTouchScreenEntryTouch
    {
        public ulong Timestamp;
        public uint Padding;
        public uint TouchIndex;
        public uint X;
        public uint Y;
        public uint DiameterX;
        public uint DiameterY;
        public uint Angle;
        public uint Padding_2;
    }

    [StructLayout(LayoutKind.Sequential, Size = 0x298)]
    public struct HidTouchScreenEntry
    {
        public HidTouchScreenEntryHeader Header;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public HidTouchScreenEntryTouch[] Touches;
        public ulong Unknown;
    }

    [StructLayout(LayoutKind.Sequential, Size = 0x3000)]
    public struct HidTouchScreen
    {
        public HidTouchScreenHeader Header;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
        public HidTouchScreenEntry[] Entries;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x3C0)]
        public byte[] Padding;
    }

    public struct Touches
    {
        public uint[] XTouches;
        public uint[] YTouches;
        public uint NumberOfTouches { get; private set; }
        public const uint Hid_Max_Num_Touches = 16;

        public void AddTouch(uint X, uint Y)
        {
            if (NumberOfTouches < Hid_Max_Num_Touches)
            {
                XTouches[NumberOfTouches] = X;
                YTouches[NumberOfTouches] = Y;
                NumberOfTouches++;
            }
        }
    }

}