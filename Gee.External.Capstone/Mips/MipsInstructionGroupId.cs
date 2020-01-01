using System.Diagnostics.CodeAnalysis;

namespace Gee.External.Capstone.Mips {
    /// <summary>
    ///     MIPS Instruction Group Unique Identifier.
    /// </summary>
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum MipsInstructionGroupId {
        /// <summary>
        ///     Indicates an invalid, or an uninitialized, instruction group.
        /// </summary>
        Invalid = 0,
        MIPS_GRP_JUMP,
        MIPS_GRP_CALL,
        MIPS_GRP_RET,
        MIPS_GRP_INT,
        MIPS_GRP_IRET,
        MIPS_GRP_PRIVILEGE,
        MIPS_GRP_BRANCH_RELATIVE,
        MIPS_GRP_BITCOUNT = 128,
        MIPS_GRP_DSP,
        MIPS_GRP_DSPR2,
        MIPS_GRP_FPIDX,
        MIPS_GRP_MSA,
        MIPS_GRP_MIPS32R2,
        MIPS_GRP_MIPS64,
        MIPS_GRP_MIPS64R2,
        MIPS_GRP_SEINREG,
        MIPS_GRP_STDENC,
        MIPS_GRP_SWAP,
        MIPS_GRP_MICROMIPS,
        MIPS_GRP_MIPS16MODE,
        MIPS_GRP_FP64BIT,
        MIPS_GRP_NONANSFPMATH,
        MIPS_GRP_NOTFP64BIT,
        MIPS_GRP_NOTINMICROMIPS,
        MIPS_GRP_NOTNACL,
        MIPS_GRP_NOTMIPS32R6,
        MIPS_GRP_NOTMIPS64R6,
        MIPS_GRP_CNMIPS,
        MIPS_GRP_MIPS32,
        MIPS_GRP_MIPS32R6,
        MIPS_GRP_MIPS64R6,
        MIPS_GRP_MIPS2,
        MIPS_GRP_MIPS3,
        MIPS_GRP_MIPS3_32,
        MIPS_GRP_MIPS3_32R2,
        MIPS_GRP_MIPS4_32,
        MIPS_GRP_MIPS4_32R2,
        MIPS_GRP_MIPS5_32R2,
        MIPS_GRP_GP32BIT,
        MIPS_GRP_GP64BIT
    }
}