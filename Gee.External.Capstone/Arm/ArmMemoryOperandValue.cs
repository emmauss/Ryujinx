namespace Gee.External.Capstone.Arm {
    /// <summary>
    ///     ARM Memory Operand Value.
    /// </summary>
    public sealed class ArmMemoryOperandValue {
        /// <summary>
        ///     Get Base Register.
        /// </summary>
        public ArmRegister Base { get; }

        /// <summary>
        ///     Get Displacement Value.
        /// </summary>
        public int Displacement { get; }

        /// <summary>
        ///     Get Index Register.
        /// </summary>
        public ArmRegister Index { get; }

        /// <summary>
        ///     Get Index Register's Left Shift Value.
        /// </summary>
        public int LeftShit { get; }

        /// <summary>
        ///     Get Index Register's Scale.
        /// </summary>
        public int Scale { get; }

        /// <summary>
        ///     Create an ARM Memory Operand Value.
        /// </summary>
        /// <param name="disassembler">
        ///     A disassembler.
        /// </param>
        /// <param name="nativeMemoryOperandValue">
        ///     A native ARM memory operand value.
        /// </param>
        internal ArmMemoryOperandValue(CapstoneDisassembler disassembler, ref NativeArmMemoryOperandValue nativeMemoryOperandValue) {
            this.Base = ArmRegister.TryCreate(disassembler, nativeMemoryOperandValue.Base);
            this.Displacement = nativeMemoryOperandValue.Displacement;
            this.Index = ArmRegister.TryCreate(disassembler, nativeMemoryOperandValue.Index);
            this.LeftShit = nativeMemoryOperandValue.LeftShift;
            this.Scale = nativeMemoryOperandValue.Scale;
        }
    }
}