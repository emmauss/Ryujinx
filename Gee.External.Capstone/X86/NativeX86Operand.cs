using System.Runtime.InteropServices;

namespace Gee.External.Capstone.X86 {
    /// <summary>
    ///     Native X86 Operand.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeX86Operand {
        /// <summary>
        ///     Operand's Type.
        /// </summary>
        public X86OperandType Type;

        /// <summary>
        ///     Operand's Value.
        /// </summary>
        public NativeX86OperandValue Value;

        /// <summary>
        ///     Operand's Size.
        /// </summary>
        public byte Size;

        /// <summary>
        ///     Operand's Access Type.
        /// </summary>
        [MarshalAs(UnmanagedType.I1)]
        public OperandAccessType AccessType;

        /// <summary>
        ///     AVX Broadcast.
        /// </summary>
        public X86AvxBroadcast AvxBroadcast;

        /// <summary>
        ///     AVX Zero Opmask.
        /// </summary>
        [MarshalAs(UnmanagedType.I1)]
        public bool AvxZeroOpMask;
    }
}