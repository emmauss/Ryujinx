using System.Diagnostics.CodeAnalysis;

namespace Gee.External.Capstone.X86 {
    /// <summary>
    ///     X86 AVX Condition Code.
    /// </summary>
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum X86AvxConditionCode {
        /// <summary>
        ///     Indicates an invalid, or an uninitialized, AVX condition code.
        /// </summary>
        Invalid = 0,
        X86_AVX_CC_EQ,
        X86_AVX_CC_LT,
        X86_AVX_CC_LE,
        X86_AVX_CC_UNORD,
        X86_AVX_CC_NEQ,
        X86_AVX_CC_NLT,
        X86_AVX_CC_NLE,
        X86_AVX_CC_ORD,
        X86_AVX_CC_EQ_UQ,
        X86_AVX_CC_NGE,
        X86_AVX_CC_NGT,
        X86_AVX_CC_FALSE,
        X86_AVX_CC_NEQ_OQ,
        X86_AVX_CC_GE,
        X86_AVX_CC_GT,
        X86_AVX_CC_TRUE,
        X86_AVX_CC_EQ_OS,
        X86_AVX_CC_LT_OQ,
        X86_AVX_CC_LE_OQ,
        X86_AVX_CC_UNORD_S,
        X86_AVX_CC_NEQ_US,
        X86_AVX_CC_NLT_UQ,
        X86_AVX_CC_NLE_UQ,
        X86_AVX_CC_ORD_S,
        X86_AVX_CC_EQ_US,
        X86_AVX_CC_NGE_UQ,
        X86_AVX_CC_NGT_UQ,
        X86_AVX_CC_FALSE_OS,
        X86_AVX_CC_NEQ_OS,
        X86_AVX_CC_GE_OQ,
        X86_AVX_CC_GT_OQ,
        X86_AVX_CC_TRUE_US
    }
}