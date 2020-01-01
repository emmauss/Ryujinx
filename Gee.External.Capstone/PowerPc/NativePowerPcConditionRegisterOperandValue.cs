using System.Runtime.InteropServices;

namespace Gee.External.Capstone.PowerPc {
    /// <summary>
    ///     Native PowerPC Condition Register Operand Value.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct NativePowerPcConditionRegisterOperandValue {
        /// <summary>
        ///     Scale.
        /// </summary>
        public int Scale;

        /// <summary>
        ///     Register.
        /// </summary>
        public PowerPcRegisterId Register;

        /// <summary>
        ///     Branch Code.
        /// </summary>
        public PowerPcBranchCode BranchCode;
    }
}