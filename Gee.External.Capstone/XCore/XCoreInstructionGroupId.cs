using System.Diagnostics.CodeAnalysis;

namespace Gee.External.Capstone.XCore {
    /// <summary>
    ///     XCore Instruction Group Unique Identifier.
    /// </summary>
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum XCoreInstructionGroupId {
        /// <summary>
        ///     Indicates an invalid, or an uninitialized, instruction group.
        /// </summary>
        Invalid = 0,
        XCORE_GRP_JUMP
    }
}