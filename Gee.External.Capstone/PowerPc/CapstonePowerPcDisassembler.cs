namespace Gee.External.Capstone.PowerPc {
    /// <summary>
    ///     Capstone PowerPC Disassembler.
    /// </summary>
    public sealed class CapstonePowerPcDisassembler : CapstoneDisassembler<PowerPcDisassembleMode, PowerPcInstruction, PowerPcInstructionDetail, PowerPcInstructionGroup, PowerPcInstructionGroupId, PowerPcInstructionId, PowerPcRegister, PowerPcRegisterId> {
        /// <summary>
        ///     Create a Capstone PowerPC Disassembler.
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
        public CapstonePowerPcDisassembler(PowerPcDisassembleMode disassembleMode) : base(DisassembleArchitecture.PowerPc, disassembleMode) { }

        /// <summary>
        ///     Create an Instruction.
        /// </summary>
        /// <param name="hInstruction">
        ///     An instruction handle.
        /// </param>
        /// <returns>
        ///     A PowerPC instruction.
        /// </returns>
        private protected override PowerPcInstruction CreateInstruction(NativeInstructionHandle hInstruction) {
            // ...
            //
            // Throws an exception if the operation fails.
            var instruction = PowerPcInstruction.Create(this, hInstruction);
            return instruction;
        }
    }
}