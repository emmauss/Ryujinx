namespace Ryujinx.HLE.Input
{
    public struct ControllerStateHeader
    {
        public long Timestamp;
        public long EntryCount;
        public long CurrentEntryIndex;
        public long MaxEntryCount;
    }
}
