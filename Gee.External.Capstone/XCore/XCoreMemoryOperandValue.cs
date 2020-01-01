namespace Gee.External.Capstone.XCore {
    /// <summary>
    ///     XCore Memory Operand Value.
    /// </summary>
    public sealed class XCoreMemoryOperandValue {
        /// <summary>
        ///     Get Base Register.
        /// </summary>
        internal XCoreRegister Base { get; }

        /// <summary>
        ///     Get Direct Value.
        /// </summary>
        internal int Direct { get; }

        /// <summary>
        ///     Get Displacement Value.
        /// </summary>
        internal int Displacement { get; }

        /// <summary>
        ///     Get Index Register.
        /// </summary>
        internal XCoreRegister Index { get; }

        /// <summary>
        ///     Create an XCore Memory Operand Value.
        /// </summary>
        /// <param name="disassembler">
        ///     A disassembler.
        /// </param>
        /// <param name="nativeMemoryOperandValue">
        ///     A native XCore memory operand value.
        /// </param>
        internal XCoreMemoryOperandValue(CapstoneDisassembler disassembler, ref NativeXCoreMemoryOperandValue nativeMemoryOperandValue) {
            this.Base = XCoreRegister.TryCreate(disassembler, (XCoreRegisterId) nativeMemoryOperandValue.Base);
            this.Direct = nativeMemoryOperandValue.Direct;
            this.Displacement = nativeMemoryOperandValue.Displacement;
            this.Index = XCoreRegister.TryCreate(disassembler, (XCoreRegisterId) nativeMemoryOperandValue.Index);
        }
    }
}