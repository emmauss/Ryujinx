namespace Gee.External.Capstone.M68K {
    /// <summary>
    ///     M68K Instruction Group.
    /// </summary>
    public sealed class M68KInstructionGroup : InstructionGroup<M68KInstructionGroupId> {
        /// <summary>
        ///     Create an M68K Instruction Group.
        /// </summary>
        /// <param name="disassembler">
        ///     A disassembler.
        /// </param>
        /// <param name="id">
        ///     The instruction group's unique identifier.
        /// </param>
        /// <returns>
        ///     An M68K instruction group.
        /// </returns>
        internal static M68KInstructionGroup Create(CapstoneDisassembler disassembler, M68KInstructionGroupId id) {
            // ...
            //
            // Throws an exception if the operation fails.
            var name = NativeCapstone.GetInstructionGroupName(disassembler.Handle, (int) id);

            var @object = new M68KInstructionGroup(id, name);
            return @object;
        }

        /// <summary>
        ///     Create an M68K Instruction Group.
        /// </summary>
        /// <param name="id">
        ///     The instruction group's unique identifier.
        /// </param>
        /// <param name="name">
        ///     The instruction group's name.
        /// </param>
        internal M68KInstructionGroup(M68KInstructionGroupId id, string name) : base(id, name) { }
    }
}