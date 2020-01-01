namespace Gee.External.Capstone.M68K {
    /// <summary>
    ///     Capstone M68K Disassembler.
    /// </summary>
    public sealed class CapstoneM68KDisassembler : CapstoneDisassembler<M68KDisassembleMode, M68KInstruction, M68KInstructionDetail, M68KInstructionGroup, M68KInstructionGroupId, M68KInstructionId, M68KRegister, M68KRegisterId> {
        /// <summary>
        ///     Create a Capstone M68K Disassembler.
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
        public CapstoneM68KDisassembler(M68KDisassembleMode disassembleMode) : base(DisassembleArchitecture.M68K, disassembleMode) { }

        /// <summary>
        ///     Create an Instruction.
        /// </summary>
        /// <param name="hInstruction">
        ///     An instruction handle.
        /// </param>
        /// <returns>
        ///     An M68K instruction.
        /// </returns>
        private protected override M68KInstruction CreateInstruction(NativeInstructionHandle hInstruction) {
            // ...
            //
            // Throws an exception if the operation fails.
            var instruction = M68KInstruction.Create(this, hInstruction);
            return instruction;
        }
    }
}