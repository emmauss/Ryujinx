using System.Diagnostics.CodeAnalysis;

namespace Gee.External.Capstone.Arm64 {
    /// <summary>
    ///     ARM64 Shift Operation.
    /// </summary>
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum Arm64ShiftOperation {
        /// <summary>
        ///     Indicates an invalid, or an uninitialized, shift operation.
        /// </summary>
        Invalid = 0,
        ARM64_SFT_LSL = 1,
        ARM64_SFT_MSL = 2,
        ARM64_SFT_LSR = 3,
        ARM64_SFT_ASR = 4,
        ARM64_SFT_ROR = 5
    }
}