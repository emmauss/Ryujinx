using System.Runtime.InteropServices;

namespace Gee.External.Capstone.M68K {
    /// <summary>
    ///     Native M68K Branch Displacement Operand Value.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeM68KBranchDisplacementOperandValue {
        /// <summary>
        ///     Displacement Value.
        /// </summary>
        public int Displacement;

        /// <summary>
        ///     Displacement Size.
        /// </summary>
        [MarshalAs(UnmanagedType.I1)]
        public M68KBranchDisplacementSize DisplacementSize;
    }
}