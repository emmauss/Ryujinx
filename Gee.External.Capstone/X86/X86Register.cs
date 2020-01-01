namespace Gee.External.Capstone.X86 {
    /// <summary>
    ///     X86 Register.
    /// </summary>
    public sealed class X86Register : Register<X86RegisterId> {
        /// <summary>
        ///     Create an X86 Register.
        /// </summary>
        /// <param name="disassembler">
        ///     A disassembler.
        /// </param>
        /// <param name="id">
        ///     The register's unique identifier.
        /// </param>
        /// <returns>
        ///     An X86 register. A null reference if the register's unique identifier is equal to
        ///     <see cref="X86RegisterId.Invalid" />.
        /// </returns>
        /// <exception cref="System.ObjectDisposedException">
        ///     Thrown if the disassembler is disposed.
        /// </exception>
        internal static X86Register TryCreate(CapstoneDisassembler disassembler, X86RegisterId id) {
            X86Register @object = null;
            if (id != X86RegisterId.Invalid) {
                // ...
                //
                // Throws an exception if the operation fails.
                var name = NativeCapstone.GetRegisterName(disassembler.Handle, (int) id);

                @object = new X86Register(id, name);
            }

            return @object;
        }

        /// <summary>
        ///     Create an X86 Register.
        /// </summary>
        /// <param name="id">
        ///     The register's unique identifier.
        /// </param>
        /// <param name="name">
        ///     The register's name.
        /// </param>
        internal X86Register(X86RegisterId id, string name) : base(id, name) { }
    }
}