namespace Gee.External.Capstone.X86 {
    /// <summary>
    ///     X86 Encoding.
    /// </summary>
    public sealed class X86Encoding {
        /// <summary>
        ///     Get Displacement Offset.
        /// </summary>
        public byte DisplacementOffset { get; }

        /// <summary>
        ///     Get Displacement Size.
        /// </summary>
        public byte DisplacementSize { get; }

        /// <summary>
        ///     Get Immediate Offset.
        /// </summary>
        public byte ImmediateOffset { get; }

        /// <summary>
        ///     Get Immediate Size.
        /// </summary>
        public byte ImmediateSize { get; }

        /// <summary>
        ///     Get ModR/M Offset.
        /// </summary>
        public byte ModRmOffset { get; }

        /// <summary>
        ///     Create an X86 Encoding.
        /// </summary>
        /// <param name="nativeEncoding">
        ///     A native X86 encoding.
        /// </param>
        internal X86Encoding(ref NativeX86Encoding nativeEncoding) {
            this.DisplacementOffset = nativeEncoding.DisplacementOffset;
            this.DisplacementSize = nativeEncoding.DisplacementSize;
            this.ImmediateOffset = nativeEncoding.ImmediateOffset;
            this.ImmediateSize = nativeEncoding.ImmediateSize;
            this.ModRmOffset = nativeEncoding.ModRmOffset;
        }
    }
}