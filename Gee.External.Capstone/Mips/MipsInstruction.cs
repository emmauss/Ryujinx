namespace Gee.External.Capstone.Mips {
    /// <summary>
    ///     MIPS Instruction.
    /// </summary>
    public sealed class MipsInstruction : Instruction<MipsInstruction, MipsInstructionDetail, MipsDisassembleMode, MipsInstructionGroup, MipsInstructionGroupId, MipsInstructionId, MipsRegister, MipsRegisterId> {
        /// <summary>
        ///     Create a MIPS Instruction.
        /// </summary>
        /// <param name="disassembler">
        ///     A disassembler.
        /// </param>
        /// <param name="hInstruction">
        ///     An instruction handle.
        /// </param>
        /// <returns>
        ///     A MIPS instruction.
        /// </returns>
        internal static MipsInstruction Create(CapstoneDisassembler disassembler, NativeInstructionHandle hInstruction) {
            var builder = new MipsInstructionBuilder();
            builder.Build(disassembler, hInstruction);

            return builder.Create();
        }

        /// <summary>
        ///     Create a MIPS Instruction.
        /// </summary>
        /// <param name="builder">
        ///     A builder to initialize the object with.
        /// </param>
        internal MipsInstruction(MipsInstructionBuilder builder) : base(builder) { }
    }
}