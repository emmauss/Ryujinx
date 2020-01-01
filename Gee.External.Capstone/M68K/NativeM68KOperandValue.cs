using System.Runtime.InteropServices;

namespace Gee.External.Capstone.M68K {
    /// <summary>
    ///     Native M68K Operand Value.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    internal struct NativeM68KOperandValue {
        /// <summary>
        ///     Immediate Value.
        /// </summary>
        [FieldOffset(0)]
        public long Immediate;

        /// <summary>
        ///     Double Precision Immediate Value.
        /// </summary>
        [FieldOffset(0)]
        public double DImmediate;

        /// <summary>
        ///     Single Precision Immediate Value.
        /// </summary>
        [FieldOffset(0)]
        public float SImmediate;

        /// <summary>
        ///     Register Value.
        /// </summary>
        [FieldOffset(0)]
        public M68KRegisterId Register;

        /// <summary>
        ///     Register Pair Value.
        /// </summary>
        [FieldOffset(0)]
        public NativeM68KRegisterPairOperandValue RegisterPair;
    }
}