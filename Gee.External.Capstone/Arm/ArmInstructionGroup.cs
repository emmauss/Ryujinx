namespace Gee.External.Capstone.Arm {
    /// <summary>
    ///     ARM Instruction Group.
    /// </summary>
    public sealed class ArmInstructionGroup : InstructionGroup<ArmInstructionGroupId> {
        /// <summary>
        ///     Create an ARM Instruction Group.
        /// </summary>
        /// <param name="disassembler">
        ///     A disassembler.
        /// </param>
        /// <param name="id">
        ///     The instruction group's unique identifier.
        /// </param>
        /// <returns>
        ///     An ARM instruction group.
        /// </returns>
        internal static ArmInstructionGroup Create(CapstoneDisassembler disassembler, ArmInstructionGroupId id) {
            // ...
            //
            // Throws an exception if the operation fails.
            var name = NativeCapstone.GetInstructionGroupName(disassembler.Handle, (int) id);

            var @object = new ArmInstructionGroup(id, name);
            return @object;
        }

        /// <summary>
        ///     Create an ARM Instruction Group.
        /// </summary>
        internal ArmInstructionGroup(ArmInstructionGroupId id, string name) : base(id, name) { }
    }
}