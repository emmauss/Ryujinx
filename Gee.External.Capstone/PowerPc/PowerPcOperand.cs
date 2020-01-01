using System;

namespace Gee.External.Capstone.PowerPc {
    /// <summary>
    ///     PowerPC Operand.
    /// </summary>
    public sealed class PowerPcOperand {
        /// <summary>
        ///     Condition Register Value.
        /// </summary>
        private readonly PowerPcConditionRegisterOperandValue _conditionRegister;

        /// <summary>
        ///     Immediate Value.
        /// </summary>
        private readonly long _immediate;

        /// <summary>
        ///     Memory Value.
        /// </summary>
        private readonly PowerPcMemoryOperandValue _memory;

        /// <summary>
        ///     Register Value.
        /// </summary>
        private readonly PowerPcRegister _register;

        /// <summary>
        ///     Get Condition Register Value.
        /// </summary>
        /// <remarks>
        ///     Represents the operand's condition register value if, and only if, the operand's type is
        ///     <see cref="PowerPcOperandType.ConditionRegister" />. To determine the operand's type, call
        ///     <see cref="Type" />.
        /// </remarks>
        /// <exception cref="System.InvalidOperationException">
        ///     Thrown if the operand's type is not <see cref="PowerPcOperandType.ConditionRegister" />.
        /// </exception>
        public PowerPcConditionRegisterOperandValue ConditionRegister {
            get {
                if (this.Type != PowerPcOperandType.ConditionRegister) {
                    const string valueName = nameof(PowerPcOperand.ConditionRegister);
                    var detailMessage = $"A value ({valueName}) is invalid when the type is ({this.Type}).";
                    throw new InvalidOperationException(detailMessage);
                }

                return this._conditionRegister;
            }
        }

        /// <summary>
        ///     Get Immediate Value.
        /// </summary>
        /// <remarks>
        ///     Represents the operand's immediate value if, and only if, the operand's type is
        ///     <see cref="PowerPcOperandType.Immediate" />. To determine the operand's type, call
        ///     <see cref="Type" />.
        /// </remarks>
        /// <exception cref="System.InvalidOperationException">
        ///     Thrown if the operand's type is not <see cref="PowerPcOperandType.Immediate" />.
        /// </exception>
        public long Immediate {
            get {
                if (this.Type != PowerPcOperandType.Immediate) {
                    const string valueName = nameof(PowerPcOperand.Immediate);
                    var detailMessage = $"A value ({valueName}) is invalid when the type is ({this.Type}).";
                    throw new InvalidOperationException(detailMessage);
                }

                return this._immediate;
            }
        }

        /// <summary>
        ///     Get Memory Value.
        /// </summary>
        /// <remarks>
        ///     Represents the operand's memory value if, and only if, the operand's type is
        ///     <see cref="PowerPcOperandType.Memory" />. To determine the operand's type, call <see cref="Type" />.
        /// </remarks>
        /// <exception cref="System.InvalidOperationException">
        ///     Thrown if the operand's type is not <see cref="PowerPcOperandType.Memory" />.
        /// </exception>
        public PowerPcMemoryOperandValue Memory {
            get {
                if (this.Type != PowerPcOperandType.Memory) {
                    const string valueName = nameof(PowerPcOperand.Memory);
                    var detailMessage = $"A value ({valueName}) is invalid when the type is ({this.Type}).";
                    throw new InvalidOperationException(detailMessage);
                }

                return this._memory;
            }
        }

        /// <summary>
        ///     Get Register Value.
        /// </summary>
        /// <remarks>
        ///     Represents the operand's register value if, and only if, the operand's type is
        ///     <see cref="PowerPcOperandType.Register" />. To determine the operand's type, call <see cref="Type" />.
        /// </remarks>
        /// <exception cref="System.InvalidOperationException">
        ///     Thrown if the operand's type is not <see cref="PowerPcOperandType.Register" />.
        /// </exception>
        public PowerPcRegister Register {
            get {
                if (this.Type != PowerPcOperandType.Register) {
                    const string valueName = nameof(PowerPcOperand.Register);
                    var detailMessage = $"A value ({valueName}) is invalid when the type is ({this.Type}).";
                    throw new InvalidOperationException(detailMessage);
                }

                return this._register;
            }
        }

        /// <summary>
        ///     Get Operand's Type.
        /// </summary>
        public PowerPcOperandType Type { get; }

        /// <summary>
        ///     Create PowerPC Operands.
        /// </summary>
        /// <param name="disassembler">
        ///     A disassembler.
        /// </param>
        /// <param name="nativeInstructionDetail">
        ///     A native PowerPC instruction detail.
        /// </param>
        /// <returns>
        ///     An array of PowerPC operands.
        /// </returns>
        internal static PowerPcOperand[] Create(CapstoneDisassembler disassembler, ref NativePowerPcInstructionDetail nativeInstructionDetail) {
            var operands = new PowerPcOperand[nativeInstructionDetail.OperandCount];
            for (var i = 0; i < operands.Length; i++) {
                ref var nativeOperand = ref nativeInstructionDetail.Operands[i];
                operands[i] = new PowerPcOperand(disassembler, ref nativeOperand);
            }

            return operands;
        }

        /// <summary>
        ///     Create a PowerPC Operand.
        /// </summary>
        /// <param name="disassembler">
        ///     A disassembler.
        /// </param>
        /// <param name="nativeOperand">
        ///     A native PowerPC operand.
        /// </param>
        internal PowerPcOperand(CapstoneDisassembler disassembler, ref NativePowerPcOperand nativeOperand) {
            this.Type = nativeOperand.Type;
            // ...
            //
            // ...
            switch (this.Type) {
                case PowerPcOperandType.ConditionRegister:
                    this._conditionRegister = new PowerPcConditionRegisterOperandValue(disassembler, ref nativeOperand.Value.ConditionRegister);
                    break;
                case PowerPcOperandType.Immediate:
                    this._immediate = nativeOperand.Value.Immediate;
                    break;
                case PowerPcOperandType.Memory:
                    this._memory = new PowerPcMemoryOperandValue(disassembler, ref nativeOperand.Value.Memory);
                    break;
                case PowerPcOperandType.Register:
                    this._register = PowerPcRegister.TryCreate(disassembler, nativeOperand.Value.Register);
                    break;
            }
        }
    }
}