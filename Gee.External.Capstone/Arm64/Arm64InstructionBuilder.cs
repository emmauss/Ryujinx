namespace Gee.External.Capstone.Arm64 {
    /// <summary>
    ///     ARM64 Instruction Builder.
    /// </summary>
    internal sealed class Arm64InstructionBuilder : InstructionBuilder<Arm64InstructionDetail, Arm64DisassembleMode, Arm64InstructionGroup, Arm64InstructionGroupId, Arm64Instruction, Arm64InstructionId, Arm64Register, Arm64RegisterId> {
        /// <summary>
        ///     Create an ARM64 Instruction.
        /// </summary>
        /// <returns>
        ///     An ARM64 instruction.
        /// </returns>
        internal Arm64Instruction Create() {
            return new Arm64Instruction(this);
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
        private protected override Arm64InstructionDetail CreateDetails(CapstoneDisassembler disassembler, NativeInstructionHandle hInstruction) {
            return Arm64InstructionDetail.Create(disassembler, hInstruction);
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
        private protected override Arm64DisassembleMode CreateDisassembleMode(NativeDisassembleMode nativeDisassembleMode) {
            return (Arm64DisassembleMode) nativeDisassembleMode;
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
        private protected override Arm64InstructionId CreateId(int id) {
            return (Arm64InstructionId) id;
        }
    }
}