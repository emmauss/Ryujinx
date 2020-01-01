namespace Gee.External.Capstone.Mips {
    /// <summary>
    ///     MIPS Register.
    /// </summary>
    public sealed class MipsRegister : Register<MipsRegisterId> {
        /// <summary>
        ///     Create a MIPS Register.
        /// </summary>
        /// <param name="disassembler">
        ///     A disassembler.
        /// </param>
        /// <param name="id">
        ///     The register's unique identifier.
        /// </param>
        /// <returns>
        ///     A MIPS register. A null reference if the register's unique identifier is equal to
        ///     <see cref="MipsRegisterId.Invalid" />.
        /// </returns>
        /// <exception cref="System.ObjectDisposedException">
        ///     Thrown if the disassembler is disposed.
        /// </exception>
        internal static MipsRegister TryCreate(CapstoneDisassembler disassembler, MipsRegisterId id) {
            MipsRegister @object = null;
            if (id != MipsRegisterId.Invalid) {
                // ...
                //
                // Throws an exception if the operation fails.
                var name = NativeCapstone.GetRegisterName(disassembler.Handle, (int) id);

                @object = new MipsRegister(id, name);
            }

            return @object;
        }

        /// <summary>
        ///     Create a MIPS Register.
        /// </summary>
        /// <param name="id">
        ///     The register's unique identifier.
        /// </param>
        /// <param name="name">
        ///     The register's name.
        /// </param>
        internal MipsRegister(MipsRegisterId id, string name) : base(id, name) { }
    }
}