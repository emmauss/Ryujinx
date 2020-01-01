namespace Gee.External.Capstone.XCore {
    /// <summary>
    ///     XCore Instruction Group.
    /// </summary>
    public sealed class XCoreInstructionGroup : InstructionGroup<XCoreInstructionGroupId> {
        /// <summary>
        ///     Create an XCore Instruction Group.
        /// </summary>
        /// <param name="disassembler">
        ///     A disassembler.
        /// </param>
        /// <param name="id">
        ///     The instruction group's unique identifier.
        /// </param>
        /// <returns>
        ///     An XCore instruction group.
        /// </returns>
        internal static XCoreInstructionGroup Create(CapstoneDisassembler disassembler, XCoreInstructionGroupId id) {
            // ...
            //
            // Throws an exception if the operation fails.
            var name = NativeCapstone.GetInstructionGroupName(disassembler.Handle, (int) id);

            var @object = new XCoreInstructionGroup(id, name);
            return @object;
        }

        /// <summary>
        ///     Create an XCore Instruction Group.
        /// </summary>
        /// <param name="id">
        ///     The instruction group's unique identifier.
        /// </param>
        /// <param name="name">
        ///     The instruction group's name.
        /// </param>
        internal XCoreInstructionGroup(XCoreInstructionGroupId id, string name) : base(id, name) { }
    }
}