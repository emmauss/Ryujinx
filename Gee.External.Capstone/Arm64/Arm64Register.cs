namespace Gee.External.Capstone.Arm64 {
    /// <summary>
    ///     ARM64 Register.
    /// </summary>
    public sealed class Arm64Register : Register<Arm64RegisterId> {
        /// <summary>
        ///     Create an ARM64 Register.
        /// </summary>
        /// <param name="disassembler">
        ///     A disassembler.
        /// </param>
        /// <param name="id">
        ///     The register's unique identifier.
        /// </param>
        /// <returns>
        ///     An ARM64 register.
        /// </returns>
        /// <exception cref="System.ObjectDisposedException">
        ///     Thrown if the disassembler is disposed.
        /// </exception>
        internal static Arm64Register TryCreate(CapstoneDisassembler disassembler, Arm64RegisterId id) {
            Arm64Register @object = null;
            if (id != Arm64RegisterId.Invalid) {
                // ...
                //
                // Throws an exception if the operation fails.
                var name = NativeCapstone.GetRegisterName(disassembler.Handle, (int) id);

                @object = new Arm64Register(id, name);
            }

            return @object;
        }

        /// <summary>
        ///     Create an ARM64 Register.
        /// </summary>
        /// <param name="id">
        ///     The register's unique identifier.
        /// </param>
        /// <param name="name">
        ///     The register's name.
        /// </param>
        internal Arm64Register(Arm64RegisterId id, string name) : base(id, name) { }
    }
}