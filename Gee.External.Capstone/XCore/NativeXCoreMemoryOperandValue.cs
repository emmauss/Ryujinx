using System.Runtime.InteropServices;

namespace Gee.External.Capstone.XCore {
    /// <summary>
    ///     Native XCore Memory Operand Value.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeXCoreMemoryOperandValue {
        /// <summary>
        ///     Base Register.
        /// </summary>
        public byte Base;

        /// <summary>
        ///     Index Register.
        /// </summary>
        public byte Index;

        /// <summary>
        ///     Displacement Value.
        /// </summary>
        public int Displacement;

        /// <summary>
        ///     Direct Value.
        /// </summary>
        public int Direct;
    }
}