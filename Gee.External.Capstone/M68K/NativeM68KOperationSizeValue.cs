using System.Runtime.InteropServices;

namespace Gee.External.Capstone.M68K {
    /// <summary>
    ///     Native M68K Operation Size Value.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    internal struct NativeM68KOperationSizeValue {
        /// <summary>
        ///     CPU Operation Size.
        /// </summary>
        [FieldOffset(0)]
        public M68KCpuOperationSize CpuOperationSize;

        /// <summary>
        ///     FPU Operation Size.
        /// </summary>
        [FieldOffset(0)]
        public M68KFpuOperationSize FpuOperationSize;
    }
}