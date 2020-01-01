using System.Diagnostics.CodeAnalysis;

namespace Gee.External.Capstone.Arm64 {
    /// <summary>
    ///     ARM64 Address Translation (AT) Operation.
    /// </summary>
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum Arm64AtOperation {
        /// <summary>
        ///     Indicates an invalid, or an uninitialized, AT operation.
        /// </summary>
        Invalid = -1,
        ARM64_AT_S1E1R,
        ARM64_AT_S1E1W,
        ARM64_AT_S1E0R,
        ARM64_AT_S1E0W,
        ARM64_AT_S1E2R,
        ARM64_AT_S1E2W,
        ARM64_AT_S12E1R,
        ARM64_AT_S12E1W,
        ARM64_AT_S12E0R,
        ARM64_AT_S12E0W,
        ARM64_AT_S1E3R,
        ARM64_AT_S1E3W
    }
}