using System.Runtime.InteropServices;

namespace Gee.External.Capstone.Mips {
    /// <summary>
    ///     Native MIPS Instruction Detail.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeMipsInstructionDetail {
        /// <summary>
        ///     Instruction's Operands' Count.
        /// </summary>
        public byte OperandCount;

        /// <summary>
        ///     Instruction's Operands.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public NativeMipsOperand[] Operands;
    }
}