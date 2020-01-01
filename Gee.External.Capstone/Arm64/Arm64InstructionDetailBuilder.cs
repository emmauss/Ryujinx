namespace Gee.External.Capstone.Arm64 {
    /// <summary>
    ///     ARM64 Instruction Detail Builder.
    /// </summary>
    internal sealed class Arm64InstructionDetailBuilder : InstructionDetailBuilder<Arm64InstructionDetail, Arm64DisassembleMode, Arm64InstructionGroup, Arm64InstructionGroupId, Arm64Instruction, Arm64InstructionId, Arm64Register, Arm64RegisterId> {
        /// <summary>
        ///     Get and Set Condition Code.
        /// </summary>
        internal Arm64ConditionCode ConditionCode { get; private set; }

        /// <summary>
        ///     Get and Set Instruction's Operands.
        /// </summary>
        internal Arm64Operand[] Operands { get; private set; }

        /// <summary>
        ///     Get and Set Update Flags Flag.
        /// </summary>
        internal bool UpdateFlags { get; private set; }

        /// <summary>
        ///     Get and Set Write Back Flag.
        /// </summary>
        internal bool WriteBack { get; private set; }

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
            var nativeInstruction = NativeCapstone.GetInstruction(hInstruction);
            var nativeInstructionDetail = NativeCapstone.GetInstructionDetail<NativeArm64InstructionDetail>(hInstruction).GetValueOrDefault();

            this.ConditionCode = nativeInstructionDetail.ConditionCode;
            this.Operands = Arm64Operand.Create(disassembler, (Arm64InstructionId) nativeInstruction.Id, ref nativeInstructionDetail);
            this.UpdateFlags = nativeInstructionDetail.UpdateFlags;
            this.WriteBack = nativeInstructionDetail.WriteBack;
        }

        /// <summary>
        ///     Create an ARM64 Instruction Detail.
        /// </summary>
        /// <returns>
        ///     An ARM64 instruction detail.
        /// </returns>
        internal Arm64InstructionDetail Create() {
            return new Arm64InstructionDetail(this);
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
        private protected override Arm64DisassembleMode CreateDisassembleMode(NativeDisassembleMode nativeDisassembleMode) {
            return (Arm64DisassembleMode) nativeDisassembleMode;
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
        ///     An ARM64 instruction group.
        /// </returns>
        private protected override Arm64InstructionGroup CreateInstructionGroup(CapstoneDisassembler disassembler, byte instructionGroupId) {
            return Arm64InstructionGroup.Create(disassembler, (Arm64InstructionGroupId) instructionGroupId);
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
        ///     An ARM64 register.
        /// </returns>
        private protected override Arm64Register CreateRegister(CapstoneDisassembler disassembler, short registerId) {
            return Arm64Register.TryCreate(disassembler, (Arm64RegisterId) registerId);
        }
    }
}