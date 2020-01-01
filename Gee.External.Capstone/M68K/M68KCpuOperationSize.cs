namespace Gee.External.Capstone.M68K {
    /// <summary>
    ///     M68K CPU Operation Size.
    /// </summary>
    public enum M68KCpuOperationSize {
        /// <summary>
        ///     Indicates an invalid, or an uninitialized, CPU operation.
        /// </summary>
        Invalid = 0,

        /// <summary>
        ///     Indicates a signed 8-bit CPU operation.
        /// </summary>
        Byte = 1,

        /// <summary>
        ///     Indicates a signed 16-bit CPU operation.
        /// </summary>
        Word = 2,

        /// <summary>
        ///     Indicates a signed 32-bit CPU operation.
        /// </summary>
        Long = 4
    }
}