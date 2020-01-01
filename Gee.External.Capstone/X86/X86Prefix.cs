using System.Diagnostics.CodeAnalysis;

namespace Gee.External.Capstone.X86 {
    /// <summary>
    ///     X86 Prefix.
    /// </summary>
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum X86Prefix : byte {
        /// <summary>
        ///     Indicates an invalid, or an uninitialized, prefix.
        /// </summary>
        Invalid = 0,
        X86_PREFIX_LOCK = 0xf0,
        X86_PREFIX_REP = 0xf3,
        X86_PREFIX_REPE = 0xf3,
        X86_PREFIX_REPNE = 0xf2,
        X86_PREFIX_CS = 0x2e,
        X86_PREFIX_SS = 0x36,
        X86_PREFIX_DS = 0x3e,
        X86_PREFIX_ES = 0x26,
        X86_PREFIX_FS = 0x64,
        X86_PREFIX_GS = 0x65,
        X86_PREFIX_OPSIZE = 0x66,
        X86_PREFIX_ADDRSIZE = 0x67
    }
}