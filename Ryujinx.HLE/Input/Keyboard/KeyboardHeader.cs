namespace Ryujinx.HLE.Input
{
    public struct KeyboardHeader
    {
        public long Timestamp;
        public long EntryCount;
        public long CurrentEntryIndex;
        public long MaxEntries;
    }
}
