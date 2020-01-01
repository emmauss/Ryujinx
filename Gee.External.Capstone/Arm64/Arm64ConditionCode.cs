using System.Diagnostics.CodeAnalysis;

namespace Gee.External.Capstone.Arm64 {
    /// <summary>
    ///     ARM64 Condition Code.
    /// </summary>
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum Arm64ConditionCode {
        /// <summary>
        ///     Indicates an invalid, or an uninitialized, condition code.
        /// </summary>
        Invalid = 0,
        ARM64_CC_EQ = 1,
        ARM64_CC_NE = 2,
        ARM64_CC_HS = 3,
        ARM64_CC_LO = 4,
        ARM64_CC_MI = 5,
        ARM64_CC_PL = 6,
        ARM64_CC_VS = 7,
        ARM64_CC_VC = 8,
        ARM64_CC_HI = 9,
        ARM64_CC_LS = 10,
        ARM64_CC_GE = 11,
        ARM64_CC_LT = 12,
        ARM64_CC_GT = 13,
        ARM64_CC_LE = 14,
        ARM64_CC_AL = 15,
        ARM64_CC_NV = 16
    }
}