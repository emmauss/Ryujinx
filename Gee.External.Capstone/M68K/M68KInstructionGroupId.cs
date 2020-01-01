using System.Diagnostics.CodeAnalysis;

namespace Gee.External.Capstone.M68K {
    /// <summary>
    ///     M68K Instruction Group Unique Identifier.
    /// </summary>
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum M68KInstructionGroupId {
        /// <summary>
        ///     Indicates an invalid, or an uninitialized, instruction group.
        /// </summary>
        Invalid = 0,
        M68K_GRP_JUMP,
        M68K_GRP_RET = 3,
        M68K_GRP_IRET = 5,
        M68K_GRP_BRANCH_RELATIVE = 7
    }
}