using System.Diagnostics.CodeAnalysis;

namespace Gee.External.Capstone.Arm64 {
    /// <summary>
    ///     ARM64 Vector Arrangement Specifier.
    /// </summary>
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum Arm64VectorArrangementSpecifier {
        /// <summary>
        ///     Indicates an invalid, or an uninitialized, vector arrangement specifier.
        /// </summary>
        Invalid = 0,
        ARM64_VAS_8B,
        ARM64_VAS_16B,
        ARM64_VAS_4H,
        ARM64_VAS_8H,
        ARM64_VAS_2S,
        ARM64_VAS_4S,
        ARM64_VAS_1D,
        ARM64_VAS_2D,
        ARM64_VAS_1Q
    }
}