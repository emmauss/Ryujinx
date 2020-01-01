using System.Runtime.InteropServices;

namespace Gee.External.Capstone.X86 {
    /// <summary>
    ///     Native X86 Flag.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    internal struct NativeX86Flag {
        /// <summary>
        ///     EFLAGS.
        /// </summary>
        [FieldOffset(0)]
        public long EFlags;

        /// <summary>
        ///     FPU Flags.
        /// </summary>
        [FieldOffset(0)]
        public long FpuFlags;
    }
}