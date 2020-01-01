using System;

namespace Gee.External.Capstone.Mips {
    /// <summary>
    ///     MIPS Disassemble Mode.
    /// </summary>
    [Flags]
    public enum MipsDisassembleMode {
        /// <summary>
        ///     Indicates binary code should be disassembled in big-endian mode.
        /// </summary>
        BigEndian = NativeDisassembleMode.BigEndian,

        /// <summary>
        ///     Indicates binary code should be disassembled in 32-bit mode.
        /// </summary>
        Bit32 = NativeDisassembleMode.Bit32,

        /// <summary>
        ///     Indicates binary code should be disassembled in 64-bit mode.
        /// </summary>
        Bit64 = NativeDisassembleMode.Bit64,

        /// <summary>
        ///     Indicates binary code should be disassembled in little-endian mode.
        /// </summary>
        LittleEndian = NativeDisassembleMode.LittleEndian,

        /// <summary>
        ///     Indicates binary code should be disassembled with support for the microMIPS instruction set.
        /// </summary>
        Micro = NativeDisassembleMode.MipsMicro,

        /// <summary>
        ///     Indicates binary code should be disassembled in MIPS2 mode.
        /// </summary>
        Mips2 = NativeDisassembleMode.Mips2,

        /// <summary>
        ///     Indicates binary code should be disassembled in MIPS3 mode.
        /// </summary>
        Mips3 = NativeDisassembleMode.Mips3,

        /// <summary>
        ///     Indicates binary code should be disassembled in MIPS32R6 mode.
        /// </summary>
        Mips32R6 = NativeDisassembleMode.Mips32R6
    }
}