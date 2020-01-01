using System.Runtime.InteropServices;

namespace Gee.External.Capstone.XCore {
    /// <summary>
    ///     Native XCore Operand.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeXCoreOperand {
        /// <summary>
        ///     Operand's Type.
        /// </summary>
        public XCoreOperandType Type;

        /// <summary>
        ///     Operand's Value.
        /// </summary>
        public NativeXCoreOperandValue Value;
    }
}