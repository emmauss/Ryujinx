using System.Runtime.InteropServices;

namespace Gee.External.Capstone.Mips {
    /// <summary>
    ///     Native Mips Operand.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeMipsOperand {
        /// <summary>
        ///     Operand's Type.
        /// </summary>
        public MipsOperandType Type;

        /// <summary>
        ///     Operand's Value.
        /// </summary>
        public NativeMipsOperandValue Value;
    }
}