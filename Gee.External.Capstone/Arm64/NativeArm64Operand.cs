using System.Runtime.InteropServices;

namespace Gee.External.Capstone.Arm64 {
    /// <summary>
    ///     Native ARM64 Operand.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeArm64Operand {
        /// <summary>
        ///     Vector Index.
        /// </summary>
        public int VectorIndex;

        /// <summary>
        ///     Vector Arrangement Specifier.
        /// </summary>
        public Arm64VectorArrangementSpecifier VectorArrangementSpecifier;

        /// <summary>
        ///     Vector Element Size Specifier.
        /// </summary>
        public Arm64VectorElementSizeSpecifier VectorElementSizeSpecifier;

        /// <summary>
        ///     Shift.
        /// </summary>
        public NativeArm64OperandShift Shift;

        /// <summary>
        ///     Extend Operation.
        /// </summary>
        public Arm64ExtendOperation ExtendOperation;

        /// <summary>
        ///     Operand's Type.
        /// </summary>
        public Arm64OperandType Type;

        /// <summary>
        ///     Operand's Value.
        /// </summary>
        public NativeArm64OperandValue Value;

        /// <summary>
        ///     Operand's Access Type.
        /// </summary>
        public OperandAccessType AccessType;
    }
}