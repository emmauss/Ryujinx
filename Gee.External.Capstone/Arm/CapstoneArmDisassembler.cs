namespace Gee.External.Capstone.Arm {
    /// <summary>
    ///     Capstone ARM Disassembler.
    /// </summary>
    public sealed class CapstoneArmDisassembler : CapstoneDisassembler<ArmDisassembleMode, ArmInstruction, ArmInstructionDetail, ArmInstructionGroup, ArmInstructionGroupId, ArmInstructionId, ArmRegister, ArmRegisterId> {
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
        public CapstoneArmDisassembler(ArmDisassembleMode disassembleMode) : base(DisassembleArchitecture.Arm, disassembleMode) { }

        /// <summary>
        ///     Create an Instruction.
        /// </summary>
        /// <param name="hInstruction">
        ///     An instruction handle.
        /// </param>
        /// <returns>
        ///     An ARM instruction.
        /// </returns>
        private protected override ArmInstruction CreateInstruction(NativeInstructionHandle hInstruction) {
            // ...
            //
            // Throws an exception if the operation fails.
            var instruction = ArmInstruction.Create(this, hInstruction);
            return instruction;
        }
    }
}