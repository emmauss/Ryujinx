namespace Gee.External.Capstone.Arm64 {
    /// <summary>
    ///     ARM64 Operand Type.
    /// </summary>
    public enum Arm64OperandType {
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
        ///     Indicates a MRS system register operand.
        /// </summary>
        MrsSystemRegister,

        /// <summary>
        ///     Indicates a MSR system register operand.
        /// </summary>
        MsrSystemRegister,

        /// <summary>
        ///     Indicates a Processor State (PSTATE) field operand.
        /// </summary>
        PStateField,

        /// <summary>
        ///     Indicates a system operation operand.
        /// </summary>
        SystemOperation,

        /// <summary>
        ///     Indicates a prefetch operation operand.
        /// </summary>
        PrefetchOperation,

        /// <summary>
        ///     Indicates a barrier operation operand.
        /// </summary>
        BarrierOperation,

        /// <summary>
        ///     Indicates an Address Translation (AT) operation.
        /// </summary>
        AtOperation = int.MaxValue * -1,

        /// <summary>
        ///     Indicates a Data Cache (DC) operation.
        /// </summary>
        DcOperation = (int.MaxValue - 1) * -1,

        /// <summary>
        ///     Indicates an Instruction Cache (IC) operation.
        /// </summary>
        IcOperation = (int.MaxValue - 2) * -1,

        /// <summary>
        ///     Indicates a Translation Lookaside Buffer (TLBI) operation.
        /// </summary>
        TlbiOperation = (int.MaxValue - 3) * -1
    }
}