namespace Gee.External.Capstone.Mips {
    /// <summary>
    ///     MIPS Instruction Group.
    /// </summary>
    public sealed class MipsInstructionGroup : InstructionGroup<MipsInstructionGroupId> {
        /// <summary>
        ///     Create a MIPS Instruction Group.
        /// </summary>
        /// <param name="disassembler">
        ///     A disassembler.
        /// </param>
        /// <param name="id">
        ///     The instruction group's unique identifier.
        /// </param>
        /// <returns>
        ///     A MIPS instruction group.
        /// </returns>
        internal static MipsInstructionGroup Create(CapstoneDisassembler disassembler, MipsInstructionGroupId id) {
            // ...
            //
            // Throws an exception if the operation fails.
            var name = NativeCapstone.GetInstructionGroupName(disassembler.Handle, (int) id);

            var @object = new MipsInstructionGroup(id, name);
            return @object;
        }

        /// <summary>
        ///     Create a MIPS Instruction Group.
        /// </summary>
        /// <param name="id">
        ///     The instruction group's unique identifier.
        /// </param>
        /// <param name="name">
        ///     The instruction group's name.
        /// </param>
        internal MipsInstructionGroup(MipsInstructionGroupId id, string name) : base(id, name) { }
    }
}