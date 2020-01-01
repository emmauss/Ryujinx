using System;

namespace Gee.External.Capstone.Arm {
    /// <summary>
    ///     ARM Operand.
    /// </summary>
    public sealed class ArmOperand {
        /// <summary>
        ///     Operand's Access Type.
        /// </summary>
        private readonly OperandAccessType _accessType;

        /// <summary>
        ///     Floating Point Value.
        /// </summary>
        private readonly double _floatingPoint;

        /// <summary>
        ///     Immediate Value.
        /// </summary>
        private readonly int _immediate;

        /// <summary>
        ///     Memory Value.
        /// </summary>
        private readonly ArmMemoryOperandValue _memory;

        /// <summary>
        ///     Register Value.
        /// </summary>
        private readonly ArmRegister _register;

        /// <summary>
        ///     SETEND Operation.
        /// </summary>
        private readonly ArmSetEndOperation _setEndOperation;

        /// <summary>
        ///     Shift Register.
        /// </summary>
        private readonly ArmRegister _shiftRegister;

        /// <summary>
        ///     Shift Value.
        /// </summary>
        private readonly int _shiftValue;

        /// <summary>
        ///     System Register Value.
        /// </summary>
        private readonly ArmSystemRegister _systemRegister;

        /// <summary>
        ///     Get Operand's Access Type.
        /// </summary>
        /// <remarks>
        ///     Represents the operand's access type if, and only if, Diet Mode is disabled. To determine if Diet Mode
        ///     is disabled, call <see cref="IsDietModeEnabled" />.
        /// </remarks>
        /// <exception cref="System.NotSupportedException">
        ///     Thrown if Diet Mode is enabled.
        /// </exception>
        public OperandAccessType AccessType {
            get {
                CapstoneDisassembler.ThrowIfDietModeIsEnabled();
                return this._accessType;
            }
        }

        /// <summary>
        ///     Get Floating Point Value.
        /// </summary>
        /// <remarks>
        ///     Represents the operand's floating point value if, and only if, the operand's type is
        ///     <see cref="ArmOperandType.FloatingPoint" />. To determine the operand's type, call
        ///     <see cref="Type" />.
        /// </remarks>
        /// <exception cref="System.InvalidOperationException">
        ///     Thrown if the operand's type is not <see cref="ArmOperandType.FloatingPoint" />.
        /// </exception>
        public double FloatingPoint {
            get {
                if (this.Type != ArmOperandType.FloatingPoint) {
                    const string valueName = nameof(ArmOperand.FloatingPoint);
                    var detailMessage = $"A value ({valueName}) is invalid when the type is ({this.Type}).";
                    throw new InvalidOperationException(detailMessage);
                }

                return this._floatingPoint;
            }
        }

        /// <summary>
        ///     Get Immediate Value.
        /// </summary>
        /// <remarks>
        ///     Represents the operand's immediate value if the operand's type is
        ///     <see cref="ArmOperandType.CImmediate" />, <see cref="ArmOperandType.Immediate" />, or
        ///     <see cref="ArmOperandType.PImmediate" />. To determine the operand's type, call <see cref="Type" />.
        /// </remarks>
        /// <exception cref="System.InvalidOperationException">
        ///     Thrown if the operand's type is not <see cref="ArmOperandType.CImmediate" />,
        ///     <see cref="ArmOperandType.Immediate" />, or <see cref="ArmOperandType.PImmediate" />.
        /// </exception>
        public int Immediate {
            get {
                var isTypeImmediate = this.Type == ArmOperandType.CImmediate;
                isTypeImmediate = isTypeImmediate || this.Type == ArmOperandType.Immediate;
                isTypeImmediate = isTypeImmediate || this.Type == ArmOperandType.PImmediate;
                if (!isTypeImmediate) {
                    const string valueName = nameof(ArmOperand.Immediate);
                    var detailMessage = $"A value ({valueName}) is invalid when the type is ({this.Type}).";
                    throw new InvalidOperationException(detailMessage);
                }

                return this._immediate;
            }
        }

        /// <summary>
        ///     Determine if Diet Mode is Enabled.
        /// </summary>
        /// <remarks>
        ///     Indicates if Diet Mode is enabled. A boolean true indicates it is enabled. A boolean false otherwise.
        /// </remarks>
        public bool IsDietModeEnabled => CapstoneDisassembler.IsDietModeEnabled;

        /// <summary>
        ///     Get Subtracted Flag.
        /// </summary>
        public bool IsSubtracted { get; }

        /// <summary>
        ///     Get Memory Value.
        /// </summary>
        /// <remarks>
        ///     Represents the operand's memory value if, and only if, the operand's type is
        ///     <see cref="ArmOperandType.Memory" />. To determine the operand's type, call <see cref="Type" />.
        /// </remarks>
        /// <exception cref="System.InvalidOperationException">
        ///     Thrown if the operand's type is not <see cref="ArmOperandType.Memory" />.
        /// </exception>
        public ArmMemoryOperandValue Memory {
            get {
                if (this.Type != ArmOperandType.Memory) {
                    const string valueName = nameof(ArmOperand.Memory);
                    var detailMessage = $"A value ({valueName}) is invalid when the type is ({this.Type}).";
                    throw new InvalidOperationException(detailMessage);
                }

                return this._memory;
            }
        }

        /// <summary>
        ///     Get Neon Lane Value.
        /// </summary>
        public sbyte NeonLane { get; }

        /// <summary>
        ///     Get Register Value.
        /// </summary>
        /// <remarks>
        ///     Represents the operand's register value if, and only if, the operand's type is
        ///     <see cref="ArmOperandType.Register" />. To determine the operand's type, call <see cref="Type" />.
        /// </remarks>
        /// <exception cref="System.InvalidOperationException">
        ///     Thrown if the operand's type is not <see cref="ArmOperandType.Register" />.
        /// </exception>
        public ArmRegister Register {
            get {
                if (this.Type != ArmOperandType.Register) {
                    const string valueName = nameof(ArmOperand.Register);
                    var detailMessage = $"A value ({valueName}) is invalid when the type is ({this.Type}).";
                    throw new InvalidOperationException(detailMessage);
                }

                return this._register;
            }
        }

        /// <summary>
        ///     Get SETEND Operation.
        /// </summary>
        /// <remarks>
        ///     Represents the operand's SETEND operation if, and only if, the operand's type is
        ///     <see cref="ArmOperandType.SetEndOperation" />. To determine the operand's type, call
        ///     <see cref="Type" />.
        /// </remarks>
        /// <exception cref="System.InvalidOperationException">
        ///     Thrown if the operand's type is not <see cref="ArmOperandType.SetEndOperation" />.
        /// </exception>
        public ArmSetEndOperation SetEndOperation {
            get {
                if (this.Type != ArmOperandType.SetEndOperation) {
                    const string valueName = nameof(ArmOperand.SetEndOperation);
                    var detailMessage = $"A value ({valueName}) is invalid when the type is ({this.Type}).";
                    throw new InvalidOperationException(detailMessage);
                }

                return this._setEndOperation;
            }
        }

        /// <summary>
        ///     Get Shift Operation.
        /// </summary>
        public ArmShiftOperation ShiftOperation { get; }

        /// <summary>
        ///     Get Shift Register.
        /// </summary>
        /// <remarks>
        ///     Conveniently represents the operand's shift register if the operand's shift operation is not
        ///     <see cref="ArmShiftOperation.Invalid" /> and greater than or equal to
        ///     <see cref="ArmShiftOperation.ARM_SFT_ASR_REG" />. To determine the operand's shift operation,
        ///     call <see cref="ShiftOperation" />.
        /// </remarks>
        /// <exception cref="System.InvalidOperationException">
        ///     Thrown if the shift operation is equal to <see cref="ArmShiftOperation.Invalid" />, or if the shift
        ///     operation is less than <see cref="ArmShiftOperation.ARM_SFT_ASR_REG" />.
        /// </exception>
        public ArmRegister ShiftRegister {
            get {
                if (this.ShiftOperation == ArmShiftOperation.Invalid) {
                    const string valueName = nameof(ArmOperand.ShiftRegister);
                    var detailMessage = $"A value ({valueName}) is invalid when the type is ({this.ShiftOperation}).";
                    throw new InvalidOperationException(detailMessage);
                }

                if (!(this.ShiftOperation >= ArmShiftOperation.ARM_SFT_ASR_REG)) {
                    const string valueName = nameof(ArmOperand.ShiftRegister);
                    var detailMessage = $"A value ({valueName}) is invalid when the type is ({this.ShiftOperation}).";
                    throw new InvalidOperationException(detailMessage);
                }

                return this._shiftRegister;
            }
        }

        /// <summary>
        ///     Get Shift Value.
        /// </summary>
        /// <remarks>
        ///     Represents the operand's shift value if, and only if, the operand's shift operation is not
        ///     <see cref="ArmShiftOperation.Invalid" />. To determine the operand's shift operation, call
        ///     <see cref="ShiftOperation" />.
        /// </remarks>
        /// <exception cref="System.InvalidOperationException">
        ///     Thrown if the shift operation is <see cref="ArmShiftOperation.Invalid" />.
        /// </exception>
        public int ShiftValue {
            get {
                if (this.ShiftOperation == ArmShiftOperation.Invalid) {
                    const string valueName = nameof(ArmOperand.ShiftValue);
                    var detailMessage = $"A value ({valueName}) is invalid when the type is ({this.ShiftOperation}).";
                    throw new InvalidOperationException(detailMessage);
                }

                return this._shiftValue;
            }
        }

        /// <summary>
        ///     Get System Register.
        /// </summary>
        /// <remarks>
        ///     Represents the operand's system register if, and only if, the operand's type is
        ///     <see cref="ArmOperandType.SystemRegister" />. To determine the operand's type, call
        ///     <see cref="Type" />.
        /// </remarks>
        /// <exception cref="System.InvalidOperationException">
        ///     Thrown if the operand's type is not <see cref="ArmOperandType.SystemRegister" />.
        /// </exception>
        public ArmSystemRegister SystemRegister {
            get {
                if (this.Type != ArmOperandType.SystemRegister) {
                    const string valueName = nameof(ArmOperand.SystemRegister);
                    var detailMessage = $"A value ({valueName}) is invalid when the type is ({this.Type}).";
                    throw new InvalidOperationException(detailMessage);
                }

                return this._systemRegister;
            }
        }

        /// <summary>
        ///     Get Operand's Type.
        /// </summary>
        public ArmOperandType Type { get; }

        /// <summary>
        ///     Get Vector Index.
        /// </summary>
        public int VectorIndex { get; }

        /// <summary>
        ///     Create ARM Operands.
        /// </summary>
        /// <param name="disassembler">
        ///     A disassembler.
        /// </param>
        /// <param name="nativeInstructionDetail">
        ///     A native ARM instruction detail.
        /// </param>
        /// <returns>
        ///     An array of ARM operands.
        /// </returns>
        internal static ArmOperand[] Create(CapstoneDisassembler disassembler, ref NativeArmInstructionDetail nativeInstructionDetail) {
            var operands = new ArmOperand[nativeInstructionDetail.OperandCount];
            for (var i = 0; i < operands.Length; i++) {
                ref var nativeOperand = ref nativeInstructionDetail.Operands[i];
                operands[i] = new ArmOperand(disassembler, ref nativeOperand);
            }

            return operands;
        }

        /// <summary>
        ///     Create an ARM Operand.
        /// </summary>
        /// <param name="disassembler">
        ///     A disassembler.
        /// </param>
        /// <param name="nativeOperand">
        ///     A native ARM operand.
        /// </param>
        internal ArmOperand(CapstoneDisassembler disassembler, ref NativeArmOperand nativeOperand) {
            this._accessType = !CapstoneDisassembler.IsDietModeEnabled ? nativeOperand.AccessType : OperandAccessType.Invalid;
            this.IsSubtracted = nativeOperand.IsSubtracted;
            this.NeonLane = nativeOperand.NeonLane;
            this.ShiftOperation = nativeOperand.Shift.Operation;
            this.Type = nativeOperand.Type;
            this.VectorIndex = nativeOperand.VectorIndex;
            // ...
            //
            // ...
            this._shiftRegister = CreateShiftRegister(this, disassembler, ref nativeOperand);
            this._shiftValue = CreateShiftValue(this, ref nativeOperand);
            // ...
            //
            // ...
            switch (this.Type) {
                case ArmOperandType.CImmediate:
                    this._immediate = nativeOperand.Value.Immediate;
                    break;
                case ArmOperandType.FloatingPoint:
                    this._floatingPoint = nativeOperand.Value.FloatingPoint;
                    break;
                case ArmOperandType.Immediate:
                    this._immediate = nativeOperand.Value.Immediate;
                    break;
                case ArmOperandType.Memory:
                    this._memory = new ArmMemoryOperandValue(disassembler, ref nativeOperand.Value.Memory);
                    break;
                case ArmOperandType.PImmediate:
                    this._immediate = nativeOperand.Value.Immediate;
                    break;
                case ArmOperandType.Register:
                    this._register = ArmRegister.TryCreate(disassembler, (ArmRegisterId) nativeOperand.Value.Register);
                    break;
                case ArmOperandType.SetEndOperation:
                    this._setEndOperation = nativeOperand.Value.SetEndOperation;
                    break;
                case ArmOperandType.SystemRegister:
                    this._systemRegister = (ArmSystemRegister) nativeOperand.Value.Register;
                    break;
            }

            // <summary>
            //      Create Shift Value.
            // </summary>
            int CreateShiftValue(ArmOperand @this, ref NativeArmOperand cNativeOperand) {
                var cShiftValue = 0;
                if (@this.ShiftOperation != ArmShiftOperation.Invalid) {
                    cShiftValue = cNativeOperand.Shift.Value;
                }

                return cShiftValue;
            }

            // <summary>
            //      Create Shift Register.
            // </summary>
            ArmRegister CreateShiftRegister(ArmOperand @this, CapstoneDisassembler cDisassembler, ref NativeArmOperand cNativeArmOperand) {
                ArmRegister cShiftRegister = null;
                if (@this.ShiftOperation != ArmShiftOperation.Invalid) {
                    if (@this.ShiftOperation >= ArmShiftOperation.ARM_SFT_ASR_REG) {
                        cShiftRegister = ArmRegister.TryCreate(cDisassembler, (ArmRegisterId) cNativeArmOperand.Shift.Value);
                    }
                }

                return cShiftRegister;
            }
        }
    }
}