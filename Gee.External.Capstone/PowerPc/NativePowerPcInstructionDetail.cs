using System.Runtime.InteropServices;

namespace Gee.External.Capstone.PowerPc {
    /// <summary>
    ///     Native PowerPC Instruction Detail.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct NativePowerPcInstructionDetail {
        /// <summary>
        ///     Branch Code.
        /// </summary>
        public PowerPcBranchCode BranchCode;

        /// <summary>
        ///     Branch Hint.
        /// </summary>
        public PowerPcBranchHint BranchHint;

        /// <summary>
        ///     Update CR0 Flag.
        /// </summary>
        [MarshalAs(UnmanagedType.I1)]
        public bool UpdateCr0;

        /// <summary>
        ///     Instruction's Operands' Count.
        /// </summary>
        public byte OperandCount;

        /// <summary>
        ///     Instruction's Operands.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public NativePowerPcOperand[] Operands;
    }
}