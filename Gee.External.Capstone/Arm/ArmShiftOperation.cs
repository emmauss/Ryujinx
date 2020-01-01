using System.Diagnostics.CodeAnalysis;

namespace Gee.External.Capstone.Arm {
    /// <summary>
    ///     ARM Shift Operation.
    /// </summary>
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum ArmShiftOperation {
        /// <summary>
        ///     Indicates an invalid, or an uninitialized, shift operation.
        /// </summary>
        Invalid = 0,
        ARM_SFT_ASR,
        ARM_SFT_LSL,
        ARM_SFT_LSR,
        ARM_SFT_ROR,
        ARM_SFT_RRX,
        ARM_SFT_ASR_REG,
        ARM_SFT_LSL_REG,
        ARM_SFT_LSR_REG,
        ARM_SFT_ROR_REG,
        ARM_SFT_RRX_REG
    }
}