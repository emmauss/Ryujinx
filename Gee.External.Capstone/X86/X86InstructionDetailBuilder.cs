namespace Gee.External.Capstone.X86 {
    /// <summary>
    ///     X86 Instruction Detail Builder.
    /// </summary>
    internal sealed class X86InstructionDetailBuilder : InstructionDetailBuilder<X86InstructionDetail, X86DisassembleMode, X86InstructionGroup, X86InstructionGroupId, X86Instruction, X86InstructionId, X86Register, X86RegisterId> {
        /// <summary>
        ///     Get and Set Address Size.
        /// </summary>
        internal byte AddressSize { get; private set; }

        /// <summary>
        ///     Get and Set AVX Condition Code.
        /// </summary>
        internal X86AvxConditionCode AvxConditionCode { get; private set; }

        /// <summary>
        ///     Get and Set AVX Rounding Mode.
        /// </summary>
        internal X86AvxRoundingMode AvxRoundingMode { get; private set; }

        /// <summary>
        ///     Get and Set AVX Suppress All Exceptions Flag.
        /// </summary>
        internal bool AvxSuppressAllExceptions { get; private set; }

        /// <summary>
        ///     Get and Set Displacement Value.
        /// </summary>
        internal long Displacement { get; private set; }

        /// <summary>
        ///     Get and Set EFlags.
        /// </summary>
        internal long EFlags { get; private set; }

        /// <summary>
        ///     Get and Set Encoding.
        /// </summary>
        internal X86Encoding Encoding { get; private set; }

        /// <summary>
        ///     Get and Set FPU Flags.
        /// </summary>
        internal long FpuFlags { get; private set; }

        /// <summary>
        ///     Get and Set ModR/M.
        /// </summary>
        internal byte ModRm { get; private set; }

        /// <summary>
        ///     Get and Set Instruction's Opcode.
        /// </summary>
        internal byte[] Opcode { get; private set; }

        /// <summary>
        ///     Get and Set Instruction's Operands.
        /// </summary>
        internal X86Operand[] Operands { get; private set; }

        /// <summary>
        ///     Get and Set Instruction's Prefix.
        /// </summary>
        internal X86Prefix[] Prefix { get; private set; }

        /// <summary>
        ///     Get and Set REX Prefix.
        /// </summary>
        internal byte Rex { get; private set; }

        /// <summary>
        ///     Get and Set SIB Value.
        /// </summary>
        internal byte Sib { get; private set; }

        /// <summary>
        ///     Get and Set SIB Base.
        /// </summary>
        internal X86Register SibBase { get; private set; }

        /// <summary>
        ///     Get and Set SIB Index.
        /// </summary>
        internal X86Register SibIndex { get; private set; }

        /// <summary>
        ///     Get and Set SIB Scale.
        /// </summary>
        internal byte SibScale { get; private set; }

        /// <summary>
        ///     Get and Set SSE Condition Code.
        /// </summary>
        internal X86SseConditionCode SseConditionCode { get; private set; }

        /// <summary>
        ///     Get and Set XOP Condition Code.
        /// </summary>
        internal X86XopConditionCode XopConditionCode { get; private set; }

        /// <summary>
        ///     Build an Instruction Detail.
        /// </summary>
        /// <param name="disassembler">
        ///     A disassembler.
        /// </param>
        /// <param name="hInstruction">
        ///     An instruction handle.
        /// </param>
        internal override void Build(CapstoneDisassembler disassembler, NativeInstructionHandle hInstruction) {
            // ...
            //
            // Throws an exception if the operation fails.
            base.Build(disassembler, hInstruction);
            var nativeInstructionDetail = NativeCapstone.GetInstructionDetail<NativeX86InstructionDetail>(hInstruction).GetValueOrDefault();

            this.AddressSize = nativeInstructionDetail.AddressSize;
            this.AvxConditionCode = nativeInstructionDetail.AvxConditionCode;
            this.AvxRoundingMode = nativeInstructionDetail.AvxRoundingMode;
            this.AvxSuppressAllExceptions = nativeInstructionDetail.AvxSuppressAllExceptions;
            this.Displacement = nativeInstructionDetail.Displacement;
            this.EFlags = nativeInstructionDetail.Flag.EFlags;
            this.Encoding = new X86Encoding(ref nativeInstructionDetail.Encoding);
            this.FpuFlags = nativeInstructionDetail.Flag.FpuFlags;
            this.ModRm = nativeInstructionDetail.ModRm;
            this.Opcode = nativeInstructionDetail.Opcode;
            this.Operands = X86Operand.Create(disassembler, ref nativeInstructionDetail);
            this.Prefix = nativeInstructionDetail.Prefix;
            this.Rex = nativeInstructionDetail.Rex;
            this.Sib = nativeInstructionDetail.Sib;
            this.SibBase = X86Register.TryCreate(disassembler, nativeInstructionDetail.SibBase);
            this.SibIndex = X86Register.TryCreate(disassembler, nativeInstructionDetail.SibIndex);
            this.SibScale = nativeInstructionDetail.SibScale;
            this.SseConditionCode = nativeInstructionDetail.SseConditionCode;
            this.XopConditionCode = nativeInstructionDetail.XopConditionCode;
        }

        /// <summary>
        ///     Create an X86 Instruction Detail.
        /// </summary>
        /// <returns>
        ///     An X86 instruction detail.
        /// </returns>
        internal X86InstructionDetail Create() {
            return new X86InstructionDetail(this);
        }

        /// <summary>
        ///     Create Disassemble Mode.
        /// </summary>
        /// <param name="nativeDisassembleMode">
        ///     A native disassemble mode.
        /// </param>
        /// <returns>
        ///     A disassemble mode.
        /// </returns>
        private protected override X86DisassembleMode CreateDisassembleMode(NativeDisassembleMode nativeDisassembleMode) {
            return (X86DisassembleMode) nativeDisassembleMode;
        }

        /// <summary>
        ///     Create an Instruction Group.
        /// </summary>
        /// <param name="disassembler">
        ///     A disassembler.
        /// </param>
        /// <param name="instructionGroupId">
        ///     An instruction group's unique identifier.
        /// </param>
        /// <returns>
        ///     An X86 instruction group.
        /// </returns>
        private protected override X86InstructionGroup CreateInstructionGroup(CapstoneDisassembler disassembler, byte instructionGroupId) {
            return X86InstructionGroup.Create(disassembler, (X86InstructionGroupId) instructionGroupId);
        }

        /// <summary>
        ///     Create a Register.
        /// </summary>
        /// <param name="disassembler">
        ///     A disassembler.
        /// </param>
        /// <param name="registerId">
        ///     A register's unique identifier.
        /// </param>
        /// <returns>
        ///     An X86 register.
        /// </returns>
        private protected override X86Register CreateRegister(CapstoneDisassembler disassembler, short registerId) {
            return X86Register.TryCreate(disassembler, (X86RegisterId) registerId);
        }
    }
}