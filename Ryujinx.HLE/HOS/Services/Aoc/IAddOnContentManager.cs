using Ryujinx.Common.Logging;
using Ryujinx.HLE.HOS.Ipc;
using Ryujinx.HLE.HOS.Kernel;
using System;
using System.Collections.Generic;

namespace Ryujinx.HLE.HOS.Services.Aoc
{
    class IAddOnContentManager : IpcService
    {
        private KEvent AddOnContentListChangedEvent;

        private Dictionary<int, ServiceProcessRequest> m_Commands;

        public override IReadOnlyDictionary<int, ServiceProcessRequest> Commands => m_Commands;

        public IAddOnContentManager(Horizon System)
        {
            m_Commands = new Dictionary<int, ServiceProcessRequest>()
            {
                { 2, CountAddOnContent               },
                { 3, ListAddOnContent                },
                { 8, GetAddOnContentListChangedEvent }
            };

            AddOnContentListChangedEvent = new KEvent(System);
        }

        // CountAddOnContent(u64, pid) -> i32
        public static long CountAddOnContent(ServiceCtx Context)
        {
            Context.ResponseData.Write(0);

            Logger.PrintStub(LogClass.ServiceAoc, "Stubbed.");

            return 0;
        }

        // ListAddOnContent(i32, i32, u64, pid) -> (i32, array<i32, 6>)
        public static long ListAddOnContent(ServiceCtx Context)
        {
            Logger.PrintStub(LogClass.ServiceAoc, "Stubbed.");

            //TODO: This is supposed to write a u32 array aswell.
            //It's unknown what it contains.
            Context.ResponseData.Write(0);

            return 0;
        }

        // GetAddOnContentListChangedEvent()
        public long GetAddOnContentListChangedEvent(ServiceCtx Context)
        {
            if (Context.Process.HandleTable.GenerateHandle(AddOnContentListChangedEvent, out int Handle) != KernelResult.Success)
            {
                throw new InvalidOperationException("Out of handles!");
            }

            Context.Response.HandleDesc = IpcHandleDesc.MakeCopy(Handle);

            return 0;
        }
    }
}