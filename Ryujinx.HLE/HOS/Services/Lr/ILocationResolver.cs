using System;
using System.Collections.Generic;
using Ryujinx.HLE.HOS.Ipc;
using Ryujinx.HLE.FileSystem;

namespace Ryujinx.HLE.HOS.Services.Lr
{
    class ILocationResolver : IpcService
    {
        private Dictionary<int, ServiceProcessRequest> m_Commands;

        public override IReadOnlyDictionary<int, ServiceProcessRequest> Commands => m_Commands;

        private StorageId StorageId;

        public ILocationResolver(StorageId StorageId)
        {
            m_Commands = new Dictionary<int, ServiceProcessRequest>()
            {
               /* { 0, ResolveProgramPath },
                { 9, Refresh            }*/
            };

            this.StorageId = StorageId;
        }

        private long Refresh(ServiceCtx Context)
        {
            throw new NotImplementedException();
        }

       /* private long ResolveProgramPath(ServiceCtx Context)
        {
            
        }*/
    }
}
