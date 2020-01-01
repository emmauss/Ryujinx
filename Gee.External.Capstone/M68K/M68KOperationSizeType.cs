namespace Gee.External.Capstone.M68K {
    /// <summary>
    ///     M68K Operation Size Type.
    /// </summary>
    public enum M68KOperationSizeType {
        /// <summary>
        ///     Indicates an invalid, or an uninitialized, operation size type.
        /// </summary>
        Invalid = 0,

        /// <summary>
        ///     Indicates a CPU operation size.
        /// </summary>
        Cpu,

        /// <summary>
        ///     Indicates an FPU operation size.
        /// </summary>
        Fpu
    }
}