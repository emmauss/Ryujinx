using System;

namespace Gee.External.Capstone.PowerPc {
    /// <summary>
    ///     PowerPC Disassemble Mode.
    /// </summary>
    [Flags]
    public enum PowerPcDisassembleMode {
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
        ///     Indicates binary code should be disassembled with support for the PowerPC Quad Processing
        ///     Extensions instruction sets.
        /// </summary>
        QuadProcessingExtensions = NativeDisassembleMode.PowerPcQuadProcessingExtensions
    }
}