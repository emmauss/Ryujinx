using System.Diagnostics.CodeAnalysis;

namespace Gee.External.Capstone.M68K {
    /// <summary>
    ///     M68K Address Mode.
    /// </summary>
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum M68KAddressMode {
        /// <summary>
        ///     Indicates an invalid, or an uninitialized, address mode.
        /// </summary>
        Invalid = 0,
        M68K_AM_REG_DIRECT_DATA,
        M68K_AM_REG_DIRECT_ADDR,
        M68K_AM_REGI_ADDR,
        M68K_AM_REGI_ADDR_POST_INC,
        M68K_AM_REGI_ADDR_PRE_DEC,
        M68K_AM_REGI_ADDR_DISP,
        M68K_AM_AREGI_INDEX_8_BIT_DISP,
        M68K_AM_AREGI_INDEX_BASE_DISP,
        M68K_AM_MEMI_POST_INDEX,
        M68K_AM_MEMI_PRE_INDEX,
        M68K_AM_PCI_DISP,
        M68K_AM_PCI_INDEX_8_BIT_DISP,
        M68K_AM_PCI_INDEX_BASE_DISP,
        M68K_AM_PC_MEMI_POST_INDEX,
        M68K_AM_PC_MEMI_PRE_INDEX,
        M68K_AM_ABSOLUTE_DATA_SHORT,
        M68K_AM_ABSOLUTE_DATA_LONG,
        M68K_AM_IMMEDIATE,
        M68K_AM_BRANCH_DISPLACEMENT
    }
}