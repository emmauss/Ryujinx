using System.Runtime.InteropServices;

namespace Gee.External.Capstone.X86 {
    /// <summary>
    ///     Native X86 Instruction Detail.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeX86InstructionDetail {
        /// <summary>
        ///     Instruction's Prefix.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.I1)]
        public X86Prefix[] Prefix;

        /// <summary>
        ///     Instruction's Opcode.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Opcode;

        /// <summary>
        ///     REX Prefix.
        /// </summary>
        public byte Rex;

        /// <summary>
        ///     Address Size.
        /// </summary>
        public byte AddressSize;

        /// <summary>
        ///     ModR/M.
        /// </summary>
        public byte ModRm;

        /// <summary>
        ///     SIB Value.
        /// </summary>
        public byte Sib;

        /// <summary>
        ///     Displacement Value.
        /// </summary>
        public long Displacement;

        /// <summary>
        ///     SIB Index.
        /// </summary>
        public X86RegisterId SibIndex;

        /// <summary>
        ///     SIB Scale.
        /// </summary>
        public byte SibScale;

        /// <summary>
        ///     SIB Base.
        /// </summary>
        public X86RegisterId SibBase;

        /// <summary>
        ///     XOP Condition Code.
        /// </summary>
        public X86XopConditionCode XopConditionCode;

        /// <summary>
        ///     SSE Condition Code.
        /// </summary>
        public X86SseConditionCode SseConditionCode;

        /// <summary>
        ///     AVX Condition Code.
        /// </summary>
        public X86AvxConditionCode AvxConditionCode;

        /// <summary>
        ///     AVX Suppress All Exceptions Flag.
        /// </summary>
        [MarshalAs(UnmanagedType.I1)]
        public bool AvxSuppressAllExceptions;

        /// <summary>
        ///     AVX Rounding Mode.
        /// </summary>
        public X86AvxRoundingMode AvxRoundingMode;

        /// <summary>
        ///     Flag.
        /// </summary>
        public NativeX86Flag Flag;

        /// <summary>
        ///     Instruction's Operands' Count.
        /// </summary>
        public byte OperandCount;

        /// <summary>
        ///     Instruction's Operands.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public NativeX86Operand[] Operands;

        /// <summary>
        ///     Encoding.
        /// </summary>
        public NativeX86Encoding Encoding;
    }
}