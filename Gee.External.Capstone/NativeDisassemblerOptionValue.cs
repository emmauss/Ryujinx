namespace Gee.External.Capstone {
    /// <summary>
    ///     Native Disassembler Option Value.
    /// </summary>
    internal enum NativeDisassemblerOptionValue {
        /// <summary>
        ///     Indicates an option should be disabled.
        /// </summary>
        Disable = 0,

        /// <summary>
        ///     Indicates an option should be enabled.
        /// </summary>
        Enable = 3,

        /// <summary>
        ///     Indicates a disassembler should use its default syntax for generated assembly code.
        /// </summary>
        UseDefaultSyntax = 0,

        /// <summary>
        ///     Indicates a disassembler should use Intel syntax for generated assembly code.
        /// </summary>
        UseIntelSyntax,

        /// <summary>
        ///     Indicates a disassembler should use AT\&\T syntax for generated assembly code.
        /// </summary>
        UseAttSyntax,

        CS_OPT_SYNTAX_NOREGNAME,

        /// <summary>
        ///     Indicates a disassembler should use MASM syntax for generated assembly code.
        /// </summary>
        UseMasmSyntax
    }
}