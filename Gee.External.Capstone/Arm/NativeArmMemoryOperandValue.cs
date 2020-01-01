using System.Runtime.InteropServices;

namespace Gee.External.Capstone.Arm {
    /// <summary>
    ///     Native ARM Memory Operand Value.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeArmMemoryOperandValue {
        /// <summary>
        ///     Base Register.
        /// </summary>
        public ArmRegisterId Base;

        /// <summary>
        ///     Index Register.
        /// </summary>
        public ArmRegisterId Index;

        /// <summary>
        ///     Index Register's Scale.
        /// </summary>
        public int Scale;

        /// <summary>
        ///     Displacement Value.
        /// </summary>
        public int Displacement;

        /// <summary>
        ///     Index Register's Left Shift Value.
        /// </summary>
        public int LeftShift;
    }
}