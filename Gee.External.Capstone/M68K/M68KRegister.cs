namespace Gee.External.Capstone.M68K {
    /// <summary>
    ///     M68K Register.
    /// </summary>
    public sealed class M68KRegister : Register<M68KRegisterId> {
        /// <summary>
        ///     Create an M68K Register.
        /// </summary>
        /// <param name="disassembler">
        ///     A disassembler.
        /// </param>
        /// <param name="id">
        ///     The register's unique identifier.
        /// </param>
        /// <returns>
        ///     An M68K register. A null reference if the register's unique identifier is equal to
        ///     <see cref="M68KRegisterId.Invalid" />.
        /// </returns>
        /// <exception cref="System.ObjectDisposedException">
        ///     Thrown if the disassembler is disposed.
        /// </exception>
        internal static M68KRegister TryCreate(CapstoneDisassembler disassembler, M68KRegisterId id) {
            M68KRegister @object = null;
            if (id != M68KRegisterId.Invalid) {
                // ...
                //
                // Throws an exception if the operation fails.
                var name = NativeCapstone.GetRegisterName(disassembler.Handle, (int) id);

                @object = new M68KRegister(id, name);
            }

            return @object;
        }

        /// <summary>
        ///     Create a M68K Register.
        /// </summary>
        /// <param name="id">
        ///     The register's unique identifier.
        /// </param>
        /// <param name="name">
        ///     The register's name.
        /// </param>
        internal M68KRegister(M68KRegisterId id, string name) : base(id, name) { }
    }
}