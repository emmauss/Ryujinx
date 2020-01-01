namespace Gee.External.Capstone.PowerPc {
    /// <summary>
    ///     PowerPC Memory Operand Value.
    /// </summary>
    public sealed class PowerPcMemoryOperandValue {
        /// <summary>
        ///     Get Base Register.
        /// </summary>
        public PowerPcRegister Base { get; }

        /// <summary>
        ///     Get Displacement Value.
        /// </summary>
        public int Displacement { get; }

        /// <summary>
        ///     Create a PowerPC Memory Operand Value.
        /// </summary>
        /// <param name="disassembler">
        ///     A disassembler.
        /// </param>
        /// <param name="nativeMemoryOperandValue">
        ///     A native PowerPC memory operand value.
        /// </param>
        internal PowerPcMemoryOperandValue(CapstoneDisassembler disassembler, ref NativePowerPcMemoryOperandValue nativeMemoryOperandValue) {
            this.Base = PowerPcRegister.TryCreate(disassembler, nativeMemoryOperandValue.Base);
            this.Displacement = nativeMemoryOperandValue.Displacement;
        }
    }
}