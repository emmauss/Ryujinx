namespace Gee.External.Capstone.Mips {
    /// <summary>
    ///     MIPS Instruction Detail.
    /// </summary>
    public sealed class MipsInstructionDetail : InstructionDetail<MipsInstructionDetail, MipsDisassembleMode, MipsInstructionGroup, MipsInstructionGroupId, MipsInstruction, MipsInstructionId, MipsRegister, MipsRegisterId> {
        /// <summary>
        ///     Get Instruction's Operands.
        /// </summary>
        public MipsOperand[] Operands { get; }

        /// <summary>
        ///     Create a MIPS Instruction Detail.
        /// </summary>
        /// <param name="disassembler">
        ///     A disassembler.
        /// </param>
        /// <param name="hInstruction">
        ///     An instruction handle.
        /// </param>
        /// <returns>
        ///     A MIPS instruction detail.
        /// </returns>
        internal static MipsInstructionDetail Create(CapstoneDisassembler disassembler, NativeInstructionHandle hInstruction) {
            var builder = new MipsInstructionDetailBuilder();
            builder.Build(disassembler, hInstruction);

            return builder.Create();
        }

        /// <summary>
        ///     Create a MIPS Instruction Detail.
        /// </summary>
        /// <param name="builder">
        ///     A builder to initialize the object with.
        /// </param>
        internal MipsInstructionDetail(MipsInstructionDetailBuilder builder) : base(builder) {
            this.Operands = builder.Operands;
        }
    }
}