using System;
using System.Collections.Generic;
using System.Text;
using Gee.External.Capstone;
using ARMeilleure.Memory;

namespace Ryujinx.Debugger.CodeViewer
{
    public static class CodeViewer
    {
        public static event EventHandler Update;

        public static long ArmStartAddress { get; set; }
        public static long ArmEndAddress { get; set; }

        private static CodeHandler _arch64CodeHandler;
        private static CodeHandler _x86_64CodeHandler;

        private static MemoryManager _armMemory;
        private static MemoryManager _jitMemory;

        public static bool Enabled => (_arch64CodeHandler.Initialized || _x86_64CodeHandler.Initialized);

        static CodeViewer()
        {
            _arch64CodeHandler = new CodeHandler(CodeType.Aarch64);
            _x86_64CodeHandler = new CodeHandler(CodeType.X86_64);
        }

        public static void LoadMemory(CodeType codeType, MemoryManager memory, long startAddress, long endAddress)
        {
            switch (codeType)
            {
                case CodeType.Aarch64:
                    _armMemory = memory;
                    _arch64CodeHandler.Initialize(startAddress);
                    ArmStartAddress = startAddress;
                    ArmEndAddress = endAddress;
                    break;
                case CodeType.X86_64:
                    _jitMemory = memory;
                    _x86_64CodeHandler.Initialize(startAddress);
                    break;
            }

            Update?.Invoke(null, null);
        }

        public static List<CodeInstruction> GetData(CodeType codeType, long offset, int length)
        {
            switch (codeType)
            {
                case CodeType.Aarch64:
                    if (!_arch64CodeHandler.Initialized)
                    {
                        throw new InvalidOperationException($"{codeType.ToString()} disassembler is not initialized.");
                    };

                    return _arch64CodeHandler.DisassembleBlock(_armMemory, offset, length);
                case CodeType.X86_64:
                    if (!_x86_64CodeHandler.Initialized)
                    {
                        throw new InvalidOperationException($"{codeType.ToString()} disassembler is not initialized.");
                    };

                    return _arch64CodeHandler.DisassembleBlock(_jitMemory, offset, length);
            }

            return null;
        }
    }
}
