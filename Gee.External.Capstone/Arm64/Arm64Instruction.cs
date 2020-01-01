namespace Gee.External.Capstone.Arm64 {
    /// <summary>
    ///     ARM64 Instruction.
    /// </summary>
    public sealed class Arm64Instruction : Instruction<Arm64Instruction, Arm64InstructionDetail, Arm64DisassembleMode, Arm64InstructionGroup, Arm64InstructionGroupId, Arm64InstructionId, Arm64Register, Arm64RegisterId> {
        /// <summary>
        ///     Create an ARM64 Instruction.
        /// </summary>
        /// <param name="disassembler">
        ///     A disassembler.
        /// </param>
        /// <param name="hInstruction">
        ///     An instruction handle.
        /// </param>
        /// <returns>
        ///     An ARM64 instruction.
        /// </returns>
        internal static Arm64Instruction Create(CapstoneDisassembler disassembler, NativeInstructionHandle hInstruction) {
            var builder = new Arm64InstructionBuilder();
            builder.Build(disassembler, hInstruction);

            return builder.Create();
        }

        /// <summary>
        ///     Create an ARM64 Instruction.
        /// </summary>
        /// <param name="builder">
        ///     A builder to initialize the object with.
        /// </param>
        internal Arm64Instruction(Arm64InstructionBuilder builder) : base(builder) { }
    }
}