namespace Gee.External.Capstone.X86 {
    /// <summary>
    ///     X86 Memory Operand Value.
    /// </summary>
    public sealed class X86MemoryOperandValue {
        /// <summary>
        ///     Get Base Register.
        /// </summary>
        public X86Register Base { get; }

        /// <summary>
        ///     Get Displacement Value.
        /// </summary>
        public long Displacement { get; }

        /// <summary>
        ///     Get Index Register.
        /// </summary>
        public X86Register Index { get; }

        /// <summary>
        ///     Get Index Register's Scale.
        /// </summary>
        public int Scale { get; }

        /// <summary>
        ///     Get Segment Register.
        /// </summary>
        public X86Register Segment { get; }

        /// <summary>
        ///     Create an X86 Memory Operand Value.
        /// </summary>
        /// <param name="disassembler">
        ///     A disassembler.
        /// </param>
        /// <param name="nativeMemoryOperandValue">
        ///     A native X86 memory operand value.
        /// </param>
        internal X86MemoryOperandValue(CapstoneDisassembler disassembler, ref NativeX86MemoryOperandValue nativeMemoryOperandValue) {
            this.Base = X86Register.TryCreate(disassembler, nativeMemoryOperandValue.Base);
            this.Displacement = nativeMemoryOperandValue.Displacement;
            this.Index = X86Register.TryCreate(disassembler, nativeMemoryOperandValue.Index);
            this.Scale = nativeMemoryOperandValue.Scale;
            this.Segment = X86Register.TryCreate(disassembler, nativeMemoryOperandValue.Segment);
        }
    }
}