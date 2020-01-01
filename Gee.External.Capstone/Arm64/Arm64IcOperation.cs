using System.Diagnostics.CodeAnalysis;

namespace Gee.External.Capstone.Arm64 {
    /// <summary>
    ///     ARM64 Instruction Cache (IC) Operation.
    /// </summary>
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum Arm64IcOperation {
        /// <summary>
        ///     Indicates an invalid, or an uninitialized, IC operation.
        /// </summary>
        Invalid = 0,
        ARM64_IC_IALLUIS,
        ARM64_IC_IALLU,
        ARM64_IC_IVAU
    }
}