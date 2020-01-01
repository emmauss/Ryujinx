namespace Gee.External.Capstone.PowerPc {
    /// <summary>
    ///     PowerPC Operand Type.
    /// </summary>
    public enum PowerPcOperandType {
        /// <summary>
        ///     Indicates an invalid, or an uninitialized, operand type.
        /// </summary>
        Invalid = 0,

        /// <summary>
        ///     Indicates a register operand.
        /// </summary>
        Register,

        /// <summary>
        ///     Indicates an immediate operand.
        /// </summary>
        Immediate,

        /// <summary>
        ///     Indicates a memory operand.
        /// </summary>
        Memory,

        /// <summary>
        ///     Indicates a condition register operand.
        /// </summary>
        ConditionRegister = 64
    }
}