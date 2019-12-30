using System;
using SharpDisasm;
using SharpDisasm.Translators;

namespace Ryujinx.Debugger.CodeViewer
{
    public static class CodeViewer
    {
        public static IntPtr CodePointer{ get; set; }

        private static ArchitectureMode _mode;
        private static Disassembler _disassembler;

        static CodeViewer()
        {
            Disassembler.Translator.IncludeAddress = true;
            Disassembler.Translator.IncludeBinary  = true;

            _mode = ArchitectureMode.x86_64;
        }

        public void Initialize()
        {


            _disassembler = new Disassembler().di
        }
    }
}
