namespace Gee.External.Capstone.M68K {
    /// <summary>
    ///     M68K Branch Displacement Operand Value.
    /// </summary>
    public sealed class M68KBranchDisplacementOperandValue {
        /// <summary>
        ///     Get Displacement Value.
        /// </summary>
        public int Displacement { get; }

        /// <summary>
        ///     Get Displacement Size.
        /// </summary>
        public M68KBranchDisplacementSize DisplacementSize { get; }

        /// <summary>
        ///     Create an M68K Branch Displacement Operand Value.
        /// </summary>
        /// <param name="nativeBranchDisplacementOperandValue">
        ///     A native M68K branch displacement operand value.
        /// </param>
        internal M68KBranchDisplacementOperandValue(ref NativeM68KBranchDisplacementOperandValue nativeBranchDisplacementOperandValue) {
            this.Displacement = nativeBranchDisplacementOperandValue.Displacement;
            this.DisplacementSize = nativeBranchDisplacementOperandValue.DisplacementSize;
        }
    }
}