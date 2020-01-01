using System.Runtime.InteropServices;

namespace Gee.External.Capstone.M68K {
    /// <summary>
    ///     Native M68K Memory Operand Value.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeM68KMemoryOperandValue {
        /// <summary>
        ///     Base Register.
        /// </summary>
        public M68KRegisterId Base;

        /// <summary>
        ///     Index Register.
        /// </summary>
        public M68KRegisterId Index;

        /// <summary>
        ///     Indirect Base Register.
        /// </summary>
        public M68KRegisterId IndirectBase;

        /// <summary>
        ///     Indirect Displacement.
        /// </summary>
        public int IndirectDisplacement;

        /// <summary>
        ///     Other Displacement.
        /// </summary>
        public int OutDisplacement;

        /// <summary>
        ///     Displacement.
        /// </summary>
        public short Displacement;

        /// <summary>
        ///     Index Register's Scale.
        /// </summary>
        public byte Scale;

        /// <summary>
        ///     Bit Field.
        /// </summary>
        public byte BitField;

        /// <summary>
        ///     Bit Field Width.
        /// </summary>
        public byte Width;

        /// <summary>
        ///     Bit Field Offset.
        /// </summary>
        public byte Offset;

        /// <summary>
        ///     Index Size.
        /// </summary>
        public byte IndexSize;
    }
}