using System.Diagnostics.CodeAnalysis;

namespace Gee.External.Capstone.Arm {
    /// <summary>
    ///     ARM CPS Flag.
    /// </summary>
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum ArmCpsFlag {
        /// <summary>
        ///     Indicates an invalid, or an uninitialized, CPS flag.
        /// </summary>
        Invalid = 0,
        ARM_CPSFLAG_F = 1,
        ARM_CPSFLAG_I = 2,
        ARM_CPSFLAG_A = 4,
        ARM_CPSFLAG_NONE = 16
    }
}