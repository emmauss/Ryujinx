using System.Diagnostics.CodeAnalysis;

namespace Gee.External.Capstone.X86 {
    /// <summary>
    ///     X86 AVX Rounding Mode.
    /// </summary>
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum X86AvxRoundingMode {
        /// <summary>
        ///     Indicates an invalid, or an uninitialized, AVX rounding mode.
        /// </summary>
        Invalid = 0,
        X86_AVX_RM_RN,
        X86_AVX_RM_RD,
        X86_AVX_RM_RU,
        X86_AVX_RM_RZ
    }
}