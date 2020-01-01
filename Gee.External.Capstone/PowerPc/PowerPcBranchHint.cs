using System.Diagnostics.CodeAnalysis;

namespace Gee.External.Capstone.PowerPc {
    /// <summary>
    ///     PowerPC Branch Hint.
    /// </summary>
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum PowerPcBranchHint {
        /// <summary>
        ///     Indicates an invalid, or an uninitialized, branch hint.
        /// </summary>
        Invalid = 0,
        PPC_BH_PLUS,
        PPC_BH_MINUS
    }
}