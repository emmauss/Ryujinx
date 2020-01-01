namespace Gee.External.Capstone.Arm64 {
    /// <summary>
    ///     Capstone ARM64 Disassembler.
    /// </summary>
    public sealed class CapstoneArm64Disassembler : CapstoneDisassembler<Arm64DisassembleMode, Arm64Instruction, Arm64InstructionDetail, Arm64InstructionGroup, Arm64InstructionGroupId, Arm64InstructionId, Arm64Register, Arm64RegisterId> {
        /// <summary>
        ///     Create a Capstone ARM Disassembler.
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
        public CapstoneArm64Disassembler(Arm64DisassembleMode disassembleMode) : base(DisassembleArchitecture.Arm64, disassembleMode) { }

        /// <summary>
        ///     Create an Instruction.
        /// </summary>
        /// <param name="hInstruction">
        ///     An instruction handle.
        /// </param>
        /// <returns>
        ///     An ARM64 instruction.
        /// </returns>
        private protected override Arm64Instruction CreateInstruction(NativeInstructionHandle hInstruction) {
            // ...
            //
            // Throws an exception if the operation fails.
            var instruction = Arm64Instruction.Create(this, hInstruction);
            return instruction;
        }
    }
}