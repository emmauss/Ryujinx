namespace Gee.External.Capstone.Mips {
    /// <summary>
    ///     MIPS Instruction Detail Builder.
    /// </summary>
    internal sealed class MipsInstructionDetailBuilder : InstructionDetailBuilder<MipsInstructionDetail, MipsDisassembleMode, MipsInstructionGroup, MipsInstructionGroupId, MipsInstruction, MipsInstructionId, MipsRegister, MipsRegisterId> {
        /// <summary>
        ///     Get and Set Instruction's Operands.
        /// </summary>
        internal MipsOperand[] Operands { get; private set; }

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
            var nativeInstructionDetail = NativeCapstone.GetInstructionDetail<NativeMipsInstructionDetail>(hInstruction).GetValueOrDefault();

            this.Operands = MipsOperand.Create(disassembler, ref nativeInstructionDetail);
        }

        /// <summary>
        ///     Create a MIPS Instruction Detail.
        /// </summary>
        /// <returns>
        ///     A MIPS instruction detail.
        /// </returns>
        internal MipsInstructionDetail Create() {
            return new MipsInstructionDetail(this);
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
        private protected override MipsDisassembleMode CreateDisassembleMode(NativeDisassembleMode nativeDisassembleMode) {
            return (MipsDisassembleMode) nativeDisassembleMode;
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
        ///     An instruction group.
        /// </returns>
        private protected override MipsInstructionGroup CreateInstructionGroup(CapstoneDisassembler disassembler, byte instructionGroupId) {
            return MipsInstructionGroup.Create(disassembler, (MipsInstructionGroupId) instructionGroupId);
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
        ///     A register.
        /// </returns>
        private protected override MipsRegister CreateRegister(CapstoneDisassembler disassembler, short registerId) {
            return MipsRegister.TryCreate(disassembler, (MipsRegisterId) registerId);
        }
    }
}