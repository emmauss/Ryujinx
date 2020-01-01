namespace Gee.External.Capstone.Mips {
    /// <summary>
    ///     MIPS Memory Operand Value.
    /// </summary>
    public sealed class MipsMemoryOperandValue {
        /// <summary>
        ///     Get Base Register.
        /// </summary>
        public MipsRegister Base { get; }

        /// <summary>
        ///     Get Displacement.
        /// </summary>
        public long Displacement { get; }

        /// <summary>
        ///     Create a MIPS Memory Operand Value.
        /// </summary>
        /// <param name="disassembler">
        ///     A disassembler.
        /// </param>
        /// <param name="nativeMemoryOperandValue">
        ///     A native MIPS memory operand value.
        /// </param>
        internal MipsMemoryOperandValue(CapstoneDisassembler disassembler, ref NativeMipsMemoryOperandValue nativeMemoryOperandValue) {
            this.Base = MipsRegister.TryCreate(disassembler, nativeMemoryOperandValue.Base);
            this.Displacement = nativeMemoryOperandValue.Displacement;
        }
    }
}