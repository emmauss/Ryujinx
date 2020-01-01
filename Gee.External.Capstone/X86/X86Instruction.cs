namespace Gee.External.Capstone.X86 {
    /// <summary>
    ///     X86 Instruction.
    /// </summary>
    public sealed class X86Instruction : Instruction<X86Instruction, X86InstructionDetail, X86DisassembleMode, X86InstructionGroup, X86InstructionGroupId, X86InstructionId, X86Register, X86RegisterId> {
        /// <summary>
        ///     Create an X86 Instruction.
        /// </summary>
        /// <param name="disassembler">
        ///     A disassembler.
        /// </param>
        /// <param name="hInstruction">
        ///     An instruction handle.
        /// </param>
        /// <returns>
        ///     An X86 instruction.
        /// </returns>
        internal static X86Instruction Create(CapstoneDisassembler disassembler, NativeInstructionHandle hInstruction) {
            var builder = new X86InstructionBuilder();
            builder.Build(disassembler, hInstruction);

            return builder.Create();
        }

        /// <summary>
        ///     Create an X86 Instruction.
        /// </summary>
        /// <param name="builder">
        ///     A builder to initialize the object with.
        /// </param>
        internal X86Instruction(X86InstructionBuilder builder) : base(builder) { }
    }
}