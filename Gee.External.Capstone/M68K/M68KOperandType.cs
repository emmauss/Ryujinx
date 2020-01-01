namespace Gee.External.Capstone.M68K {
    /// <summary>
    ///     M68K Operand Type.
    /// </summary>
    public enum M68KOperandType {
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
        ///     Indicates a single precision floating point operand.
        /// </summary>
        SImmediate,

        /// <summary>
        ///     Indicates a double precision floating point operand.
        /// </summary>
        DImmediate,

        /// <summary>
        ///     Indicates a register bits operand.
        /// </summary>
        RegisterBits,

        /// <summary>
        ///     Indicates a register pair operand.
        /// </summary>
        RegisterPair,

        /// <summary>
        ///     Indicates a branch displacement operand.
        /// </summary>
        BranchDisplacement
    }
}