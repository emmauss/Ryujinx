namespace Gee.External.Capstone.X86 {
    /// <summary>
    ///     X86 Instruction Detail.
    /// </summary>
    public sealed class X86InstructionDetail : InstructionDetail<X86InstructionDetail, X86DisassembleMode, X86InstructionGroup, X86InstructionGroupId, X86Instruction, X86InstructionId, X86Register, X86RegisterId> {
        /// <summary>
        ///     Get Address Size.
        /// </summary>
        public byte AddressSize { get; }

        /// <summary>
        ///     Get AVX Condition Code.
        /// </summary>
        public X86AvxConditionCode AvxConditionCode { get; }

        /// <summary>
        ///     Get AVX Rounding Mode.
        /// </summary>
        public X86AvxRoundingMode AvxRoundingMode { get; }

        /// <summary>
        ///     Get AVX Suppress All Exceptions Flag.
        /// </summary>
        public bool AvxSuppressAllExceptions { get; }

        /// <summary>
        ///     Get Displacement Value.
        /// </summary>
        public long Displacement { get; }

        /// <summary>
        ///     Get EFlags.
        /// </summary>
        public long EFlags { get; }

        /// <summary>
        ///     Get Encoding.
        /// </summary>
        public X86Encoding Encoding { get; }

        /// <summary>
        ///     Get FPU Flags.
        /// </summary>
        public long FpuFlags { get; }

        /// <summary>
        ///     Get ModR/M.
        /// </summary>
        public byte ModRm { get; }

        /// <summary>
        ///     Get Instruction's Opcode.
        /// </summary>
        public byte[] Opcode { get; }

        /// <summary>
        ///     Get Instruction's Operands.
        /// </summary>
        public X86Operand[] Operands { get; }

        /// <summary>
        ///     Get Instruction's Prefix.
        /// </summary>
        public X86Prefix[] Prefix { get; }

        /// <summary>
        ///     Get REX Prefix.
        /// </summary>
        public byte Rex { get; }

        /// <summary>
        ///     Get SIB Value.
        /// </summary>
        public byte Sib { get; }

        /// <summary>
        ///     Get SIB Base.
        /// </summary>
        public X86Register SibBase { get; }

        /// <summary>
        ///     Get SIB Index.
        /// </summary>
        public X86Register SibIndex { get; }

        /// <summary>
        ///     Get SIB Scale.
        /// </summary>
        public byte SibScale { get; }

        /// <summary>
        ///     Get SSE Condition Code.
        /// </summary>
        public X86SseConditionCode SseConditionCode { get; }

        /// <summary>
        ///     Get XOP Condition Code.
        /// </summary>
        public X86XopConditionCode XopConditionCode { get; }

        /// <summary>
        ///     Create an X86 Instruction Detail.
        /// </summary>
        /// <param name="disassembler">
        ///     A disassembler.
        /// </param>
        /// <param name="hInstruction">
        ///     An instruction handle.
        /// </param>
        /// <returns>
        ///     An X86 instruction detail.
        /// </returns>
        internal static X86InstructionDetail Create(CapstoneDisassembler disassembler, NativeInstructionHandle hInstruction) {
            var builder = new X86InstructionDetailBuilder();
            builder.Build(disassembler, hInstruction);

            return builder.Create();
        }

        /// <summary>
        ///     Create an X86 Instruction Detail.
        /// </summary>
        /// <param name="builder">
        ///     A builder to initialize the object with.
        /// </param>
        internal X86InstructionDetail(X86InstructionDetailBuilder builder) : base(builder) {
            this.AddressSize = builder.AddressSize;
            this.AvxConditionCode = builder.AvxConditionCode;
            this.AvxRoundingMode = builder.AvxRoundingMode;
            this.AvxSuppressAllExceptions = builder.AvxSuppressAllExceptions;
            this.Displacement = builder.Displacement;
            this.EFlags = builder.EFlags;
            this.Encoding = builder.Encoding;
            this.FpuFlags = builder.FpuFlags;
            this.ModRm = builder.ModRm;
            this.Opcode = builder.Opcode;
            this.Operands = builder.Operands;
            this.Prefix = builder.Prefix;
            this.Rex = builder.Rex;
            this.Sib = builder.Sib;
            this.SibBase = builder.SibBase;
            this.SibIndex = builder.SibIndex;
            this.SibScale = builder.SibScale;
            this.SseConditionCode = builder.SseConditionCode;
            this.XopConditionCode = builder.XopConditionCode;
        }
    }
}