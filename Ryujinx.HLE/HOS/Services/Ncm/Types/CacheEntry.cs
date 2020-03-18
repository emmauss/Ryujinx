using LibHac.Fs;
using Ryujinx.HLE.Utilities;

namespace Ryujinx.HLE.HOS.Services.Ncm.Types
{
    public class CacheEntry
    {
        public UInt128 PlaceHolderId;
        public IFile Handle;

        public void Close()
        {
            Handle?.Dispose();

            PlaceHolderId = new UInt128(0, 0);
        }
    }
}