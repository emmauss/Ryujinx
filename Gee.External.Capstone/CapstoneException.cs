using System;

namespace Gee.External.Capstone {
    /// <summary>
    ///     Capstone Exception.
    /// </summary>
    public sealed class CapstoneException : Exception {
        /// <summary>
        ///     Create a Capstone Exception.
        /// </summary>
        /// <param name="detailMessage">
        ///     A detail message describing the reason the exception was thrown.
        /// </param>
        internal CapstoneException(string detailMessage) : base(detailMessage) { }

        /// <summary>
        ///     Create a Capstone Exception.
        /// </summary>
        /// <param name="detailMessage">
        ///     A detail message describing the reason the exception was thrown.
        /// </param>
        /// <param name="innerException">
        ///     An exception that is the cause of this exception being thrown.
        /// </param>
        internal CapstoneException(string detailMessage, Exception innerException) : base(detailMessage, innerException) { }
    }
}