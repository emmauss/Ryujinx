namespace Gee.External.Capstone.Arm64 {
    /// <summary>
    ///     ARM64 Instruction Group.
    /// </summary>
    public sealed class Arm64InstructionGroup : InstructionGroup<Arm64InstructionGroupId> {
        /// <summary>
        ///     Create an ARM64 Instruction Group.
        /// </summary>
        /// <param name="disassembler">
        ///     A disassembler.
        /// </param>
        /// <param name="id">
        ///     The instruction group's unique identifier.
        /// </param>
        /// <returns>
        ///     An ARM64 instruction group.
        /// </returns>
        internal static Arm64InstructionGroup Create(CapstoneDisassembler disassembler, Arm64InstructionGroupId id) {
            // ...
            //
            // Throws an exception if the operation fails.
            var name = NativeCapstone.GetInstructionGroupName(disassembler.Handle, (int) id);

            var @object = new Arm64InstructionGroup(id, name);
            return @object;
        }

        /// <summary>
        ///     Create an ARM64 Instruction Group.
        /// </summary>
        public Arm64InstructionGroup(Arm64InstructionGroupId id, string name) : base(id, name) { }
    }
}