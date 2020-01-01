namespace Gee.External.Capstone.Arm {
    /// <summary>
    ///     ARM Register.
    /// </summary>
    public sealed class ArmRegister : Register<ArmRegisterId> {
        /// <summary>
        ///     Create an ARM Register.
        /// </summary>
        /// <param name="disassembler">
        ///     A disassembler.
        /// </param>
        /// <param name="id">
        ///     The register's unique identifier.
        /// </param>
        /// <returns>
        ///     An ARM register.
        /// </returns>
        /// <exception cref="System.ObjectDisposedException">
        ///     Thrown if the disassembler is disposed.
        /// </exception>
        internal static ArmRegister TryCreate(CapstoneDisassembler disassembler, ArmRegisterId id) {
            ArmRegister @object = null;
            if (id != ArmRegisterId.Invalid) {
                // ...
                //
                // Throws an exception if the operation fails.
                var name = NativeCapstone.GetRegisterName(disassembler.Handle, (int) id);

                @object = new ArmRegister(id, name);
            }

            return @object;
        }

        /// <summary>
        ///     Create an ARM Register.
        /// </summary>
        /// <param name="id">
        ///     The register's unique identifier.
        /// </param>
        /// <param name="name">
        ///     The register's name.
        /// </param>
        internal ArmRegister(ArmRegisterId id, string name) : base(id, name) { }
    }
}