using System.Diagnostics.CodeAnalysis;

namespace Gee.External.Capstone.PowerPc {
    /// <summary>
    ///     PowerPC Instruction Group Unique Identifier.
    /// </summary>
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum PowerPcInstructionGroupId {
        /// <summary>
        ///     Indicates an invalid, or an uninitialized, instruction group.
        /// </summary>
        Invalid = 0,
        PPC_GRP_JUMP,
        PPC_GRP_ALTIVEC = 128,
        PPC_GRP_MODE32,
        PPC_GRP_MODE64,
        PPC_GRP_BOOKE,
        PPC_GRP_NOTBOOKE,
        PPC_GRP_SPE,
        PPC_GRP_VSX,
        PPC_GRP_E500,
        PPC_GRP_PPC4XX,
        PPC_GRP_PPC6XX,
        PPC_GRP_ICBT,
        PPC_GRP_P8ALTIVEC,
        PPC_GRP_P8VECTOR,
        PPC_GRP_QPX
    }
}