using System.Runtime.InteropServices;

namespace Gee.External.Capstone.X86 {
    /// <summary>
    ///     Native X86 Memory Operand Value.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeX86MemoryOperandValue {
        /// <summary>
        ///     Segment Register.
        /// </summary>
        public X86RegisterId Segment;

        /// <summary>
        ///     Base Register.
        /// </summary>
        public X86RegisterId Base;

        /// <summary>
        ///     Index Register.
        /// </summary>
        public X86RegisterId Index;

        /// <summary>
        ///     Index Register's Scale.
        /// </summary>
        public int Scale;

        /// <summary>
        ///     Displacement Value.
        /// </summary>
        public long Displacement;
    }
}