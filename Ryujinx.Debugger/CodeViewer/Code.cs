using System;
using SharpDisasm;
using SharpDisasm.Translators;

namespace Ryujinx.Debugger.CodeViewer
{
    public class Code
    {
        public IntPtr Address{ get; set; }
        public Code(IntPtr address)
        {
            Address = address;
        }

        public string ToAsm()
        {
            
        }

        public Span<byte> AsSpan()
        {
            return new Span<byte>(Address.ToPointer(), 4);
        }
    }
}
