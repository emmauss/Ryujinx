namespace Gee.External.Capstone.Arm {
    /// <summary>
    ///     ARM Operand Type.
    /// </summary>
    public enum ArmOperandType {
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
        ///     Indicates a floating point operand.
        /// </summary>
        FloatingPoint,

        /// <summary>
        ///     Indicates a CImmediate operand.
        /// </summary>
        CImmediate = 64,

        /// <summary>
        ///     Indicates a PImmediate operand.
        /// </summary>
        PImmediate,

        /// <summary>
        ///     Indicates a SETEND operation operand.
        /// </summary>
        SetEndOperation,

        /// <summary>
        ///     Indicates a system register operand.
        /// </summary>
        SystemRegister
    }
}