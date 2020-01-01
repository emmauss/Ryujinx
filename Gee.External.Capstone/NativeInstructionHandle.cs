using Microsoft.Win32.SafeHandles;
using System;

namespace Gee.External.Capstone {
    /// <summary>
    ///     Native Instruction Handle.
    /// </summary>
    internal sealed class NativeInstructionHandle : SafeHandleZeroOrMinusOneIsInvalid {
        /// <summary>
        ///     Create an Instruction Handle.
        /// </summary>
        /// <param name="pInstruction">
        ///     A pointer to an instruction.
        /// </param>
        internal NativeInstructionHandle(IntPtr pInstruction) : base(true) {
            this.handle = pInstruction;
        }

        /// <summary>
        ///     Release Handle.
        /// </summary>
        /// <returns>
        ///     A boolean true if the handle was released. A boolean false otherwise.
        /// </returns>
        protected override bool ReleaseHandle() {
            NativeCapstoneImport.FreeInstructions(this.handle, (IntPtr) 1);
            this.handle = IntPtr.Zero;

            return true;
        }
    }
}