using System.Diagnostics.CodeAnalysis;

namespace Gee.External.Capstone.Arm64 {
    /// <summary>
    ///     ARM64 Prefetch Operation.
    /// </summary>
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum Arm64PrefetchOperation {
        /// <summary>
        ///     Indicates an invalid, or an uninitialized, prefetch operation.
        /// </summary>
        Invalid = 0,
        ARM64_PRFM_PLDL1KEEP = 0x00 + 1,
        ARM64_PRFM_PLDL1STRM = 0x01 + 1,
        ARM64_PRFM_PLDL2KEEP = 0x02 + 1,
        ARM64_PRFM_PLDL2STRM = 0x03 + 1,
        ARM64_PRFM_PLDL3KEEP = 0x04 + 1,
        ARM64_PRFM_PLDL3STRM = 0x05 + 1,
        ARM64_PRFM_PLIL1KEEP = 0x08 + 1,
        ARM64_PRFM_PLIL1STRM = 0x09 + 1,
        ARM64_PRFM_PLIL2KEEP = 0x0a + 1,
        ARM64_PRFM_PLIL2STRM = 0x0b + 1,
        ARM64_PRFM_PLIL3KEEP = 0x0c + 1,
        ARM64_PRFM_PLIL3STRM = 0x0d + 1,
        ARM64_PRFM_PSTL1KEEP = 0x10 + 1,
        ARM64_PRFM_PSTL1STRM = 0x11 + 1,
        ARM64_PRFM_PSTL2KEEP = 0x12 + 1,
        ARM64_PRFM_PSTL2STRM = 0x13 + 1,
        ARM64_PRFM_PSTL3KEEP = 0x14 + 1,
        ARM64_PRFM_PSTL3STRM = 0x15 + 1
    }
}