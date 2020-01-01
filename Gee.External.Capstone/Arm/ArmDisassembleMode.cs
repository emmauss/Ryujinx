using System;

namespace Gee.External.Capstone.Arm {
    /// <summary>
    ///     ARM Disassemble Mode.
    /// </summary>
    [Flags]
    public enum ArmDisassembleMode {
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
        LittleEndian = NativeDisassembleMode.LittleEndian,

        /// <summary>
        ///     Indicates binary code should be disassembled with support for the ARM Cortex-M processor cores.
        /// </summary>
        CortexM = NativeDisassembleMode.ArmCortexM,

        /// <summary>
        ///     Indicates binary code should be disassembled with support for the ARM Thumb and ARM Thumb-2
        ///     instruction sets.
        /// </summary>
        Thumb = NativeDisassembleMode.ArmThumb,

        /// <summary>
        ///     Indicates binary code should be disassembled with support for the ARMv8 instruction set.
        /// </summary>
        V8 = NativeDisassembleMode.ArmV8,
    }
}