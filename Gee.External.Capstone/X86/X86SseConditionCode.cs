using System.Diagnostics.CodeAnalysis;

namespace Gee.External.Capstone.X86 {
    /// <summary>
    ///     X86 SSE Condition Code.
    /// </summary>
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum X86SseConditionCode {
        /// <summary>
        ///     Indicates an invalid, or an uninitialized, SSE condition code.
        /// </summary>
        Invalid = 0,
        X86_SSE_CC_EQ,
        X86_SSE_CC_LT,
        X86_SSE_CC_LE,
        X86_SSE_CC_UNORD,
        X86_SSE_CC_NEQ,
        X86_SSE_CC_NLT,
        X86_SSE_CC_NLE,
        X86_SSE_CC_ORD
    }
}