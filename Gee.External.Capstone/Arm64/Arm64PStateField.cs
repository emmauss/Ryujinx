using System.Diagnostics.CodeAnalysis;

namespace Gee.External.Capstone.Arm64 {
    /// <summary>
    ///     ARM64 Processor State (PSTATE) Field.
    /// </summary>
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum Arm64PStateField {
        /// <summary>
        ///     Indicates an invalid, or an uninitialized, PSTATE field.
        /// </summary>
        Invalid = 0,
        ARM64_PSTATE_SPSEL = 0x05,
        ARM64_PSTATE_DAIFSET = 0x1e,
        ARM64_PSTATE_DAIFCLR = 0x1f
    }
}