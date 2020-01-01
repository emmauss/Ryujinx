namespace Gee.External.Capstone.Arm {
    /// <summary>
    ///     ARM Instruction.
    /// </summary>
    public sealed class ArmInstruction : Instruction<ArmInstruction, ArmInstructionDetail, ArmDisassembleMode, ArmInstructionGroup, ArmInstructionGroupId, ArmInstructionId, ArmRegister, ArmRegisterId> {
        /// <summary>
        ///     Create an ARM Instruction.
        /// </summary>
        /// <param name="disassembler">
        ///     A disassembler.
        /// </param>
        /// <param name="hInstruction">
        ///     An instruction handle.
        /// </param>
        /// <returns>
        ///     An ARM instruction.
        /// </returns>
        internal static ArmInstruction Create(CapstoneDisassembler disassembler, NativeInstructionHandle hInstruction) {
            var builder = new ArmInstructionBuilder();
            builder.Build(disassembler, hInstruction);

            return builder.Create();
        }

        /// <summary>
        ///     Create an ARM Instruction.
        /// </summary>
        /// <param name="builder">
        ///     A builder to initialize the object with.
        /// </param>
        internal ArmInstruction(ArmInstructionBuilder builder) : base(builder) { }
    }
}