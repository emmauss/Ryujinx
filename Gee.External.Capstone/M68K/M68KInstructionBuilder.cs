namespace Gee.External.Capstone.M68K {
    /// <summary>
    ///     M68K Instruction Builder.
    /// </summary>
    internal sealed class M68KInstructionBuilder : InstructionBuilder<M68KInstructionDetail, M68KDisassembleMode, M68KInstructionGroup, M68KInstructionGroupId, M68KInstruction, M68KInstructionId, M68KRegister, M68KRegisterId> {
        /// <summary>
        ///     Create an M68K Instruction.
        /// </summary>
        /// <returns>
        ///     An M68K instruction.
        /// </returns>
        internal M68KInstruction Create() {
            return new M68KInstruction(this);
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
        private protected override M68KInstructionDetail CreateDetails(CapstoneDisassembler disassembler, NativeInstructionHandle hInstruction) {
            return M68KInstructionDetail.Create(disassembler, hInstruction);
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
        private protected override M68KDisassembleMode CreateDisassembleMode(NativeDisassembleMode nativeDisassembleMode) {
            return (M68KDisassembleMode) nativeDisassembleMode;
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
        private protected override M68KInstructionId CreateId(int id) {
            return (M68KInstructionId) id;
        }
    }
}