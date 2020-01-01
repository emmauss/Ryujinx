using System;
using System.Runtime.InteropServices;

namespace Gee.External.Capstone {
    /// <summary>
    ///     Safe Handle Extension.
    /// </summary>
    internal static class SafeHandleExtension {
        /// <summary>
        ///     Add a Reference to and Get a Handle.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Convenient method to add a reference to a handle before retrieving its wrapped pointer. If a
        ///         reference cannot be added to the handle, an exception is thrown. This follows Microsoft's
        ///         recommended best practice to add a reference to the handle before retrieving its wrapped pointer
        ///         to minimize the risk of handle recycle attacks. You, however, are responsible for releasing the
        ///         reference to the handle after you are done with it, using <c>SafeHandle.DangerousRelease()</c>,
        ///         otherwise you risk a memory leak.
        ///     </para>
        ///     <para>
        ///         This method is the equivalent to calling both <c>SafeHandle.DangerousAddRef()</c> and
        ///         <c>SafeHandle.DangerousGetHandle()</c>, except an exception is thrown if the operation fails. This
        ///         is for convenience if, and only if, you want to treat the failure of this operation as
        ///         exceptional! If you do not want to treat the failure of this operation as exceptional and you
        ///         instead have a non-exceptional back-off routine, do not call this method and perform your back-off
        ///         routine in an exception handler! You're better off simply calling both
        ///         <c>SafeHandle.DangerousAddRef()</c> and <c>SafeHandle.DangerousGetHandle()</c> yourself.
        ///     </para>
        /// </remarks>
        /// <param name="this">
        ///     A handle.
        /// </param>
        /// <returns>
        ///     The handle's wrapped pointer.
        /// </returns>
        /// <exception cref="System.ArgumentException">
        ///     Thrown if the handle is closed, or if the handle is invalid.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        ///     Thrown if the handle is a null reference.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        ///     Thrown if a reference to the handle could not be added.
        /// </exception>
        internal static IntPtr DangerousAddRefAndGetHandle(this SafeHandle @this) {
            var isReferenceAdded = false;
            @this.DangerousAddRef(ref isReferenceAdded);
            if (!isReferenceAdded) {
                const string detailMessage = "Unable to add a reference to a handle.";
                throw new InvalidOperationException(detailMessage);
            }

            var pHandle = @this.DangerousGetHandle();
            return pHandle;
        }
    }
}