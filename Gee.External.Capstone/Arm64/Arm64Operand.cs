using System;

namespace Gee.External.Capstone.Arm64 {
    /// <summary>
    ///     ARM64 Operand.
    /// </summary>
    public sealed class Arm64Operand {
        /// <summary>
        ///     Operand's Access Type.
        /// </summary>
        private readonly OperandAccessType _accessType;

        /// <summary>
        ///     Address Translation (AT) Operation.
        /// </summary>
        private readonly Arm64AtOperation _atOperation;

        /// <summary>
        ///     Barrier Operation.
        /// </summary>
        private readonly Arm64BarrierOperation _barrierOperation;

        /// <summary>
        ///     Data Cache (DC) Operation.
        /// </summary>
        private readonly Arm64DcOperation _dcOperation;

        /// <summary>
        ///     Floating Point Value.
        /// </summary>
        private readonly double _floatingPoint;

        /// <summary>
        ///     Instruction Cache (IC) Operation.
        /// </summary>
        private readonly Arm64IcOperation _icOperation;

        /// <summary>
        ///     Immediate Value.
        /// </summary>
        private readonly long _immediate;

        /// <summary>
        ///     Memory Value.
        /// </summary>
        private readonly Arm64MemoryOperandValue _memory;

        /// <summary>
        ///     MRS System Register Value.
        /// </summary>
        private readonly Arm64MrsSystemRegister _mrsSystemRegister;

        /// <summary>
        ///     MSR System Register Value.
        /// </summary>
        private readonly Arm64MsrSystemRegister _msrSystemRegister;

        /// <summary>
        ///     Prefetch Operation.
        /// </summary>
        private readonly Arm64PrefetchOperation _prefetchOperation;

        /// <summary>
        ///     Processor State (PSTATE) Field.
        /// </summary>
        private readonly Arm64PStateField _pStateField;

        /// <summary>
        ///     Register Value.
        /// </summary>
        private readonly Arm64Register _register;

        /// <summary>
        ///     Shift Value.
        /// </summary>
        private readonly int _shiftValue;

        /// <summary>
        ///     Translation Lookaside Buffer (TLBI) Operation.
        /// </summary>
        private readonly Arm64TlbiOperation _tlbiOperation;

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
        ///     Get Address Translation (AT) Operation.
        /// </summary>
        /// <remarks>
        ///     Represents the operand's Address Translation (AT) operation if, and only if, the operand's type is
        ///     <see cref="Arm64OperandType.AtOperation" />. To determine the operand's type, call
        ///     <see cref="Type" />.
        /// </remarks>
        /// <exception cref="System.InvalidOperationException">
        ///     Thrown if the operand's type is not <see cref="Arm64OperandType.AtOperation" />.
        /// </exception>
        public Arm64AtOperation AtOperation {
            get {
                if (this.Type != Arm64OperandType.AtOperation) {
                    const string valueName = nameof(Arm64Operand.AtOperation);
                    var detailMessage = $"A value ({valueName}) is invalid when the type is ({this.Type}).";
                    throw new InvalidOperationException(detailMessage);
                }

                return this._atOperation;
            }
        }

        /// <summary>
        ///     Get Barrier Operation.
        /// </summary>
        /// <remarks>
        ///     Represents the operand's barrier operation if, and only if, the operand's type is
        ///     <see cref="Arm64OperandType.BarrierOperation" />. To determine the operand's type, call
        ///     <see cref="Type" />.
        /// </remarks>
        /// <exception cref="System.InvalidOperationException">
        ///     Thrown if the operand's type is not <see cref="Arm64OperandType.BarrierOperation" />.
        /// </exception>
        public Arm64BarrierOperation BarrierOperation {
            get {
                if (this.Type != Arm64OperandType.BarrierOperation) {
                    const string valueName = nameof(Arm64Operand.BarrierOperation);
                    var detailMessage = $"A value ({valueName}) is invalid when the type is ({this.Type}).";
                    throw new InvalidOperationException(detailMessage);
                }

                return this._barrierOperation;
            }
        }

        /// <summary>
        ///     Get Data Cache (DC) Operation.
        /// </summary>
        /// <remarks>
        ///     Represents the operand's Data Cache (DC) operation if, and only if, the operand's type is
        ///     <see cref="Arm64OperandType.DcOperation" />. To determine the operand's type, call
        ///     <see cref="Type" />.
        /// </remarks>
        /// <exception cref="System.InvalidOperationException">
        ///     Thrown if the operand's type is not <see cref="Arm64OperandType.DcOperation" />.
        /// </exception>
        public Arm64DcOperation DcOperation {
            get {
                if (this.Type != Arm64OperandType.DcOperation) {
                    const string valueName = nameof(Arm64Operand.DcOperation);
                    var detailMessage = $"A value ({valueName}) is invalid when the type is ({this.Type}).";
                    throw new InvalidOperationException(detailMessage);
                }

                return this._dcOperation;
            }
        }

        /// <summary>
        ///     Get Extend Operation.
        /// </summary>
        public Arm64ExtendOperation ExtendOperation { get; }

        /// <summary>
        ///     Get Floating Point Value.
        /// </summary>
        /// <remarks>
        ///     Represents the operand's floating point value if, and only if, the operand's type is
        ///     <see cref="Arm64OperandType.FloatingPoint" />. To determine the operand's type, call
        ///     <see cref="Type" />.
        /// </remarks>
        /// <exception cref="System.InvalidOperationException">
        ///     Thrown if the operand's type is not <see cref="Arm64OperandType.FloatingPoint" />.
        /// </exception>
        public double FloatingPoint {
            get {
                if (this.Type != Arm64OperandType.FloatingPoint) {
                    const string valueName = nameof(Arm64Operand.FloatingPoint);
                    var detailMessage = $"A value ({valueName}) is invalid when the type is ({this.Type}).";
                    throw new InvalidOperationException(detailMessage);
                }

                return this._floatingPoint;
            }
        }

        /// <summary>
        ///     Get Instruction Cache (IC) Operation.
        /// </summary>
        /// <remarks>
        ///     Represents the operand's Instruction Cache (IC) operation if, and only if, the operand's type is
        ///     <see cref="Arm64OperandType.IcOperation" />. To determine the operand's type, call
        ///     <see cref="Type" />.
        /// </remarks>
        /// <exception cref="System.InvalidOperationException">
        ///     Thrown if the operand's type is not <see cref="Arm64OperandType.IcOperation" />.
        /// </exception>
        public Arm64IcOperation IcOperation {
            get {
                if (this.Type != Arm64OperandType.IcOperation) {
                    const string valueName = nameof(Arm64Operand.IcOperation);
                    var detailMessage = $"A value ({valueName}) is invalid when the type is ({this.Type}).";
                    throw new InvalidOperationException(detailMessage);
                }

                return this._icOperation;
            }
        }

        /// <summary>
        ///     Get Immediate Value.
        /// </summary>
        /// <remarks>
        ///     Represents the operand's immediate value if, and only if, the operand's type is
        ///     <see cref="Arm64OperandType.Immediate" />. To determine the operand's type, call <see cref="Type" />.
        /// </remarks>
        /// <exception cref="System.InvalidOperationException">
        ///     Thrown if the operand's type is not <see cref="Arm64OperandType.Immediate" />.
        /// </exception>
        public long Immediate {
            get {
                if (this.Type != Arm64OperandType.CImmediate && this.Type != Arm64OperandType.Immediate) {
                    const string valueName = nameof(Arm64Operand.Immediate);
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
        ///     Get Memory Value.
        /// </summary>
        /// <remarks>
        ///     Represents the operand's memory value if, and only if, the operand's type is
        ///     <see cref="Arm64OperandType.Memory" />. To determine the operand's type, call <see cref="Type" />.
        /// </remarks>
        /// <exception cref="System.InvalidOperationException">
        ///     Thrown if the operand's type is not <see cref="Arm64OperandType.Memory" />.
        /// </exception>
        public Arm64MemoryOperandValue Memory {
            get {
                if (this.Type != Arm64OperandType.Memory) {
                    const string valueName = nameof(Arm64Operand.Memory);
                    var detailMessage = $"A value ({valueName}) is invalid when the type is ({this.Type}).";
                    throw new InvalidOperationException(detailMessage);
                }

                return this._memory;
            }
        }

        /// <summary>
        ///     Get MRS System Register Value.
        /// </summary>
        /// <remarks>
        ///     Represents the operand's MRS system register value if, and only if, the operand's type is
        ///     <see cref="Arm64OperandType.MrsSystemRegister" />. To determine the operand's type, call
        ///     <see cref="Type" />.
        /// </remarks>
        /// <exception cref="System.InvalidOperationException">
        ///     Thrown if the operand's type is not <see cref="Arm64OperandType.MrsSystemRegister" />.
        /// </exception>
        public Arm64MrsSystemRegister MrsSystemRegister {
            get {
                if (this.Type != Arm64OperandType.MrsSystemRegister) {
                    const string valueName = nameof(Arm64Operand.MrsSystemRegister);
                    var detailMessage = $"A value ({valueName}) is invalid when the type is ({this.Type}).";
                    throw new InvalidOperationException(detailMessage);
                }

                return this._mrsSystemRegister;
            }
        }

        /// <summary>
        ///     Get MSR System Register Value.
        /// </summary>
        /// <remarks>
        ///     Represents the operand's MRS system register value if, and only if, the operand's type is
        ///     <see cref="Arm64OperandType.MsrSystemRegister" />. To determine the operand's type, call
        ///     <see cref="Type" />.
        /// </remarks>
        /// <exception cref="System.InvalidOperationException">
        ///     Thrown if the operand's type is not <see cref="Arm64OperandType.MsrSystemRegister" />.
        /// </exception>
        public Arm64MsrSystemRegister MsrSystemRegister {
            get {
                if (this.Type != Arm64OperandType.MsrSystemRegister) {
                    const string valueName = nameof(Arm64Operand.MsrSystemRegister);
                    var detailMessage = $"A value ({valueName}) is invalid when the type is ({this.Type}).";
                    throw new InvalidOperationException(detailMessage);
                }

                return this._msrSystemRegister;
            }
        }

        /// <summary>
        ///     Get Prefetch Operation.
        /// </summary>
        /// <remarks>
        ///     Represents the operand's prefetch operation if, and only if, the operand's type is
        ///     <see cref="Arm64OperandType.PrefetchOperation" />. To determine the operand's type, call
        ///     <see cref="Type" />.
        /// </remarks>
        /// <exception cref="System.InvalidOperationException">
        ///     Thrown if the operand's type is not <see cref="Arm64OperandType.PrefetchOperation" />.
        /// </exception>
        public Arm64PrefetchOperation PrefetchOperation {
            get {
                if (this.Type != Arm64OperandType.PrefetchOperation) {
                    const string valueName = nameof(Arm64Operand.PrefetchOperation);
                    var detailMessage = $"A value ({valueName}) is invalid when the type is ({this.Type}).";
                    throw new InvalidOperationException(detailMessage);
                }

                return this._prefetchOperation;
            }
        }

        /// <summary>
        ///     Get Processor State (PSTATE) Field.
        /// </summary>
        /// <remarks>
        ///     Represents the operand's processor state (PSTATE) field if, and only if, the operand's type is
        ///     <see cref="Arm64OperandType.PStateField" />. To determine the operand's type, call
        ///     <see cref="Type" />.
        /// </remarks>
        /// <exception cref="System.InvalidOperationException">
        ///     Thrown if the operand's type is not <see cref="Arm64OperandType.PStateField" />.
        /// </exception>
        public Arm64PStateField PStateField {
            get {
                if (this.Type != Arm64OperandType.PStateField) {
                    const string valueName = nameof(Arm64Operand.PStateField);
                    var detailMessage = $"A value ({valueName}) is invalid when the type is ({this.Type}).";
                    throw new InvalidOperationException(detailMessage);
                }

                return this._pStateField;
            }
        }

        /// <summary>
        ///     Get Register Value.
        /// </summary>
        /// <remarks>
        ///     Represents the operand's register value if, and only if, the operand's type is
        ///     <see cref="Arm64OperandType.Register" />. To determine the operand's type, call <see cref="Type" />.
        /// </remarks>
        /// <exception cref="System.InvalidOperationException">
        ///     Thrown if the operand's type is not <see cref="Arm64OperandType.Register" />.
        /// </exception>
        public Arm64Register Register {
            get {
                if (this.Type != Arm64OperandType.Register) {
                    const string valueName = nameof(Arm64Operand.Register);
                    var detailMessage = $"A value ({valueName}) is invalid when the type is ({this.Type}).";
                    throw new InvalidOperationException(detailMessage);
                }

                return this._register;
            }
        }

        /// <summary>
        ///     Get Shift Operation.
        /// </summary>
        public Arm64ShiftOperation ShiftOperation { get; }

        /// <summary>
        ///     Get Shift Value.
        /// </summary>
        /// <remarks>
        ///     Represents the operand's shift value if, and only if, the operand's shift operation is not
        ///     <see cref="Arm64ShiftOperation.Invalid" />. To determine the operand's shift operation, call
        ///     <see cref="ShiftOperation" />.
        /// </remarks>
        /// <exception cref="System.InvalidOperationException">
        ///     Thrown if the shift operation is <see cref="Arm64ShiftOperation.Invalid" />.
        /// </exception>
        public int ShiftValue {
            get {
                if (this.ShiftOperation == Arm64ShiftOperation.Invalid) {
                    const string valueName = nameof(Arm64Operand.ShiftValue);
                    var detailMessage = $"A value ({valueName}) is invalid when the type is ({this.ShiftOperation}).";
                    throw new InvalidOperationException(detailMessage);
                }

                return this._shiftValue;
            }
        }

        /// <summary>
        ///     Get Translation Lookaside Buffer (TLBI) Operation.
        /// </summary>
        /// <remarks>
        ///     Represents the operand's Translation Lookaside Buffer (TLBI) operation if, and only if, the operand's
        ///     type is <see cref="Arm64OperandType.TlbiOperation" />. To determine the operand's type, call
        ///     <see cref="Type" />.
        /// </remarks>
        /// <exception cref="System.InvalidOperationException">
        ///     Thrown if the operand's type is not <see cref="Arm64OperandType.TlbiOperation" />.
        /// </exception>
        public Arm64TlbiOperation TlbiOperation {
            get {
                if (this.Type != Arm64OperandType.TlbiOperation) {
                    const string valueName = nameof(Arm64Operand.TlbiOperation);
                    var detailMessage = $"A value ({valueName}) is invalid when the type is ({this.Type}).";
                    throw new InvalidOperationException(detailMessage);
                }

                return this._tlbiOperation;
            }
        }

        /// <summary>
        ///     Get Operand's Type.
        /// </summary>
        public Arm64OperandType Type { get; }

        /// <summary>
        ///     Get Vector Arrangement Specifier.
        /// </summary>
        public Arm64VectorArrangementSpecifier VectorArrangementSpecifier { get; }

        /// <summary>
        ///     Get Vector Element Size Specifier.
        /// </summary>
        public Arm64VectorElementSizeSpecifier VectorElementSizeSpecifier { get; }

        /// <summary>
        ///     Get Vector Index.
        /// </summary>
        public int VectorIndex { get; }

        /// <summary>
        ///     Create ARM64 Operands.
        /// </summary>
        /// <param name="disassembler">
        ///     A disassembler.
        /// </param>
        /// <param name="instructionId">
        ///     An instruction's unique identifier.
        /// </param>
        /// <param name="nativeInstructionDetail">
        ///     A native ARM64 instruction detail.
        /// </param>
        /// <returns>
        ///     An array of ARM64 operands.
        /// </returns>
        internal static Arm64Operand[] Create(CapstoneDisassembler disassembler, Arm64InstructionId instructionId, ref NativeArm64InstructionDetail nativeInstructionDetail) {
            var operands = new Arm64Operand[nativeInstructionDetail.OperandCount];
            for (var i = 0; i < operands.Length; i++) {
                ref var nativeOperand = ref nativeInstructionDetail.Operands[i];
                operands[i] = new Arm64Operand(disassembler, instructionId, ref nativeOperand);
            }

            return operands;
        }

        /// <summary>
        ///     Create an ARM64 Operand.
        /// </summary>
        /// <param name="disassembler">
        ///     A disassembler.
        /// </param>
        /// <param name="instructionId">
        ///     An instruction's unique identifier.
        /// </param>
        /// <param name="nativeOperand">
        ///     A native ARM64 operand.
        /// </param>
        internal Arm64Operand(CapstoneDisassembler disassembler, Arm64InstructionId instructionId, ref NativeArm64Operand nativeOperand) {
            this._accessType = !CapstoneDisassembler.IsDietModeEnabled ? nativeOperand.AccessType : OperandAccessType.Invalid;
            this.ExtendOperation = nativeOperand.ExtendOperation;
            this.ShiftOperation = nativeOperand.Shift.Operation;
            this.Type = nativeOperand.Type;
            this.VectorArrangementSpecifier = nativeOperand.VectorArrangementSpecifier;
            this.VectorElementSizeSpecifier = nativeOperand.VectorElementSizeSpecifier;
            this.VectorIndex = nativeOperand.VectorIndex;
            // ...
            //
            // ...
            this._shiftValue = CreateShiftValue(this, ref nativeOperand);
            // ...
            //
            // ...
            switch (this.Type) {
                case Arm64OperandType.BarrierOperation:
                    this._barrierOperation = nativeOperand.Value.BarrierOperation;
                    break;
                case Arm64OperandType.CImmediate:
                    this._immediate = nativeOperand.Value.Immediate;
                    break;
                case Arm64OperandType.FloatingPoint:
                    this._floatingPoint = nativeOperand.Value.FloatingPoint;
                    break;
                case Arm64OperandType.Immediate:
                    this._immediate = nativeOperand.Value.Immediate;
                    break;
                case Arm64OperandType.Memory:
                    this._memory = new Arm64MemoryOperandValue(disassembler, ref nativeOperand.Value.Memory);
                    break;
                case Arm64OperandType.MrsSystemRegister:
                    this._mrsSystemRegister = (Arm64MrsSystemRegister) nativeOperand.Value.Register;
                    break;
                case Arm64OperandType.MsrSystemRegister:
                    this._msrSystemRegister = (Arm64MsrSystemRegister) nativeOperand.Value.Register;
                    break;
                case Arm64OperandType.PrefetchOperation:
                    this._prefetchOperation = nativeOperand.Value.PrefetchOperation;
                    break;
                case Arm64OperandType.PStateField:
                    this._pStateField = nativeOperand.Value.PStateField;
                    break;
                case Arm64OperandType.Register:
                    this._register = Arm64Register.TryCreate(disassembler, nativeOperand.Value.Register);
                    break;
                case Arm64OperandType.SystemOperation:
                    switch (instructionId) {
                        case Arm64InstructionId.ARM64_INS_AT:
                            this._atOperation = (Arm64AtOperation) nativeOperand.Value.SystemOperation;
                            this.Type = Arm64OperandType.AtOperation;
                            break;
                        case Arm64InstructionId.ARM64_INS_DC:
                            this._dcOperation = (Arm64DcOperation) nativeOperand.Value.SystemOperation;
                            this.Type = Arm64OperandType.DcOperation;
                            break;
                        case Arm64InstructionId.ARM64_INS_IC:
                            this._icOperation = (Arm64IcOperation) nativeOperand.Value.SystemOperation;
                            this.Type = Arm64OperandType.IcOperation;
                            break;
                        case Arm64InstructionId.ARM64_INS_TLBI:
                            this._tlbiOperation = (Arm64TlbiOperation) nativeOperand.Value.SystemOperation;
                            this.Type = Arm64OperandType.TlbiOperation;
                            break;
                    }

                    break;
            }

            // <summary>
            //      Create Shift Value.
            // </summary>
            int CreateShiftValue(Arm64Operand @this, ref NativeArm64Operand cNativeOperand) {
                var cShiftValue = 0;
                if (@this.ShiftOperation != Arm64ShiftOperation.Invalid) {
                    cShiftValue = cNativeOperand.Shift.Value;
                }

                return cShiftValue;
            }
        }
    }
}