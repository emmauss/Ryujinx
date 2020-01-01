namespace Gee.External.Capstone.XCore {
    /// <summary>
    ///     XCore Instruction Detail Builder.
    /// </summary>
    internal sealed class XCoreInstructionDetailBuilder : InstructionDetailBuilder<XCoreInstructionDetail, XCoreDisassembleMode, XCoreInstructionGroup, XCoreInstructionGroupId, XCoreInstruction, XCoreInstructionId, XCoreRegister, XCoreRegisterId> {
        /// <summary>
        ///     Get and Set Instruction's Operands.
        /// </summary>
        internal XCoreOperand[] Operands { get; private set; }

        /// <summary>
        ///     Build an Instruction Detail.
        /// </summary>
        /// <param name="disassembler">
        ///     A disassembler.
        /// </param>
        /// <param name="hInstruction">
        ///     An instruction handle.
        /// </param>
        internal override void Build(CapstoneDisassembler disassembler, NativeInstructionHandle hInstruction) {
            // ...
            //
            // Throws an exception if the operation fails.
            base.Build(disassembler, hInstruction);
            var nativeInstructionDetail = NativeCapstone.GetInstructionDetail<NativeXCoreInstructionDetail>(hInstruction).GetValueOrDefault();

            this.Operands = XCoreOperand.Create(disassembler, ref nativeInstructionDetail);
        }

        /// <summary>
        ///     Create an XCore Instruction Detail.
        /// </summary>
        /// <returns>
        ///     An XCore instruction detail.
        /// </returns>
        internal XCoreInstructionDetail Create() {
            return new XCoreInstructionDetail(this);
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
        ///     Create an Instruction Group.
        /// </summary>
        /// <param name="disassembler">
        ///     A disassembler.
        /// </param>
        /// <param name="instructionGroupId">
        ///     An instruction group's unique identifier.
        /// </param>
        /// <returns>
        ///     An instruction group.
        /// </returns>
        private protected override XCoreInstructionGroup CreateInstructionGroup(CapstoneDisassembler disassembler, byte instructionGroupId) {
            return XCoreInstructionGroup.Create(disassembler, (XCoreInstructionGroupId) instructionGroupId);
        }

        /// <summary>
        ///     Create a Register.
        /// </summary>
        /// <param name="disassembler">
        ///     A disassembler.
        /// </param>
        /// <param name="registerId">
        ///     A register's unique identifier.
        /// </param>
        /// <returns>
        ///     A register.
        /// </returns>
        private protected override XCoreRegister CreateRegister(CapstoneDisassembler disassembler, short registerId) {
            return XCoreRegister.TryCreate(disassembler, (XCoreRegisterId) registerId);
        }
    }
}