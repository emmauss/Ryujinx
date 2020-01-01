namespace Gee.External.Capstone.PowerPc {
    /// <summary>
    ///     PowerPC Instruction Detail.
    /// </summary>
    public sealed class PowerPcInstructionDetail : InstructionDetail<PowerPcInstructionDetail, PowerPcDisassembleMode, PowerPcInstructionGroup, PowerPcInstructionGroupId, PowerPcInstruction, PowerPcInstructionId, PowerPcRegister, PowerPcRegisterId> {
        /// <summary>
        ///     Get Branch Code.
        /// </summary>
        public PowerPcBranchCode BranchCode { get; }

        /// <summary>
        ///     Get Hint.
        /// </summary>
        public PowerPcBranchHint BranchHint { get; }

        /// <summary>
        ///     Get Instruction's Operands.
        /// </summary>
        public PowerPcOperand[] Operands { get; }

        /// <summary>
        ///     Get Update CR0 Flag.
        /// </summary>
        public bool UpdateCr0 { get; }

        /// <summary>
        ///     Create a PowerPC Instruction Detail.
        /// </summary>
        /// <param name="disassembler">
        ///     A disassembler.
        /// </param>
        /// <param name="hInstruction">
        ///     An instruction handle.
        /// </param>
        /// <returns>
        ///     A PowerPC instruction detail.
        /// </returns>
        internal static PowerPcInstructionDetail Create(CapstoneDisassembler disassembler, NativeInstructionHandle hInstruction) {
            var builder = new PowerPcInstructionDetailBuilder();
            builder.Build(disassembler, hInstruction);

            return builder.Create();
        }

        /// <summary>
        ///     Create a PowerPC Instruction Detail.
        /// </summary>
        /// <param name="builder">
        ///     A builder to initialize the object with.
        /// </param>
        internal PowerPcInstructionDetail(PowerPcInstructionDetailBuilder builder) : base(builder) {
            this.BranchCode = builder.BranchCode;
            this.BranchHint = builder.BranchHint;
            this.Operands = builder.Operands;
            this.UpdateCr0 = builder.UpdateCr0;
        }
    }
}