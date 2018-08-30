using Ryujinx.HLE.HOS;
using Ryujinx.HLE.HOS.SystemState;

namespace Ryujinx.HLE.FileSystem
{
    struct SaveInfo
    {
        public long   TitleId { get; private set; }
        public long   SaveID  { get; private set; }
        public UserId UserID  { get; private set; }

        public SaveDataType SaveDataType { get; private set; }
        public SaveSpaceId  SaveSpaceId  { get; private set; }

        public SaveInfo(ServiceCtx Context, SaveSpaceId SaveSpaceId)
        {
            TitleId          = Context.RequestData.ReadInt64();
            UserID           = new UserId(Context.RequestData.ReadInt64(), Context.RequestData.ReadInt64());
            SaveID           = Context.RequestData.ReadInt64();
            SaveDataType     = (SaveDataType)Context.RequestData.ReadByte();
            this.SaveSpaceId = SaveSpaceId;
        }
    }
}
