namespace Gee.External.Capstone.M68K {
    /// <summary>
    ///     M68K Instruction.
    /// </summary>
    public sealed class M68KInstruction : Instruction<M68KInstruction, M68KInstructionDetail, M68KDisassembleMode, M68KInstructionGroup, M68KInstructionGroupId, M68KInstructionId, M68KRegister, M68KRegisterId> {
        /// <summary>
        ///     Create an M68K Instruction.
        /// </summary>
        /// <param name="disassembler">
        ///     A disassembler.
        /// </param>
        /// <param name="hInstruction">
        ///     An instruction handle.
        /// </param>
        /// <returns>
        ///     An M68K instruction.
        /// </returns>
        internal static M68KInstruction Create(CapstoneDisassembler disassembler, NativeInstructionHandle hInstruction) {
            var builder = new M68KInstructionBuilder();
            builder.Build(disassembler, hInstruction);

            return builder.Create();
        }

        /// <summary>
        ///     Create an M68K Instruction.
        /// </summary>
        /// <param name="builder">
        ///     A builder to initialize the object with.
        /// </param>
        internal M68KInstruction(M68KInstructionBuilder builder) : base(builder) { }
    }
}