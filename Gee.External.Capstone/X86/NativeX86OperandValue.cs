using System.Runtime.InteropServices;

namespace Gee.External.Capstone.X86 {
    /// <summary>
    ///     Native X86 Operand Value.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    internal struct NativeX86OperandValue {
        /// <summary>
        ///     Register Value.
        /// </summary>
        [FieldOffset(0)]
        public X86RegisterId Register;

        /// <summary>
        ///     Immediate Value.
        /// </summary>
        [FieldOffset(0)]
        public long Immediate;

        /// <summary>
        ///     Memory Value.
        /// </summary>
        [FieldOffset(0)]
        public NativeX86MemoryOperandValue Memory;
    }
}