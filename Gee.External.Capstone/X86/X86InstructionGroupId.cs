using System.Diagnostics.CodeAnalysis;

namespace Gee.External.Capstone.X86 {
    /// <summary>
    ///     X86 Instruction Group Unique Identifier.
    /// </summary>
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum X86InstructionGroupId : byte {
        /// <summary>
        ///     Indicates an invalid, or an uninitialized, instruction group.
        /// </summary>
        Invalid = 0,
        X86_GRP_JUMP,
        X86_GRP_CALL,
        X86_GRP_RET,
        X86_GRP_INT,
        X86_GRP_IRET,
        X86_GRP_PRIVILEGE,
        X86_GRP_BRANCH_RELATIVE,
        X86_GRP_VM = 128,
        X86_GRP_3DNOW,
        X86_GRP_AES,
        X86_GRP_ADX,
        X86_GRP_AVX,
        X86_GRP_AVX2,
        X86_GRP_AVX512,
        X86_GRP_BMI,
        X86_GRP_BMI2,
        X86_GRP_CMOV,
        X86_GRP_F16C,
        X86_GRP_FMA,
        X86_GRP_FMA4,
        X86_GRP_FSGSBASE,
        X86_GRP_HLE,
        X86_GRP_MMX,
        X86_GRP_MODE32,
        X86_GRP_MODE64,
        X86_GRP_RTM,
        X86_GRP_SHA,
        X86_GRP_SSE1,
        X86_GRP_SSE2,
        X86_GRP_SSE3,
        X86_GRP_SSE41,
        X86_GRP_SSE42,
        X86_GRP_SSE4A,
        X86_GRP_SSSE3,
        X86_GRP_PCLMUL,
        X86_GRP_XOP,
        X86_GRP_CDI,
        X86_GRP_ERI,
        X86_GRP_TBM,
        X86_GRP_16BITMODE,
        X86_GRP_NOT64BITMODE,
        X86_GRP_SGX,
        X86_GRP_DQI,
        X86_GRP_BWI,
        X86_GRP_PFI,
        X86_GRP_VLX,
        X86_GRP_SMAP,
        X86_GRP_NOVLX,
        X86_GRP_FPU
    }
}