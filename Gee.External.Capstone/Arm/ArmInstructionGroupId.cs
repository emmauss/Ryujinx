using System.Diagnostics.CodeAnalysis;

namespace Gee.External.Capstone.Arm {
    /// <summary>
    ///     ARM Instruction Group Unique Identifier.
    /// </summary>
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum ArmInstructionGroupId {
        /// <summary>
        ///     Indicates an invalid, or an uninitialized, instruction group.
        /// </summary>
        Invalid = 0,
        ARM_GRP_JUMP,
        ARM_GRP_CALL,
        ARM_GRP_INT = 4,
        ARM_GRP_PRIVILEGE = 6,
        ARM_GRP_BRANCH_RELATIVE,
        ARM_GRP_CRYPTO = 128,
        ARM_GRP_DATABARRIER,
        ARM_GRP_DIVIDE,
        ARM_GRP_FPARMV8,
        ARM_GRP_MULTPRO,
        ARM_GRP_NEON,
        ARM_GRP_T2EXTRACTPACK,
        ARM_GRP_THUMB2DSP,
        ARM_GRP_TRUSTZONE,
        ARM_GRP_V4T,
        ARM_GRP_V5T,
        ARM_GRP_V5TE,
        ARM_GRP_V6,
        ARM_GRP_V6T2,
        ARM_GRP_V7,
        ARM_GRP_V8,
        ARM_GRP_VFP2,
        ARM_GRP_VFP3,
        ARM_GRP_VFP4,
        ARM_GRP_ARM,
        ARM_GRP_MCLASS,
        ARM_GRP_NOTMCLASS,
        ARM_GRP_THUMB,
        ARM_GRP_THUMB1ONLY,
        ARM_GRP_THUMB2,
        ARM_GRP_PREV8,
        ARM_GRP_FPVMLX,
        ARM_GRP_MULOPS,
        ARM_GRP_CRC,
        ARM_GRP_DPVFP,
        ARM_GRP_V6M,
        ARM_GRP_VIRTUALIZATION
    }
}