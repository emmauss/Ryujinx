namespace Gee.External.Capstone.Arm {
    /// <summary>
    ///     ARM Instruction Detail.
    /// </summary>
    public sealed class ArmInstructionDetail : InstructionDetail<ArmInstructionDetail, ArmDisassembleMode, ArmInstructionGroup, ArmInstructionGroupId, ArmInstruction, ArmInstructionId, ArmRegister, ArmRegisterId> {
        /// <summary>
        ///     Get Condition Code.
        /// </summary>
        public ArmConditionCode ConditionCode { get; }

        /// <summary>
        ///     Get CPS Flag.
        /// </summary>
        public ArmCpsFlag CpsFlag { get; }

        /// <summary>
        ///     Get CPS Mode.
        /// </summary>
        public ArmCpsMode CpsMode { get; }

        /// <summary>
        ///     Get User Mode Flag.
        /// </summary>
        public bool IsUserMode { get; }

        /// <summary>
        ///     Get Memory Barrier Operation.
        /// </summary>
        public ArmMemoryBarrierOperation MemoryBarrierOperation { get; }

        /// <summary>
        ///     Get Instruction's Operands.
        /// </summary>
        public ArmOperand[] Operands { get; }

        /// <summary>
        ///     Get Update Flags Flag.
        /// </summary>
        public bool UpdateFlags { get; }

        /// <summary>
        ///     Get Vector Data Type.
        /// </summary>
        public ArmVectorDataType VectorDataType { get; }

        /// <summary>
        ///     Get Vector Size.
        /// </summary>
        public int VectorSize { get; }

        /// <summary>
        ///     Get Write Back Flag.
        /// </summary>
        public bool WriteBack { get; }

        /// <summary>
        ///     Create an ARM Instruction Detail.
        /// </summary>
        /// <param name="disassembler">
        ///     A disassembler.
        /// </param>
        /// <param name="hInstruction">
        ///     An instruction handle.
        /// </param>
        /// <returns>
        ///     An ARM instruction detail.
        /// </returns>
        internal static ArmInstructionDetail Create(CapstoneDisassembler disassembler, NativeInstructionHandle hInstruction) {
            var builder = new ArmInstructionDetailBuilder();
            builder.Build(disassembler, hInstruction);

            return builder.Create();
        }

        /// <summary>
        ///     Create an ARM Instruction Detail.
        /// </summary>
        /// <param name="builder">
        ///     A builder to initialize the object with.
        /// </param>
        internal ArmInstructionDetail(ArmInstructionDetailBuilder builder) : base(builder) {
            this.ConditionCode = builder.ConditionCode;
            this.CpsFlag = builder.CpsFlag;
            this.CpsMode = builder.CpsMode;
            this.MemoryBarrierOperation = builder.MemoryBarrierOperation;
            this.IsUserMode = builder.IsUserMode;
            this.Operands = builder.Operands;
            this.UpdateFlags = builder.UpdateFlags;
            this.VectorDataType = builder.VectorDataType;
            this.VectorSize = builder.VectorSize;
            this.WriteBack = builder.WriteBack;
        }
    }
}