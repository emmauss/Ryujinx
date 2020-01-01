namespace Gee.External.Capstone.Mips {
    /// <summary>
    ///     Capstone MIPS Disassembler.
    /// </summary>
    public sealed class CapstoneMipsDisassembler : CapstoneDisassembler<MipsDisassembleMode, MipsInstruction, MipsInstructionDetail, MipsInstructionGroup, MipsInstructionGroupId, MipsInstructionId, MipsRegister, MipsRegisterId> {
        /// <summary>
        ///     Create a Capstone MIPS Disassembler.
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
        public CapstoneMipsDisassembler(MipsDisassembleMode disassembleMode) : base(DisassembleArchitecture.Mips, disassembleMode) { }

        /// <summary>
        ///     Create an Instruction.
        /// </summary>
        /// <param name="hInstruction">
        ///     An instruction handle.
        /// </param>
        /// <returns>
        ///     A MIPS instruction.
        /// </returns>
        private protected override MipsInstruction CreateInstruction(NativeInstructionHandle hInstruction) {
            // ...
            //
            // Throws an exception if the operation fails.
            var instruction = MipsInstruction.Create(this, hInstruction);
            return instruction;
        }
    }
}