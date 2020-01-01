using System.Runtime.InteropServices;

namespace Gee.External.Capstone.M68K {
    /// <summary>
    ///     Native M68K Operand.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeM68KOperand {
        /// <summary>
        ///     Value.
        /// </summary>
        public NativeM68KOperandValue Value;

        /// <summary>
        ///     Memory Value.
        /// </summary>
        public NativeM68KMemoryOperandValue Memory;

        /// <summary>
        ///     Branch Displacement Value.
        /// </summary>
        public NativeM68KBranchDisplacementOperandValue BranchDisplacement;

        /// <summary>
        ///     Register Bits Value.
        /// </summary>
        public int RegisterBits;

        /// <summary>
        ///     Operand's Type.
        /// </summary>
        public M68KOperandType Type;

        /// <summary>
        ///     Address Mode.
        /// </summary>
        public M68KAddressMode AddressMode;
    }
}