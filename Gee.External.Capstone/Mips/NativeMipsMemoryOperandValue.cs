using System.Runtime.InteropServices;

namespace Gee.External.Capstone.Mips {
    /// <summary>
    ///     Native MIPS Memory Operand Value.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeMipsMemoryOperandValue {
        /// <summary>
        ///     Base Register.
        /// </summary>
        public MipsRegisterId Base;

        /// <summary>
        ///     Displacement.
        /// </summary>
        public long Displacement;
    }
}