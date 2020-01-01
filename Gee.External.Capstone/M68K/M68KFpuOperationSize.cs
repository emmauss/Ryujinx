namespace Gee.External.Capstone.M68K {
    /// <summary>
    ///     M68K FPU Operation Size.
    /// </summary>
    public enum M68KFpuOperationSize {
        /// <summary>
        ///     Indicates an invalid, or an uninitialized, FPU operation.
        /// </summary>
        Invalid = 0,

        /// <summary>
        ///     Indicates a signed 4-byte FPU operation.
        /// </summary>
        Single = 4,

        /// <summary>
        ///     Indicates a signed 8-byte FPU operation.
        /// </summary>
        Double = 8,

        /// <summary>
        ///     Indicates a signed 12-byte FPU operation.
        /// </summary>
        Extended = 12
    }
}