using System;

namespace Gee.External.Capstone {
    /// <summary>
    ///     Operand Access Type.
    /// </summary>
    [Flags]
    public enum OperandAccessType : byte {
        /// <summary>
        ///     Indicates an invalid, or an uninitialized, operand access type.
        /// </summary>
        Invalid = 0,
        Read = 1  << 0,
        Write = 1 << 1
    }
}