namespace Gee.External.Capstone.Mips {
    /// <summary>
    ///     MIPS Operand Type.
    /// </summary>
    public enum MipsOperandType {
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
        Memory
    }
}