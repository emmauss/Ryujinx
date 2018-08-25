using Ryujinx.HLE.HOS.SystemState;

namespace Ryujinx.HLE.FileSystem
{
    struct SaveInfo
    {
        public long   TitleId { get; set; }
        public long   SaveID  { get; set; }
        public UserId UserID  { get; set; }

        public SaveDataType SaveDataType { get; set; }
        public SaveSpaceId  SaveSpaceId  { get; set; }
    }
}
