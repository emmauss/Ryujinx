using System.Runtime.InteropServices;

namespace Gee.External.Capstone.Arm {
    /// <summary>
    ///     Native ARM Instruction Detail.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeArmInstructionDetail {
        /// <summary>
        ///     User Mode Flag.
        /// </summary>
        [MarshalAs(UnmanagedType.I1)]
        public bool IsUserMode;

        /// <summary>
        ///     Vector Size.
        /// </summary>
        public int VectorSize;

        /// <summary>
        ///     Vector Data Type.
        /// </summary>
        public ArmVectorDataType VectorDataType;

        /// <summary>
        ///     CPS Mode.
        /// </summary>
        public ArmCpsMode CpsMode;

        /// <summary>
        ///     CPS Flag.
        /// </summary>
        public ArmCpsFlag CpsFlag;

        /// <summary>
        ///     Condition Code.
        /// </summary>
        public ArmConditionCode ConditionCode;

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
        ///     Memory Barrier Operation Operation.
        /// </summary>
        public ArmMemoryBarrierOperation MemoryBarrierOperation;

        /// <summary>
        ///     Instruction's Operands' Count.
        /// </summary>
        public byte OperandCount;

        /// <summary>
        ///     Instruction's Operands.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 36)]
        public NativeArmOperand[] Operands;
    }
}