using System;
using System.Collections.Generic;
using System.Text;
using Gee.External.Capstone;

namespace Ryujinx.Debugger.CodeViewer
{
    public struct CodeInstruction
    {
        public long Address { get; set; }
        public byte[] Data { get; set; }

        public string Instruction { get; set; }
    }
}
