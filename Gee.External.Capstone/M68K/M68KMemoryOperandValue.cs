namespace Gee.External.Capstone.M68K {
    /// <summary>
    ///     M68K Memory Operand Value.
    /// </summary>
    public sealed class M68KMemoryOperandValue {
        /// <summary>
        ///     Get Base Register.
        /// </summary>
        public M68KRegister Base { get; }

        /// <summary>
        ///     Get Bit Field.
        /// </summary>
        public byte BitField { get; }

        /// <summary>
        ///     Get Displacement.
        /// </summary>
        public short Displacement { get; }

        /// <summary>
        ///     Get Index Register.
        /// </summary>
        public M68KRegister Index { get; }

        /// <summary>
        ///     Get Index Size.
        /// </summary>
        public byte IndexSize { get; }

        /// <summary>
        ///     Get Indirect Base Register.
        /// </summary>
        public M68KRegister IndirectBase { get; }

        /// <summary>
        ///     Get Indirect Displacement.
        /// </summary>
        public int IndirectDisplacement { get; }

        /// <summary>
        ///     Get Bit Field Offset.
        /// </summary>
        public byte Offset { get; }

        /// <summary>
        ///     Get Other Displacement.
        /// </summary>
        public int OutDisplacement { get; }

        /// <summary>
        ///     Get Index Register's Scale.
        /// </summary>
        public byte Scale { get; }

        /// <summary>
        ///     Get Bit Field Width.
        /// </summary>
        public byte Width { get; }

        /// <summary>
        ///     Create an M68K Memory Operand Value.
        /// </summary>
        /// <param name="disassembler">
        ///     A disassembler.
        /// </param>
        /// <param name="nativeMemoryOperandValue">
        ///     A native M68K memory operand value.
        /// </param>
        internal M68KMemoryOperandValue(CapstoneDisassembler disassembler, ref NativeM68KMemoryOperandValue nativeMemoryOperandValue) {
            this.Base = M68KRegister.TryCreate(disassembler, nativeMemoryOperandValue.Base);
            this.BitField = nativeMemoryOperandValue.BitField;
            this.Displacement = nativeMemoryOperandValue.Displacement;
            this.Index = M68KRegister.TryCreate(disassembler, nativeMemoryOperandValue.Index);
            this.IndexSize = nativeMemoryOperandValue.IndexSize;
            this.IndirectBase = M68KRegister.TryCreate(disassembler, nativeMemoryOperandValue.IndirectBase);
            this.IndirectDisplacement = nativeMemoryOperandValue.IndirectDisplacement;
            this.Offset = nativeMemoryOperandValue.Offset;
            this.OutDisplacement = nativeMemoryOperandValue.OutDisplacement;
            this.Scale = nativeMemoryOperandValue.Scale;
            this.Width = nativeMemoryOperandValue.Width;
        }
    }
}