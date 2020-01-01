using System.Runtime.InteropServices;

namespace Gee.External.Capstone.Arm {
    /// <summary>
    ///     Native ARM Operand Shift.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeArmOperandShift {
        /// <summary>
        ///     Shift Operation.
        /// </summary>
        public ArmShiftOperation Operation;

        /// <summary>
        ///     Shift Value.
        /// </summary>
        public int Value;
    }
}