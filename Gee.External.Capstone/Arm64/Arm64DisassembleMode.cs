using System;

namespace Gee.External.Capstone.Arm64 {
    /// <summary>
    ///     ARM64 Disassemble Mode.
    /// </summary>
    [Flags]
    public enum Arm64DisassembleMode {
        /// <summary>
        ///     Indicates binary code should be disassembled in 32-bit ARM mode.
        /// </summary>
        Arm = NativeDisassembleMode.Arm,

        /// <summary>
        ///     Indicates binary code should be disassembled in big-endian mode.
        /// </summary>
        BigEndian = NativeDisassembleMode.BigEndian,

        /// <summary>
        ///     Indicates binary code should be disassembled in little-endian mode.
        /// </summary>
        LittleEndian = NativeDisassembleMode.LittleEndian
    }
}