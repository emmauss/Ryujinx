using System;
using System.Collections.Generic;
using Ryujinx.HLE.HOS.Ipc;
using Ryujinx.HLE.FileSystem;
using Ryujinx.HLE.FileSystem.Content;

namespace Ryujinx.HLE.HOS.Services.Ncm
{
    class IContentStorage : IpcService
    {
        private Dictionary<int, ServiceProcessRequest> m_Commands;

        public override IReadOnlyDictionary<int, ServiceProcessRequest> Commands => m_Commands;

        public IContentStorage()
        {
            m_Commands = new Dictionary<int, ServiceProcessRequest>()
            {
                //{8, GetPath }
            };
        }

        /*private long GetPath(ServiceCtx Context)
        {
            NcaId NcaId = new NcaId(Context.RequestData.ReadBytes(0x10));


        }*/
    }
}
