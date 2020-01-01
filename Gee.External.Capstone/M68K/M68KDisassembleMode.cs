using System;

namespace Gee.External.Capstone.M68K {
    /// <summary>
    ///     M68K Disassemble Mode.
    /// </summary>
    [Flags]
    public enum M68KDisassembleMode {
        /// <summary>
        ///     Indicates binary code should be disassembled in big-endian mode.
        /// </summary>
        BigEndian = NativeDisassembleMode.BigEndian,

        /// <summary>
        ///     Indicates binary code should be disassembled with support for the M68K000 instruction set.
        /// </summary>
        M68K000 = NativeDisassembleMode.M68K000,

        /// <summary>
        ///     Indicates binary code should be disassembled with support for the M68K010 instruction set.
        /// </summary>
        M68K010 = NativeDisassembleMode.M68K010,

        /// <summary>
        ///     Indicates binary code should be disassembled with support for the M68K020 instruction set.
        /// </summary>
        M68K020 = NativeDisassembleMode.M68K020,

        /// <summary>
        ///     Indicates binary code should be disassembled with support for the M68K030 instruction set.
        /// </summary>
        M68K030 = NativeDisassembleMode.M68K030,

        /// <summary>
        ///     Indicates binary code should be disassembled with support for the M68K040 instruction set.
        /// </summary>
        M68K040 = NativeDisassembleMode.M68K040,

        /// <summary>
        ///     Indicates binary code should be disassembled with support for the M68K060 instruction set.
        /// </summary>
        M68K060 = NativeDisassembleMode.M68K060
    }
}