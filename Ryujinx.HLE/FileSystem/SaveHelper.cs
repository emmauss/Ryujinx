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
            string BasePartitionPath = "nand";
            long   CurrentTitleId    = SaveMetaData.TitleId;

            switch (SaveMetaData.SaveSpaceId)
            {
                case SaveSpaceId.NandUser:
                    BasePartitionPath =  Path.Combine(BasePartitionPath, "user");
                    break;
                case SaveSpaceId.NandSystem:
                    BasePartitionPath = Path.Combine(BasePartitionPath, "system");
                    break;
                case SaveSpaceId.SdCard:
                    BasePartitionPath = Path.Combine("sdmc", "Nintendo");
                    break;
            }

            BasePartitionPath = Path.Combine(BasePartitionPath, "save");

            if (SaveMetaData.TitleId == 0 && SaveMetaData.SaveDataType == SaveDataType.SaveData)
            {
                CurrentTitleId = Context.Process.MetaData.ACI0.TitleId;
            }

            string SavePath = Path.Combine(BasePartitionPath,
                SaveMetaData.SaveID.ToString("X16"),
                SaveMetaData.UserID.ToString(),
                SaveMetaData.SaveDataType == SaveDataType.SaveData ? CurrentTitleId.ToString("X16") : string.Empty);

            return SavePath;
        }
    }
}
