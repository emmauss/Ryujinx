namespace Gee.External.Capstone.X86 {
    /// <summary>
    ///     X86 Instruction Builder.
    /// </summary>
    internal sealed class X86InstructionBuilder : InstructionBuilder<X86InstructionDetail, X86DisassembleMode, X86InstructionGroup, X86InstructionGroupId, X86Instruction, X86InstructionId, X86Register, X86RegisterId> {
        /// <summary>
        ///     Create an X86 Instruction.
        /// </summary>
        /// <returns>
        ///     An X86 instruction.
        /// </returns>
        internal X86Instruction Create() {
            return new X86Instruction(this);
        }

        /// <summary>
        ///     Create Instruction's Details.
        /// </summary>
        /// <param name="disassembler">
        ///     A disassembler.
        /// </param>
        /// <param name="hInstruction">
        ///     An instruction handle.
        /// </param>
        /// <returns>
        ///     The instruction's details.
        /// </returns>
        private protected override X86InstructionDetail CreateDetails(CapstoneDisassembler disassembler, NativeInstructionHandle hInstruction) {
            return X86InstructionDetail.Create(disassembler, hInstruction);
        }

        /// <summary>
        ///     Create Disassemble Mode.
        /// </summary>
        /// <param name="nativeDisassembleMode">
        ///     A native disassemble mode.
        /// </param>
        /// <returns>
        ///     A disassemble mode.
        /// </returns>
        private protected override X86DisassembleMode CreateDisassembleMode(NativeDisassembleMode nativeDisassembleMode) {
            return (X86DisassembleMode) nativeDisassembleMode;
        }

        /// <summary>
        ///     Create Instruction's Unique Identifier.
        /// </summary>
        /// <param name="id">
        ///     An instruction's unique identifier.
        /// </param>
        /// <returns>
        ///     The instruction's unique identifier.
        /// </returns>
        private protected override X86InstructionId CreateId(int id) {
            return (X86InstructionId) id;
        }
    }
}