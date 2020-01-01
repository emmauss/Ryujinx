namespace Gee.External.Capstone.Arm {
    /// <summary>
    ///     ARM Instruction Builder.
    /// </summary>
    internal sealed class ArmInstructionBuilder : InstructionBuilder<ArmInstructionDetail, ArmDisassembleMode, ArmInstructionGroup, ArmInstructionGroupId, ArmInstruction, ArmInstructionId, ArmRegister, ArmRegisterId> {
        /// <summary>
        ///     Create an ARM Instruction.
        /// </summary>
        /// <returns>
        ///     An ARM instruction.
        /// </returns>
        internal ArmInstruction Create() {
            return new ArmInstruction(this);
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
        private protected override ArmInstructionDetail CreateDetails(CapstoneDisassembler disassembler, NativeInstructionHandle hInstruction) {
            return ArmInstructionDetail.Create(disassembler, hInstruction);
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
        private protected override ArmDisassembleMode CreateDisassembleMode(NativeDisassembleMode nativeDisassembleMode) {
            return (ArmDisassembleMode) nativeDisassembleMode;
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
        private protected override ArmInstructionId CreateId(int id) {
            return (ArmInstructionId) id;
        }
    }
}