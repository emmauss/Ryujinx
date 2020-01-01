namespace Gee.External.Capstone {
    /// <summary>
    ///     Disassemble Syntax.
    /// </summary>
    public enum DisassembleSyntax {
        /// <summary>
        ///     Indicates a disassembler should use AT&amp;T syntax for generated assembly code.
        /// </summary>
        Att = NativeDisassemblerOptionValue.UseAttSyntax,

        /// <summary>
        ///     Indicates a disassembler should use Intel syntax for generated assembly code.
        /// </summary>
        Intel = NativeDisassemblerOptionValue.UseIntelSyntax,

        /// <summary>
        ///     Indicates a disassembler should use MASM syntax for generated assembly code.
        /// </summary>
        Masm = NativeDisassemblerOptionValue.UseMasmSyntax
    }
}