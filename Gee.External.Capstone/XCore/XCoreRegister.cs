namespace Gee.External.Capstone.XCore {
    /// <summary>
    ///     XCore Register.
    /// </summary>
    public sealed class XCoreRegister : Register<XCoreRegisterId> {
        /// <summary>
        ///     Create an XCore Register.
        /// </summary>
        /// <param name="disassembler">
        ///     A disassembler.
        /// </param>
        /// <param name="id">
        ///     The register's unique identifier.
        /// </param>
        /// <returns>
        ///     An XCore register. A null reference if the register's unique identifier is equal to
        ///     <see cref="XCoreRegisterId.Invalid" />.
        /// </returns>
        /// <exception cref="System.ObjectDisposedException">
        ///     Thrown if the disassembler is disposed.
        /// </exception>
        internal static XCoreRegister TryCreate(CapstoneDisassembler disassembler, XCoreRegisterId id) {
            XCoreRegister @object = null;
            if (id != XCoreRegisterId.Invalid) {
                // ...
                //
                // Throws an exception if the operation fails.
                var name = NativeCapstone.GetRegisterName(disassembler.Handle, (int) id);

                @object = new XCoreRegister(id, name);
            }

            return @object;
        }

        /// <summary>
        ///     Create a XCore Register.
        /// </summary>
        /// <param name="id">
        ///     The register's unique identifier.
        /// </param>
        /// <param name="name">
        ///     The register's name.
        /// </param>
        internal XCoreRegister(XCoreRegisterId id, string name) : base(id, name) { }
    }
}