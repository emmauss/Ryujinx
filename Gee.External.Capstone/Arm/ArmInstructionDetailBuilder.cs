namespace Gee.External.Capstone.Arm {
    /// <summary>
    ///     ARM Instruction Detail Builder.
    /// </summary>
    internal sealed class ArmInstructionDetailBuilder : InstructionDetailBuilder<ArmInstructionDetail, ArmDisassembleMode, ArmInstructionGroup, ArmInstructionGroupId, ArmInstruction, ArmInstructionId, ArmRegister, ArmRegisterId> {
        /// <summary>
        ///     Get and Set Condition Code.
        /// </summary>
        internal ArmConditionCode ConditionCode { get; private set; }

        /// <summary>
        ///     Get and Set CPS Flag.
        /// </summary>
        internal ArmCpsFlag CpsFlag { get; private set; }

        /// <summary>
        ///     Get and Set CPS Mode.
        /// </summary>
        internal ArmCpsMode CpsMode { get; private set; }

        /// <summary>
        ///     Get and Set User Mode Flag.
        /// </summary>
        internal bool IsUserMode { get; private set; }

        /// <summary>
        ///     Get and Set Memory Barrier Operation.
        /// </summary>
        internal ArmMemoryBarrierOperation MemoryBarrierOperation { get; private set; }

        /// <summary>
        ///     Get and Set Instruction's Operands.
        /// </summary>
        internal ArmOperand[] Operands { get; private set; }

        /// <summary>
        ///     Get and Set Update Flags Flag.
        /// </summary>
        internal bool UpdateFlags { get; private set; }

        /// <summary>
        ///     Get and Set Vector Data Type.
        /// </summary>
        internal ArmVectorDataType VectorDataType { get; private set; }

        /// <summary>
        ///     Get and Set Vector Size.
        /// </summary>
        internal int VectorSize { get; private set; }

        /// <summary>
        ///     Get and Set Write Back Flag.
        /// </summary>
        internal bool WriteBack { get; private set; }

        /// <summary>
        ///     Build an Instruction Detail.
        /// </summary>
        /// <param name="disassembler">
        ///     A disassembler.
        /// </param>
        /// <param name="hInstruction">
        ///     An instruction handle.
        /// </param>
        internal override void Build(CapstoneDisassembler disassembler, NativeInstructionHandle hInstruction) {
            // ...
            //
            // Throws an exception if the operation fails.
            base.Build(disassembler, hInstruction);
            var nativeInstructionDetail = NativeCapstone.GetInstructionDetail<NativeArmInstructionDetail>(hInstruction).GetValueOrDefault();

            this.ConditionCode = nativeInstructionDetail.ConditionCode;
            this.CpsFlag = nativeInstructionDetail.CpsFlag;
            this.CpsMode = nativeInstructionDetail.CpsMode;
            this.IsUserMode = nativeInstructionDetail.IsUserMode;
            this.MemoryBarrierOperation = nativeInstructionDetail.MemoryBarrierOperation;
            this.Operands = ArmOperand.Create(disassembler, ref nativeInstructionDetail);
            this.UpdateFlags = nativeInstructionDetail.UpdateFlags;
            this.VectorDataType = nativeInstructionDetail.VectorDataType;
            this.VectorSize = nativeInstructionDetail.VectorSize;
            this.WriteBack = nativeInstructionDetail.WriteBack;
        }

        /// <summary>
        ///     Create an ARM Instruction Detail.
        /// </summary>
        /// <returns>
        ///     An ARM instruction detail.
        /// </returns>
        internal ArmInstructionDetail Create() {
            return new ArmInstructionDetail(this);
        }

        /// <summary>
        ///     Create Disassemble Mode.
        /// </summary>
        /// <param name="nativeDisassembleMode">
        ///     A native disassemble mode.
        /// </param>
        /// <returns>
        ///     A disassemble mode.
        /// </returns>
        private protected override ArmDisassembleMode CreateDisassembleMode(NativeDisassembleMode nativeDisassembleMode) {
            return (ArmDisassembleMode) nativeDisassembleMode;
        }

        /// <summary>
        ///     Create an Instruction Group.
        /// </summary>
        /// <param name="disassembler">
        ///     A disassembler.
        /// </param>
        /// <param name="instructionGroupId">
        ///     An instruction group's unique identifier.
        /// </param>
        /// <returns>
        ///     An ARM instruction group.
        /// </returns>
        private protected override ArmInstructionGroup CreateInstructionGroup(CapstoneDisassembler disassembler, byte instructionGroupId) {
            return ArmInstructionGroup.Create(disassembler, (ArmInstructionGroupId) instructionGroupId);
        }

        /// <summary>
        ///     Create a Register.
        /// </summary>
        /// <param name="disassembler">
        ///     A disassembler.
        /// </param>
        /// <param name="registerId">
        ///     A register's unique identifier.
        /// </param>
        /// <returns>
        ///     An ARM register.
        /// </returns>
        private protected override ArmRegister CreateRegister(CapstoneDisassembler disassembler, short registerId) {
            return ArmRegister.TryCreate(disassembler, (ArmRegisterId) registerId);
        }
    }
}