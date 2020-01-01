using System.Runtime.InteropServices;

namespace Gee.External.Capstone.M68K {
    /// <summary>
    ///     Native M68K Instruction Detail.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeM68KInstructionDetail {
        /// <summary>
        ///     Instruction's Operands.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public NativeM68KOperand[] Operands;

        /// <summary>
        ///     Operation's Size.
        /// </summary>
        public NativeM68KOperationSize OperationSize;

        /// <summary>
        ///     Instruction's Operands' Count.
        /// </summary>
        public byte OperandCount;
    }
}