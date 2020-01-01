using System.Runtime.InteropServices;

namespace Gee.External.Capstone.Mips {
    /// <summary>
    ///     Native MIPS Operand Value.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    internal struct NativeMipsOperandValue {
        /// <summary>
        ///     Register Value.
        /// </summary>
        [FieldOffset(0)]
        public MipsRegisterId Register;

        /// <summary>
        ///     Immediate Value.
        /// </summary>
        [FieldOffset(0)]
        public long Immediate;

        /// <summary>
        ///     Memory Value.
        /// </summary>
        [FieldOffset(0)]
        public NativeMipsMemoryOperandValue Memory;
    }
}