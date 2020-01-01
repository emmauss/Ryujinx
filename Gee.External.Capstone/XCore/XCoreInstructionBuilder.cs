namespace Gee.External.Capstone.XCore {
    /// <summary>
    ///     XCore Instruction Builder.
    /// </summary>
    internal sealed class XCoreInstructionBuilder : InstructionBuilder<XCoreInstructionDetail, XCoreDisassembleMode, XCoreInstructionGroup, XCoreInstructionGroupId, XCoreInstruction, XCoreInstructionId, XCoreRegister, XCoreRegisterId> {
        /// <summary>
        ///     Create an XCore Instruction.
        /// </summary>
        /// <returns>
        ///     An XCore instruction.
        /// </returns>
        internal XCoreInstruction Create() {
            return new XCoreInstruction(this);
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
        private protected override XCoreInstructionDetail CreateDetails(CapstoneDisassembler disassembler, NativeInstructionHandle hInstruction) {
            return XCoreInstructionDetail.Create(disassembler, hInstruction);
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
        private protected override XCoreDisassembleMode CreateDisassembleMode(NativeDisassembleMode nativeDisassembleMode) {
            return (XCoreDisassembleMode) nativeDisassembleMode;
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
        private protected override XCoreInstructionId CreateId(int id) {
            return (XCoreInstructionId) id;
        }
    }
}