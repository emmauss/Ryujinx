using System.Diagnostics.CodeAnalysis;

namespace Gee.External.Capstone.Arm {
    /// <summary>
    ///     ARM SETEND Operation.
    /// </summary>
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum ArmSetEndOperation {
        /// <summary>
        ///     Indicates an invalid, or an uninitialized, SETEND operation.
        /// </summary>
        Invalid = 0,
        ARM_SETEND_BE,
        ARM_SETEND_LE
    }
}