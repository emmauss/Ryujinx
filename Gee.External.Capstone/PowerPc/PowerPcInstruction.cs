namespace Gee.External.Capstone.PowerPc {
    /// <summary>
    ///     PowerPC Instruction.
    /// </summary>
    public sealed class PowerPcInstruction : Instruction<PowerPcInstruction, PowerPcInstructionDetail, PowerPcDisassembleMode, PowerPcInstructionGroup, PowerPcInstructionGroupId, PowerPcInstructionId, PowerPcRegister, PowerPcRegisterId> {
        /// <summary>
        ///     Create a PowerPC Instruction.
        /// </summary>
        /// <param name="disassembler">
        ///     A disassembler.
        /// </param>
        /// <param name="hInstruction">
        ///     An instruction handle.
        /// </param>
        /// <returns>
        ///     A PowerPC instruction.
        /// </returns>
        internal static PowerPcInstruction Create(CapstoneDisassembler disassembler, NativeInstructionHandle hInstruction) {
            var builder = new PowerPcInstructionBuilder();
            builder.Build(disassembler, hInstruction);

            return builder.Create();
        }

        /// <summary>
        ///     Create a PowerPC Instruction.
        /// </summary>
        /// <param name="builder">
        ///     A builder to initialize the object with.
        /// </param>
        internal PowerPcInstruction(PowerPcInstructionBuilder builder) : base(builder) { }
    }
}