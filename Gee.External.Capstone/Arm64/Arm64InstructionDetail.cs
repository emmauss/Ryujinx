namespace Gee.External.Capstone.Arm64 {
    /// <summary>
    ///     ARM64 Instruction Detail.
    /// </summary>
    public sealed class Arm64InstructionDetail : InstructionDetail<Arm64InstructionDetail, Arm64DisassembleMode, Arm64InstructionGroup, Arm64InstructionGroupId, Arm64Instruction, Arm64InstructionId, Arm64Register, Arm64RegisterId> {
        /// <summary>
        ///     Get Condition Code.
        /// </summary>
        public Arm64ConditionCode ConditionCode { get; }

        /// <summary>
        ///     Get Instruction's Operands.
        /// </summary>
        public Arm64Operand[] Operands { get; }

        /// <summary>
        ///     Get Update Flags Flag.
        /// </summary>
        public bool UpdateFlags { get; }

        /// <summary>
        ///     Get Write Back Flag.
        /// </summary>
        public bool WriteBack { get; }

        /// <summary>
        ///     Create an ARM64 Instruction Detail.
        /// </summary>
        /// <param name="disassembler">
        ///     A disassembler.
        /// </param>
        /// <param name="hInstruction">
        ///     An instruction handle.
        /// </param>
        /// <returns>
        ///     An ARM64 instruction detail.
        /// </returns>
        internal static Arm64InstructionDetail Create(CapstoneDisassembler disassembler, NativeInstructionHandle hInstruction) {
            var builder = new Arm64InstructionDetailBuilder();
            builder.Build(disassembler, hInstruction);

            return builder.Create();
        }

        /// <summary>
        ///     Create an ARM64 Instruction Detail.
        /// </summary>
        /// <param name="builder">
        ///     A builder to initialize the object with.
        /// </param>
        internal Arm64InstructionDetail(Arm64InstructionDetailBuilder builder) : base(builder) {
            this.ConditionCode = builder.ConditionCode;
            this.Operands = builder.Operands;
            this.UpdateFlags = builder.UpdateFlags;
            this.WriteBack = builder.WriteBack;
        }
    }
}