namespace Gee.External.Capstone.PowerPc {
    /// <summary>
    ///     PowerPC Instruction Detail Builder.
    /// </summary>
    internal sealed class PowerPcInstructionDetailBuilder : InstructionDetailBuilder<PowerPcInstructionDetail, PowerPcDisassembleMode, PowerPcInstructionGroup, PowerPcInstructionGroupId, PowerPcInstruction, PowerPcInstructionId, PowerPcRegister, PowerPcRegisterId> {
        /// <summary>
        ///     Get and Set Branch Code.
        /// </summary>
        internal PowerPcBranchCode BranchCode { get; private set; }

        /// <summary>
        ///     Get and Set Branch Hint.
        /// </summary>
        internal PowerPcBranchHint BranchHint { get; private set; }

        /// <summary>
        ///     Get and Set Instruction's Operands.
        /// </summary>
        internal PowerPcOperand[] Operands { get; private set; }

        /// <summary>
        ///     Get and Set Update CR0 Flag.
        /// </summary>
        internal bool UpdateCr0 { get; private set; }

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
            var nativeInstructionDetail = NativeCapstone.GetInstructionDetail<NativePowerPcInstructionDetail>(hInstruction).GetValueOrDefault();

            this.BranchCode = nativeInstructionDetail.BranchCode;
            this.BranchHint = nativeInstructionDetail.BranchHint;
            this.Operands = PowerPcOperand.Create(disassembler, ref nativeInstructionDetail);
            this.UpdateCr0 = nativeInstructionDetail.UpdateCr0;
        }

        /// <summary>
        ///     Create a PowerPC Instruction Detail.
        /// </summary>
        /// <returns>
        ///     A PowerPC instruction detail.
        /// </returns>
        internal PowerPcInstructionDetail Create() {
            return new PowerPcInstructionDetail(this);
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
        private protected override PowerPcDisassembleMode CreateDisassembleMode(NativeDisassembleMode nativeDisassembleMode) {
            return (PowerPcDisassembleMode) nativeDisassembleMode;
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
        ///     A PowerPC instruction group.
        /// </returns>
        private protected override PowerPcInstructionGroup CreateInstructionGroup(CapstoneDisassembler disassembler, byte instructionGroupId) {
            return PowerPcInstructionGroup.Create(disassembler, (PowerPcInstructionGroupId) instructionGroupId);
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
        ///     A PowerPC register.
        /// </returns>
        private protected override PowerPcRegister CreateRegister(CapstoneDisassembler disassembler, short registerId) {
            return PowerPcRegister.TryCreate(disassembler, (PowerPcRegisterId) registerId);
        }
    }
}