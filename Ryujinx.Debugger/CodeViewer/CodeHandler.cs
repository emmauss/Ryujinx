using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Gee.External.Capstone;
using Gee.External.Capstone.Arm64;
using Gee.External.Capstone.X86;
using Ryujinx.Common;
using ARMeilleure.Memory;

namespace Ryujinx.Debugger.CodeViewer
{
    internal class CodeHandler : IDisposable
    {
        public const int Arch64InstructionSize = 4;
        public const int X86_64InstructionSize = 4;
        public CodeType CodeType { get; }
        public bool Initialized { get; set; }

        private long _address;

        private CapstoneDisassembler _disassembler;
        public CodeHandler(CodeType codeType)
        {
            CodeType = codeType;
        }

        public void Initialize(long address)
        {
            switch (CodeType)
            {
                case CodeType.Aarch64:
                    var disassembler = new CapstoneArm64Disassembler(Arm64DisassembleMode.Arm);
                    disassembler.DisassembleSyntax = DisassembleSyntax.Intel;
                    disassembler.EnableSkipDataMode = true;
                    _disassembler = disassembler;
                    break;
                case CodeType.X86_64:
                    _disassembler = new CapstoneX86Disassembler(X86DisassembleMode.LittleEndian);
                    break;
            }

            _address = address;

            _disassembler.EnableInstructionDetails = false;

            Initialized = true;
        }

        public List<CodeInstruction> DisassembleBlock(MemoryManager memory, long offset, int length = 100)
        {
            List<CodeInstruction> disassembledCode = new List<CodeInstruction>();

            if (offset >= 0 && length > 0)
            {
                switch (CodeType)
                {
                    case CodeType.Aarch64:
                        byte[] region = memory.ReadBytes(offset, length * Arch64InstructionSize);

                        var str = HexUtils.ToHex(region);

                        var disassembled  = (_disassembler as CapstoneArm64Disassembler).Disassemble(region);

                        foreach (Arm64Instruction instruction in disassembled)
                        {
                            disassembledCode.Add(new CodeInstruction()
                            {
                                Address = instruction.Address + offset, //Capstone's dumb
                                Data = instruction.Bytes,
                                Instruction = $"{instruction.Mnemonic} {instruction.Operand}"
                            });
                        }

                        break;
                }
            }

            return disassembledCode;
        }

        public void Dispose()
        {
            _disassembler?.Dispose();
        }
    }
}
