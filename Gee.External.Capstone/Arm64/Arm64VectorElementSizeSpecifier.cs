using System.Diagnostics.CodeAnalysis;

namespace Gee.External.Capstone.Arm64 {
    /// <summary>
    ///     ARM64 Vector Element Size Specifier.
    /// </summary>
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum Arm64VectorElementSizeSpecifier {
        /// <summary>
        ///     Indicates an invalid, or an uninitialized, vector element size specifier.
        /// </summary>
        Invalid = 0,
        ARM64_VESS_B,
        ARM64_VESS_H,
        ARM64_VESS_S,
        ARM64_VESS_D
    }
}