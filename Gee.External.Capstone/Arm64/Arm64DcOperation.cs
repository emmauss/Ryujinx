using System.Diagnostics.CodeAnalysis;

namespace Gee.External.Capstone.Arm64 {
    /// <summary>
    ///     ARM64 Data Cache (DC) Operation.
    /// </summary>
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum Arm64DcOperation {
        /// <summary>
        ///     Indicates an invalid, or an uninitialized, DC operation.
        /// </summary>
        Invalid = 0,
        ARM64_DC_ZVA,
        ARM64_DC_IVAC,
        ARM64_DC_ISW,
        ARM64_DC_CVAC,
        ARM64_DC_CSW,
        ARM64_DC_CVAU,
        ARM64_DC_CIVAC,
        ARM64_DC_CISW
    }
}