using System.Runtime.InteropServices;

namespace Gee.External.Capstone.Arm64 {
    /// <summary>
    ///     Native ARM64 Operand Shift.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeArm64OperandShift {
        /// <summary>
        ///     Shift Operation.
        /// </summary>
        public Arm64ShiftOperation Operation;

        /// <summary>
        ///     Shift Value.
        /// </summary>
        public int Value;
    }
}