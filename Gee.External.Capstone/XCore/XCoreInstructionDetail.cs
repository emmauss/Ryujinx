namespace Gee.External.Capstone.XCore {
    /// <summary>
    ///     XCore Instruction Detail.
    /// </summary>
    public sealed class XCoreInstructionDetail : InstructionDetail<XCoreInstructionDetail, XCoreDisassembleMode, XCoreInstructionGroup, XCoreInstructionGroupId, XCoreInstruction, XCoreInstructionId, XCoreRegister, XCoreRegisterId> {
        /// <summary>
        ///     Get Instruction's Operands.
        /// </summary>
        public XCoreOperand[] Operands { get; }

        /// <summary>
        ///     Create an XCore Instruction Detail.
        /// </summary>
        /// <param name="disassembler">
        ///     A disassembler.
        /// </param>
        /// <param name="hInstruction">
        ///     An instruction handle.
        /// </param>
        /// <returns>
        ///     An XCore instruction detail.
        /// </returns>
        internal static XCoreInstructionDetail Create(CapstoneDisassembler disassembler, NativeInstructionHandle hInstruction) {
            var builder = new XCoreInstructionDetailBuilder();
            builder.Build(disassembler, hInstruction);

            return builder.Create();
        }

        /// <summary>
        ///     Create an XCore Instruction Detail.
        /// </summary>
        /// <param name="builder">
        ///     A builder to initialize the object with.
        /// </param>
        internal XCoreInstructionDetail(XCoreInstructionDetailBuilder builder) : base(builder) {
            this.Operands = builder.Operands;
        }
    }
}