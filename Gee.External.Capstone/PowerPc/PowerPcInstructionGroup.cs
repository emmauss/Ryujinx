namespace Gee.External.Capstone.PowerPc {
    /// <summary>
    ///     PowerPC Instruction Group.
    /// </summary>
    public sealed class PowerPcInstructionGroup : InstructionGroup<PowerPcInstructionGroupId> {
        /// <summary>
        ///     Create a PowerPC Instruction Group.
        /// </summary>
        /// <param name="disassembler">
        ///     A disassembler.
        /// </param>
        /// <param name="id">
        ///     The instruction group's unique identifier.
        /// </param>
        /// <returns>
        ///     A PowerPC instruction group.
        /// </returns>
        internal static PowerPcInstructionGroup Create(CapstoneDisassembler disassembler, PowerPcInstructionGroupId id) {
            // ...
            //
            // Throws an exception if the operation fails.
            var name = NativeCapstone.GetInstructionGroupName(disassembler.Handle, (int) id);

            var @object = new PowerPcInstructionGroup(id, name);
            return @object;
        }

        /// <summary>
        ///     Create a PowerPC Instruction Group.
        /// </summary>
        /// <param name="id">
        ///     The instruction group's unique identifier.
        /// </param>
        /// <param name="name">
        ///     The instruction group's name.
        /// </param>
        internal PowerPcInstructionGroup(PowerPcInstructionGroupId id, string name) : base(id, name) { }
    }
}