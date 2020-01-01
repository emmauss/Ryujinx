using System.Runtime.InteropServices;

namespace Gee.External.Capstone.Arm64 {
    /// <summary>
    ///     Native ARM64 Operand Value.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    internal struct NativeArm64OperandValue {
        /// <summary>
        ///     Register.
        /// </summary>
        [FieldOffset(0)]
        public Arm64RegisterId Register;

        /// <summary>
        ///     Immediate Value.
        /// </summary>
        [FieldOffset(0)]
        public long Immediate;

        /// <summary>
        ///     Floating Point Value.
        /// </summary>
        [FieldOffset(0)]
        public double FloatingPoint;

        /// <summary>
        ///     Memory Value.
        /// </summary>
        [FieldOffset(0)]
        public NativeArm64MemoryOperandValue Memory;

        /// <summary>
        ///     Processor State (PSTATE) Field.
        /// </summary>
        [FieldOffset(0)]
        public Arm64PStateField PStateField;

        /// <summary>
        ///     System Operation.
        /// </summary>
        [FieldOffset(0)]
        public int SystemOperation;

        /// <summary>
        ///     Prefetch Operation.
        /// </summary>
        [FieldOffset(0)]
        public Arm64PrefetchOperation PrefetchOperation;

        /// <summary>
        ///     Barrier Operation.
        /// </summary>
        [FieldOffset(0)]
        public Arm64BarrierOperation BarrierOperation;
    }
}