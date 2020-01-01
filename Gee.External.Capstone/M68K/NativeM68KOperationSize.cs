using System.Runtime.InteropServices;

namespace Gee.External.Capstone.M68K {
    /// <summary>
    ///     Native M68K Operation Size.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeM68KOperationSize {
        /// <summary>
        ///     Operation Size's Type.
        /// </summary>
        public M68KOperationSizeType Type;

        /// <summary>
        ///     Operation's Value.
        /// </summary>
        public NativeM68KOperationSizeValue Value;
    }
}