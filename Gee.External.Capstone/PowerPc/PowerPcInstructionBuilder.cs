namespace Gee.External.Capstone.PowerPc {
    /// <summary>
    ///     PowerPC Instruction Builder.
    /// </summary>
    internal sealed class PowerPcInstructionBuilder : InstructionBuilder<PowerPcInstructionDetail, PowerPcDisassembleMode, PowerPcInstructionGroup, PowerPcInstructionGroupId, PowerPcInstruction, PowerPcInstructionId, PowerPcRegister, PowerPcRegisterId> {
        /// <summary>
        ///     Create a PowerPC Instruction.
        /// </summary>
        /// <returns>
        ///     A PowerPC instruction.
        /// </returns>
        internal PowerPcInstruction Create() {
            return new PowerPcInstruction(this);
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
        private protected override PowerPcInstructionDetail CreateDetails(CapstoneDisassembler disassembler, NativeInstructionHandle hInstruction) {
            return PowerPcInstructionDetail.Create(disassembler, hInstruction);
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
        private protected override PowerPcDisassembleMode CreateDisassembleMode(NativeDisassembleMode nativeDisassembleMode) {
            return (PowerPcDisassembleMode) nativeDisassembleMode;
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
        private protected override PowerPcInstructionId CreateId(int id) {
            return (PowerPcInstructionId) id;
        }
    }
}