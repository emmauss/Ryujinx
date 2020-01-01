namespace Gee.External.Capstone.XCore {
    /// <summary>
    ///     Capstone XCore Disassembler.
    /// </summary>
    public sealed class CapstoneXCoreDisassembler : CapstoneDisassembler<XCoreDisassembleMode, XCoreInstruction, XCoreInstructionDetail, XCoreInstructionGroup, XCoreInstructionGroupId, XCoreInstructionId, XCoreRegister, XCoreRegisterId> {
        /// <summary>
        ///     Create a Capstone XCore Disassembler.
        /// </summary>
        /// <param name="disassembleMode">
        ///     The hardware mode for the disassembler to use.
        /// </param>
        /// <exception cref="Gee.External.Capstone.CapstoneException">
        ///     Thrown if a disassembler could not be created.
        /// </exception>
        /// <exception cref="System.OutOfMemoryException">
        ///     Thrown if sufficient memory cannot be allocated to perform the operation as a rare indication that the
        ///     system is under heavy load.
        /// </exception>
        public CapstoneXCoreDisassembler(XCoreDisassembleMode disassembleMode) : base(DisassembleArchitecture.XCore, disassembleMode) { }

        /// <summary>
        ///     Create an Instruction.
        /// </summary>
        /// <param name="hInstruction">
        ///     An instruction handle.
        /// </param>
        /// <returns>
        ///     An XCore instruction.
        /// </returns>
        private protected override XCoreInstruction CreateInstruction(NativeInstructionHandle hInstruction) {
            // ...
            //
            // Throws an exception if the operation fails.
            var instruction = XCoreInstruction.Create(this, hInstruction);
            return instruction;
        }
    }
}