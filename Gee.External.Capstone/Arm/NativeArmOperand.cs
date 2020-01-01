using System.Runtime.InteropServices;

namespace Gee.External.Capstone.Arm {
    /// <summary>
    ///     Native ARM Operand.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeArmOperand {
        /// <summary>
        ///     Vector Index.
        /// </summary>
        public int VectorIndex;

        /// <summary>
        ///     Shift.
        /// </summary>
        public NativeArmOperandShift Shift;

        /// <summary>
        ///     Operand's Type.
        /// </summary>
        public ArmOperandType Type;

        /// <summary>
        ///     Operand's Value.
        /// </summary>
        public NativeArmOperandValue Value;

        /// <summary>
        ///     Operand's Subtracted Flag.
        /// </summary>
        [MarshalAs(UnmanagedType.I1)]
        public bool IsSubtracted;

        /// <summary>
        ///     Operand's Access Type.
        /// </summary>
        [MarshalAs(UnmanagedType.I1)]
        public OperandAccessType AccessType;

        /// <summary>
        ///     Neon Lane Value.
        /// </summary>
        public sbyte NeonLane;
    }
}