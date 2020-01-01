using System.Runtime.InteropServices;

namespace Gee.External.Capstone.Arm64 {
    /// <summary>
    ///     Native ARM64 Memory Operand Value.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeArm64MemoryOperandValue {
        /// <summary>
        ///     Base Register.
        /// </summary>
        public Arm64RegisterId Base;

        /// <summary>
        ///     Index Register.
        /// </summary>
        public Arm64RegisterId Index;

        /// <summary>
        ///     Displacement Value.
        /// </summary>
        public int Displacement;
    }
}