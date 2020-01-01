using System.Diagnostics.CodeAnalysis;

namespace Gee.External.Capstone.Arm64 {
    /// <summary>
    ///     ARM64 MSR System Register.
    /// </summary>
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum Arm64MsrSystemRegister {
        /// <summary>
        ///     Indicates an invalid, or an uninitialized, MSR register.
        /// </summary>
        Invalid = -1,
        ARM64_SYSREG_DBGDTRTX_EL0 = 0x9828,
        ARM64_SYSREG_OSLAR_EL1 = 0x8084,
        ARM64_SYSREG_PMSWINC_EL0 = 0xdce4,
        ARM64_SYSREG_TRCOSLAR = 0x8884,
        ARM64_SYSREG_TRCLAR = 0x8be6,
        ARM64_SYSREG_ICC_EOIR1_EL1 = 0xc661,
        ARM64_SYSREG_ICC_EOIR0_EL1 = 0xc641,
        ARM64_SYSREG_ICC_DIR_EL1 = 0xc659,
        ARM64_SYSREG_ICC_SGI1R_EL1 = 0xc65d,
        ARM64_SYSREG_ICC_ASGI1R_EL1 = 0xc65e,
        ARM64_SYSREG_ICC_SGI0R_EL1 = 0xc65f
    }
}