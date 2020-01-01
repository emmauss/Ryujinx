using System.Diagnostics.CodeAnalysis;

namespace Gee.External.Capstone.X86 {
    /// <summary>
    ///     X86 AVX Broadcast.
    /// </summary>
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum X86AvxBroadcast {
        /// <summary>
        ///     Indicates an invalid, or an uninitialized, AVX broadcast.
        /// </summary>
        Invalid = 0,
        X86_AVX_BCAST_2,
        X86_AVX_BCAST_4,
        X86_AVX_BCAST_8,
        X86_AVX_BCAST_16
    }
}