using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Ryujinx.HLE.HOS;

namespace Ryujinx.HLE.FileSystem
{
    static class SaveHelper
    {
        public static string GetSavePath(Save SaveMetaData, ServiceCtx Context)
        {
            string BaseSavePath   = "nand";
            long   CurrentTitleId = SaveMetaData.TitleId;

            switch (SaveMetaData.SaveSpaceId)
            {
                case SaveSpaceId.NandUser:
                    BaseSavePath =  Path.Combine(BaseSavePath, "user");
                    break;
                case SaveSpaceId.NandSystem:
                    BaseSavePath = Path.Combine(BaseSavePath, "system");
                    break;
                case SaveSpaceId.SdCard:
                    BaseSavePath = Path.Combine("sdmc", "Nintendo");
                    break;
            }

            BaseSavePath = Path.Combine(BaseSavePath, "save");

            if (SaveMetaData.TitleId == 0 && SaveMetaData.SaveDataType == SaveDataType.SaveData)
            {
                CurrentTitleId = Context.Process.MetaData.ACI0.TitleId;
            }

            string SavePath = Path.Combine(BaseSavePath,
                SaveMetaData.SaveID.ToString("x16"),
                SaveMetaData.UserID.ToString(),
                SaveMetaData.SaveDataType == SaveDataType.SaveData ? CurrentTitleId.ToString("x16") : string.Empty);

            return SavePath;
        }
    }
}
