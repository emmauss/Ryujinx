using System.Diagnostics.CodeAnalysis;

namespace Gee.External.Capstone.X86 {
    /// <summary>
    ///     X86 XOP Condition Code.
    /// </summary>
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum X86XopConditionCode {
        /// <summary>
        ///     Indicates an invalid, or an uninitialized, XOP condition code.
        /// </summary>
        Invalid = 0,
        X86_XOP_CC_LT,
        X86_XOP_CC_LE,
        X86_XOP_CC_GT,
        X86_XOP_CC_GE,
        X86_XOP_CC_EQ,
        X86_XOP_CC_NEQ,
        X86_XOP_CC_FALSE,
        X86_XOP_CC_TRUE
    }
}