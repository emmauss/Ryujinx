using Microsoft.Win32.SafeHandles;
using System;

namespace Gee.External.Capstone {
    /// <summary>
    ///     Native Disassembler Handle.
    /// </summary>
    internal sealed class NativeDisassemblerHandle : SafeHandleZeroOrMinusOneIsInvalid {
        /// <summary>
        ///     Create a Native Disassembler Handle.
        /// </summary>
        /// <param name="pDisassembler">
        ///     A pointer to a disassembler.
        /// </param>
        internal NativeDisassemblerHandle(IntPtr pDisassembler) : base(true) {
            this.handle = pDisassembler;
        }

        /// <summary>
        ///     Release Handle.
        /// </summary>
        /// <returns>
        ///     A boolean true if the handle was released. A boolean false otherwise.
        /// </returns>
        protected override bool ReleaseHandle() {
            var resultCode = NativeCapstoneImport.CloseDisassembler(ref this.handle);
            var isHandleReleased = resultCode == NativeCapstoneResultCode.Ok;

            return isHandleReleased;
        }
    }
}