namespace Gee.External.Capstone {
    /// <summary>
    ///     Native Capstone Result Code.
    /// </summary>
    internal enum NativeCapstoneResultCode {
        /// <summary>
        ///     Indicates an operation completed successfully.
        /// </summary>
        Ok = 0,

        /// <summary>
        ///     Indicates an operation failed because sufficient memory cannot be allocated to perform the operation.
        /// </summary>
        OutOfMemory,

        UnsupportedDisassembleArchitecture,
        InvalidHandle1,
        InvalidHandle2,
        UnsupportedDisassembleMode,
        InvalidOption,
        UnsupportedInstructionDetail,
        UninitializedMemoryManagement,
        UnsupportedVersion,
        UnSupportedDietModeOperation,
        UnsupportedSkipDataModeOperation,
        UnSupportedX86AttSyntax,
        UnSupportedX86IntelSyntax,
        UnSupportedX86MasmSyntax
    }
}