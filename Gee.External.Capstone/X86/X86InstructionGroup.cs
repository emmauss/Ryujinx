namespace Gee.External.Capstone.X86 {
    /// <summary>
    ///     X86 Instruction Group.
    /// </summary>
    public sealed class X86InstructionGroup : InstructionGroup<X86InstructionGroupId> {
        /// <summary>
        ///     Create an X86 Instruction Group.
        /// </summary>
        /// <param name="disassembler">
        ///     A disassembler.
        /// </param>
        /// <param name="id">
        ///     The instruction group's unique identifier.
        /// </param>
        /// <returns>
        ///     An X86 instruction group.
        /// </returns>
        internal static X86InstructionGroup Create(CapstoneDisassembler disassembler, X86InstructionGroupId id) {
            // ...
            //
            // Throws an exception if the operation fails.
            var name = NativeCapstone.GetInstructionGroupName(disassembler.Handle, (int) id);

            var @object = new X86InstructionGroup(id, name);
            return @object;
        }

        /// <summary>
        ///     Create an X86 Instruction Group.
        /// </summary>
        internal X86InstructionGroup(X86InstructionGroupId id, string name) : base(id, name) { }
    }
}