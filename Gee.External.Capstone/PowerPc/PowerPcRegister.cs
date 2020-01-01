namespace Gee.External.Capstone.PowerPc {
    /// <summary>
    ///     PowerPC Register.
    /// </summary>
    public sealed class PowerPcRegister : Register<PowerPcRegisterId> {
        /// <summary>
        ///     Create a PowerPC Register.
        /// </summary>
        /// <param name="disassembler">
        ///     A disassembler.
        /// </param>
        /// <param name="id">
        ///     The register's unique identifier.
        /// </param>
        /// <returns>
        ///     A PowerPC register. A null reference if the register's unique identifier is equal to
        ///     <see cref="PowerPcRegisterId.Invalid" />.
        /// </returns>
        /// <exception cref="System.ObjectDisposedException">
        ///     Thrown if the disassembler is disposed.
        /// </exception>
        internal static PowerPcRegister TryCreate(CapstoneDisassembler disassembler, PowerPcRegisterId id) {
            PowerPcRegister @object = null;
            if (id != PowerPcRegisterId.Invalid) {
                // ...
                //
                // Throws an exception if the operation fails.
                var name = NativeCapstone.GetRegisterName(disassembler.Handle, (int) id);

                @object = new PowerPcRegister(id, name);
            }

            return @object;
        }

        /// <summary>
        ///     Create a PowerPC Register.
        /// </summary>
        /// <param name="id">
        ///     The register's unique identifier.
        /// </param>
        /// <param name="name">
        ///     The register's name.
        /// </param>
        internal PowerPcRegister(PowerPcRegisterId id, string name) : base(id, name) { }
    }
}