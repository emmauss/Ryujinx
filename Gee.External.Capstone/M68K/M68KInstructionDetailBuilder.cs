namespace Gee.External.Capstone.M68K {
    /// <summary>
    ///     M68K Instruction Detail Builder.
    /// </summary>
    internal sealed class M68KInstructionDetailBuilder : InstructionDetailBuilder<M68KInstructionDetail, M68KDisassembleMode, M68KInstructionGroup, M68KInstructionGroupId, M68KInstruction, M68KInstructionId, M68KRegister, M68KRegisterId> {
        /// <summary>
        ///     Get and Set Instruction's Operands.
        /// </summary>
        internal M68KOperand[] Operands { get; private set; }

        /// <summary>
        ///     Get and Set Operation Size.
        /// </summary>
        internal M68KOperationSize OperationSize { get; private set; }

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
            var nativeInstructionDetail = NativeCapstone.GetInstructionDetail<NativeM68KInstructionDetail>(hInstruction).GetValueOrDefault();

            this.Operands = M68KOperand.Create(disassembler, ref nativeInstructionDetail);
            this.OperationSize = new M68KOperationSize(ref nativeInstructionDetail.OperationSize);
        }

        /// <summary>
        ///     Create an M68K Instruction Detail.
        /// </summary>
        /// <returns>
        ///     An M68K instruction detail.
        /// </returns>
        internal M68KInstructionDetail Create() {
            return new M68KInstructionDetail(this);
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
        private protected override M68KDisassembleMode CreateDisassembleMode(NativeDisassembleMode nativeDisassembleMode) {
            return (M68KDisassembleMode) nativeDisassembleMode;
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
        private protected override M68KInstructionGroup CreateInstructionGroup(CapstoneDisassembler disassembler, byte instructionGroupId) {
            return M68KInstructionGroup.Create(disassembler, (M68KInstructionGroupId) instructionGroupId);
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
        private protected override M68KRegister CreateRegister(CapstoneDisassembler disassembler, short registerId) {
            return M68KRegister.TryCreate(disassembler, (M68KRegisterId) registerId);
        }
    }
}