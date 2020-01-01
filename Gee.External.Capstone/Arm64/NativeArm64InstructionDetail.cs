using System.Runtime.InteropServices;

namespace Gee.External.Capstone.Arm64 {
    /// <summary>
    ///     Native ARM64 Instruction Detail.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeArm64InstructionDetail {
        /// <summary>
        ///     Condition Code.
        /// </summary>
        public Arm64ConditionCode ConditionCode;

        /// <summary>
        ///     Update Flags Flag.
        /// </summary>
        [MarshalAs(UnmanagedType.I1)]
        public bool UpdateFlags;

        /// <summary>
        ///     Write Back Flag.
        /// </summary>
        [MarshalAs(UnmanagedType.I1)]
        public bool WriteBack;

        /// <summary>
        ///     Instruction's Operand Count.
        /// </summary>
        public byte OperandCount;

        /// <summary>
        ///     Instruction's Operands.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public NativeArm64Operand[] Operands;
    }
}