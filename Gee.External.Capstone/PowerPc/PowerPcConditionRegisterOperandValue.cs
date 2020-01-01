namespace Gee.External.Capstone.PowerPc {
    /// <summary>
    ///     PowerPC Condition Register Operand Value.
    /// </summary>
    public sealed class PowerPcConditionRegisterOperandValue {
        /// <summary>
        ///     Get Branch Code.
        /// </summary>
        public PowerPcBranchCode BranchCode { get; }

        /// <summary>
        ///     Get Register.
        /// </summary>
        public PowerPcRegister Register { get; }

        /// <summary>
        ///     Get Scale.
        /// </summary>
        public int Scale { get; }

        /// <summary>
        ///     Create a PowerPC Condition Register Operand Value.
        /// </summary>
        /// <param name="disassembler">
        ///     A disassembler.
        /// </param>
        /// <param name="nativeConditionRegisterOperandValue">
        ///     A native PowerPC condition register operand value.
        /// </param>
        internal PowerPcConditionRegisterOperandValue(CapstoneDisassembler disassembler, ref NativePowerPcConditionRegisterOperandValue nativeConditionRegisterOperandValue) {
            this.BranchCode = nativeConditionRegisterOperandValue.BranchCode;
            this.Register = PowerPcRegister.TryCreate(disassembler, nativeConditionRegisterOperandValue.Register);
            this.Scale = nativeConditionRegisterOperandValue.Scale;
        }
    }
}