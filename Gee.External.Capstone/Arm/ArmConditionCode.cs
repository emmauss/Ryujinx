using System.Diagnostics.CodeAnalysis;

namespace Gee.External.Capstone.Arm {
    /// <summary>
    ///     ARM Condition Code.
    /// </summary>
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum ArmConditionCode {
        /// <summary>
        ///     Indicates an invalid, or an uninitialized, condition code.
        /// </summary>
        Invalid = 0,
        ARM_CC_EQ,
        ARM_CC_NE,
        ARM_CC_HS,
        ARM_CC_LO,
        ARM_CC_MI,
        ARM_CC_PL,
        ARM_CC_VS,
        ARM_CC_VC,
        ARM_CC_HI,
        ARM_CC_LS,
        ARM_CC_GE,
        ARM_CC_LT,
        ARM_CC_GT,
        ARM_CC_LE,
        ARM_CC_AL
    }
}