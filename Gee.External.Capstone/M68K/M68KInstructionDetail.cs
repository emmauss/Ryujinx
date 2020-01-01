namespace Gee.External.Capstone.M68K {
    /// <summary>
    ///     M68K Instruction Detail.
    /// </summary>
    public sealed class M68KInstructionDetail : InstructionDetail<M68KInstructionDetail, M68KDisassembleMode, M68KInstructionGroup, M68KInstructionGroupId, M68KInstruction, M68KInstructionId, M68KRegister, M68KRegisterId> {
        /// <summary>
        ///     Get Instruction's Operands.
        /// </summary>
        public M68KOperand[] Operands { get; }

        /// <summary>
        ///     Get Operation Size.
        /// </summary>
        public M68KOperationSize OperationSize { get; }

        /// <summary>
        ///     Create an M68K Instruction Detail.
        /// </summary>
        /// <param name="disassembler">
        ///     A disassembler.
        /// </param>
        /// <param name="hInstruction">
        ///     An instruction handle.
        /// </param>
        /// <returns>
        ///     An M68K instruction detail.
        /// </returns>
        internal static M68KInstructionDetail Create(CapstoneDisassembler disassembler, NativeInstructionHandle hInstruction) {
            var builder = new M68KInstructionDetailBuilder();
            builder.Build(disassembler, hInstruction);

            return builder.Create();
        }

        /// <summary>
        ///     Create an M68K Instruction Detail.
        /// </summary>
        /// <param name="builder">
        ///     A builder to initialize the object with.
        /// </param>
        internal M68KInstructionDetail(M68KInstructionDetailBuilder builder) : base(builder) {
            this.Operands = builder.Operands;
            this.OperationSize = builder.OperationSize;
        }
    }
}