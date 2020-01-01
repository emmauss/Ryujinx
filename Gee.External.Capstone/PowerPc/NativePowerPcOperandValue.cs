using System.Runtime.InteropServices;

namespace Gee.External.Capstone.PowerPc {
    /// <summary>
    ///     Native PowerPC Operand Value.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    internal struct NativePowerPcOperandValue {
        /// <summary>
        ///     Register Value.
        /// </summary>
        [FieldOffset(0)]
        public PowerPcRegisterId Register;

        /// <summary>
        ///     Immediate Value.
        /// </summary>
        [FieldOffset(0)]
        public long Immediate;

        /// <summary>
        ///     Memory Value.
        /// </summary>
        [FieldOffset(0)]
        public NativePowerPcMemoryOperandValue Memory;

        /// <summary>
        ///     Condition Register Value.
        /// </summary>
        [FieldOffset(0)]
        public NativePowerPcConditionRegisterOperandValue ConditionRegister;
    }
}