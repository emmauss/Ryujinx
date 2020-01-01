using System.Diagnostics.CodeAnalysis;

namespace Gee.External.Capstone.Arm {
    /// <summary>
    ///     ARM Memory Barrier Operation.
    /// </summary>
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum ArmMemoryBarrierOperation {
        /// <summary>
        ///     Indicates an invalid, or an uninitialized, memory barrier operation.
        /// </summary>
        Invalid = 0,
        ARM_MB_RESERVED_0,
        ARM_MB_OSHLD,
        ARM_MB_OSHST,
        ARM_MB_OSH,
        ARM_MB_RESERVED_4,
        ARM_MB_NSHLD,
        ARM_MB_NSHST,
        ARM_MB_NSH,
        ARM_MB_RESERVED_8,
        ARM_MB_ISHLD,
        ARM_MB_ISHST,
        ARM_MB_ISH,
        ARM_MB_RESERVED_12,
        ARM_MB_LD,
        ARM_MB_ST,
        ARM_MB_SY
    }
}