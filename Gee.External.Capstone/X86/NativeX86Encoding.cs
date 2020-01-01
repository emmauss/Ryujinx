using System.Runtime.InteropServices;

namespace Gee.External.Capstone.X86 {
    /// <summary>
    ///     Native X86 Encoding.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeX86Encoding {
        /// <summary>
        ///     ModR/M Offset.
        /// </summary>
        public byte ModRmOffset;

        /// <summary>
        ///     Displacement Offset.
        /// </summary>
        public byte DisplacementOffset;

        /// <summary>
        ///     Displacement Size.
        /// </summary>
        public byte DisplacementSize;

        /// <summary>
        ///     Immediate Offset.
        /// </summary>
        public byte ImmediateOffset;

        /// <summary>
        ///     Immediate Size.
        /// </summary>
        public byte ImmediateSize;
    }
}