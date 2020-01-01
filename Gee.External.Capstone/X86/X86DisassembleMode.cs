using System;

namespace Gee.External.Capstone.X86 {
    /// <summary>
    ///     X86 Disassemble Mode.
    /// </summary>
    [Flags]
    public enum X86DisassembleMode {
        /// <summary>
        ///     Indicates binary code should be disassembled in 16-bit mode.
        /// </summary>
        Bit16 = NativeDisassembleMode.Bit16,

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
        LittleEndian = NativeDisassembleMode.LittleEndian
    }
}