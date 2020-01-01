namespace Gee.External.Capstone.XCore {
    /// <summary>
    ///     XCore Instruction.
    /// </summary>
    public sealed class XCoreInstruction : Instruction<XCoreInstruction, XCoreInstructionDetail, XCoreDisassembleMode, XCoreInstructionGroup, XCoreInstructionGroupId, XCoreInstructionId, XCoreRegister, XCoreRegisterId> {
        /// <summary>
        ///     Create an XCore Instruction.
        /// </summary>
        /// <param name="disassembler">
        ///     A disassembler.
        /// </param>
        /// <param name="hInstruction">
        ///     An instruction handle.
        /// </param>
        /// <returns>
        ///     An XCore instruction.
        /// </returns>
        internal static XCoreInstruction Create(CapstoneDisassembler disassembler, NativeInstructionHandle hInstruction) {
            var builder = new XCoreInstructionBuilder();
            builder.Build(disassembler, hInstruction);

            return builder.Create();
        }

        /// <summary>
        ///     Create an XCore Instruction.
        /// </summary>
        /// <param name="builder">
        ///     A builder to initialize the object with.
        /// </param>
        internal XCoreInstruction(XCoreInstructionBuilder builder) : base(builder) { }
    }
}