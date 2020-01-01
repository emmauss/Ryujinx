using System.Diagnostics.CodeAnalysis;

namespace Gee.External.Capstone.Arm64 {
    /// <summary>
    ///     ARM64 Extend Operation.
    /// </summary>
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum Arm64ExtendOperation {
        /// <summary>
        ///     Indicates an invalid, or an uninitialized, extend operation.
        /// </summary>
        Invalid = 0,
        ARM64_EXT_UXTB = 1,
        ARM64_EXT_UXTH = 2,
        ARM64_EXT_UXTW = 3,
        ARM64_EXT_UXTX = 4,
        ARM64_EXT_SXTB = 5,
        ARM64_EXT_SXTH = 6,
        ARM64_EXT_SXTW = 7,
        ARM64_EXT_SXTX = 8
    }
}