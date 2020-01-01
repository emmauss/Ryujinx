using System.Diagnostics.CodeAnalysis;

namespace Gee.External.Capstone.XCore {
    /// <summary>
    ///     XCore Register Unique Identifier.
    /// </summary>
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum XCoreRegisterId {
        /// <summary>
        ///     Indicates an invalid, or an uninitialized, register.
        /// </summary>
        Invalid = 0,
        XCORE_REG_CP,
        XCORE_REG_DP,
        XCORE_REG_LR,
        XCORE_REG_SP,
        XCORE_REG_R0,
        XCORE_REG_R1,
        XCORE_REG_R2,
        XCORE_REG_R3,
        XCORE_REG_R4,
        XCORE_REG_R5,
        XCORE_REG_R6,
        XCORE_REG_R7,
        XCORE_REG_R8,
        XCORE_REG_R9,
        XCORE_REG_R10,
        XCORE_REG_R11,
        XCORE_REG_PC,
        XCORE_REG_SCP,
        XCORE_REG_SSR,
        XCORE_REG_ET,
        XCORE_REG_ED,
        XCORE_REG_SED,
        XCORE_REG_KEP,
        XCORE_REG_KSP,
        XCORE_REG_ID
    }
}