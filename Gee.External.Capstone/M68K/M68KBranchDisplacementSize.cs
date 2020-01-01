namespace Gee.External.Capstone.M68K {
    /// <summary>
    ///     M68K Branch Displacement Size.
    /// </summary>
    public enum M68KBranchDisplacementSize : byte {
        /// <summary>
        ///     Indicates an invalid, or an uninitialized, branch displacement size.
        /// </summary>
        Invalid = 0,

        /// <summary>
        ///     Indicates a signed 8-bit branch displacement size.
        /// </summary>
        Byte = 1,

        /// <summary>
        ///     Indicates a signed 16-bit branch displacement size.
        /// </summary>
        Word = 2,

        /// <summary>
        ///     Indicates a signed 32-bit branch displacement size.
        /// </summary>
        Long = 4
    }
}