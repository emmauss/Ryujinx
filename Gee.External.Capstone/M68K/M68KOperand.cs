using System;

namespace Gee.External.Capstone.M68K {
    /// <summary>
    ///     M68K Operand.
    /// </summary>
    public sealed class M68KOperand {
        /// <summary>
        ///     Branch Displacement Value.
        /// </summary>
        private readonly M68KBranchDisplacementOperandValue _branchDisplacement;

        /// <summary>
        ///     Double Precision Immediate Value.
        /// </summary>
        private readonly double _dImmediate;

        /// <summary>
        ///     Immediate Value.
        /// </summary>
        private readonly long _immediate;

        /// <summary>
        ///     Memory Value.
        /// </summary>
        private readonly M68KMemoryOperandValue _memory;

        /// <summary>
        ///     Register Value.
        /// </summary>
        private readonly M68KRegister _register;

        /// <summary>
        ///     Register Bits Value.
        /// </summary>
        private readonly int _registerBits;

        /// <summary>
        ///     Register Pair Value.
        /// </summary>
        private readonly Tuple<M68KRegister, M68KRegister> _registerPair;

        /// <summary>
        ///     Single Precision Immediate Value.
        /// </summary>
        private readonly float _sImmediate;

        /// <summary>
        ///     Address Mode.
        /// </summary>
        public M68KAddressMode AddressMode { get; }

        /// <summary>
        ///     Get Branch Displacement Value.
        /// </summary>
        /// <remarks>
        ///     Represents the operand's branch displacement value if, and only if, the operand's type is
        ///     <see cref="M68KOperandType.BranchDisplacement" />. To determine the operand's type, call
        ///     <see cref="Type" />.
        /// </remarks>
        /// <exception cref="System.InvalidOperationException">
        ///     Thrown if the operand's type is to <see cref="M68KOperandType.BranchDisplacement" />.
        /// </exception>
        public M68KBranchDisplacementOperandValue BranchDisplacement {
            get {
                if (this.Type != M68KOperandType.BranchDisplacement) {
                    const string valueName = nameof(M68KOperand.BranchDisplacement);
                    var detailMessage = $"A value ({valueName}) is invalid when the type is ({this.Type}).";
                    throw new InvalidOperationException(detailMessage);
                }

                return this._branchDisplacement;
            }
        }

        /// <summary>
        ///     Get Double Precision Immediate Value.
        /// </summary>
        /// <remarks>
        ///     Represents the operand's double precision immediate value if, and only if, the operand's type is
        ///     <see cref="M68KOperandType.DImmediate" />. To determine the operand's type, call <see cref="Type" />.
        /// </remarks>
        /// <exception cref="System.InvalidOperationException">
        ///     Thrown if the operand's type is not <see cref="M68KOperandType.DImmediate" />.
        /// </exception>
        public double DImmediate {
            get {
                if (this.Type != M68KOperandType.DImmediate) {
                    const string valueName = nameof(M68KOperand.DImmediate);
                    var detailMessage = $"A value ({valueName}) is invalid when the type is ({this.Type}).";
                    throw new InvalidOperationException(detailMessage);
                }

                return this._dImmediate;
            }
        }

        /// <summary>
        ///     Get Immediate Value.
        /// </summary>
        /// <remarks>
        ///     Represents the operand's immediate value if, and only if, the operand's type is
        ///     <see cref="M68KOperandType.Immediate" />. To determine the operand's type, call <see cref="Type" />.
        /// </remarks>
        /// <exception cref="System.InvalidOperationException">
        ///     Thrown if the operand's type is not <see cref="M68KOperandType.Immediate" />.
        /// </exception>
        public long Immediate {
            get {
                if (this.Type != M68KOperandType.Immediate) {
                    const string valueName = nameof(M68KOperand.Immediate);
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
        ///     <see cref="M68KOperandType.Memory" />. To determine the operand's type, call <see cref="Type" />.
        /// </remarks>
        /// <exception cref="System.InvalidOperationException">
        ///     Thrown if the operand's type is not <see cref="M68KOperandType.Memory" />.
        /// </exception>
        public M68KMemoryOperandValue Memory {
            get {
                if (this.Type != M68KOperandType.Memory) {
                    const string valueName = nameof(M68KOperand.Memory);
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
        ///     <see cref="M68KOperandType.Register" />. To determine the operand's type, call <see cref="Type" />.
        /// </remarks>
        /// <exception cref="System.InvalidOperationException">
        ///     Thrown if the operand's type is not <see cref="M68KOperandType.Register" />.
        /// </exception>
        public M68KRegister Register {
            get {
                if (this.Type != M68KOperandType.Register) {
                    const string valueName = nameof(M68KOperand.Register);
                    var detailMessage = $"A value ({valueName}) is invalid when the type is ({this.Type}).";
                    throw new InvalidOperationException(detailMessage);
                }

                return this._register;
            }
        }

        /// <summary>
        ///     Get Register Bits Value.
        /// </summary>
        /// <remarks>
        ///     Represents the operand's register bits value if, and only if, the operand's type is
        ///     <see cref="M68KOperandType.RegisterBits" />. To determine the operand's type, call <see cref="Type" />.
        /// </remarks>
        /// <exception cref="System.InvalidOperationException">
        ///     Thrown if the operand's type is not <see cref="M68KOperandType.RegisterBits" />.
        /// </exception>
        public int RegisterBits {
            get {
                if (this.Type != M68KOperandType.RegisterBits) {
                    const string valueName = nameof(M68KOperand.RegisterBits);
                    var detailMessage = $"A value ({valueName}) is invalid when the type is ({this.Type}).";
                    throw new InvalidOperationException(detailMessage);
                }

                return this._registerBits;
            }
        }

        /// <summary>
        ///     Get Register Pair Value.
        /// </summary>
        /// <remarks>
        ///     Represents a 2-tuple of the operand's register pair value if, and only if, the operand's type is
        ///     <see cref="M68KOperandType.RegisterPair" />. To determine the operand's type, call
        ///     <see cref="Type" />.
        /// </remarks>
        /// <exception cref="System.InvalidOperationException">
        ///     Thrown if the operand's type is not <see cref="M68KOperandType.RegisterPair" />.
        /// </exception>
        public Tuple<M68KRegister, M68KRegister> RegisterPair {
            get {
                if (this.Type != M68KOperandType.RegisterPair) {
                    const string valueName = nameof(M68KOperand.RegisterPair);
                    var detailMessage = $"A value ({valueName}) is invalid when the type is ({this.Type}).";
                    throw new InvalidOperationException(detailMessage);
                }

                return this._registerPair;
            }
        }

        /// <summary>
        ///     Get Single Precision Immediate Value.
        /// </summary>
        /// <remarks>
        ///     Represents the operand's single precision immediate value if, and only if, the operand's type is
        ///     <see cref="M68KOperandType.SImmediate" />. To determine the operand's type, call <see cref="Type" />.
        /// </remarks>
        /// <exception cref="System.InvalidOperationException">
        ///     Thrown if the operand's type is not <see cref="M68KOperandType.SImmediate" />.
        /// </exception>
        public float SImmediate {
            get {
                if (this.Type != M68KOperandType.SImmediate) {
                    const string valueName = nameof(M68KOperand.SImmediate);
                    var detailMessage = $"A value ({valueName}) is invalid when the type is ({this.Type}).";
                    throw new InvalidOperationException(detailMessage);
                }

                return this._sImmediate;
            }
        }

        /// <summary>
        ///     Operand's Type.
        /// </summary>
        /// <remarks>
        ///     Represents the operand's type.
        /// </remarks>
        public M68KOperandType Type { get; }

        /// <summary>
        ///     Create M68K Operands.
        /// </summary>
        /// <param name="disassembler">
        ///     A disassembler.
        /// </param>
        /// <param name="nativeInstructionDetail">
        ///     A native M68K instruction detail.
        /// </param>
        /// <returns>
        ///     An array of M68K operands.
        /// </returns>
        internal static M68KOperand[] Create(CapstoneDisassembler disassembler, ref NativeM68KInstructionDetail nativeInstructionDetail) {
            var operands = new M68KOperand[nativeInstructionDetail.OperandCount];
            for (var i = 0; i < operands.Length; i++) {
                ref var nativeOperand = ref nativeInstructionDetail.Operands[i];
                operands[i] = new M68KOperand(disassembler, ref nativeOperand);
            }

            return operands;
        }

        /// <summary>
        ///     Create an M68K Operand.
        /// </summary>
        /// <param name="disassembler">
        ///     A disassembler.
        /// </param>
        /// <param name="nativeOperand">
        ///     A native M68K operand.
        /// </param>
        internal M68KOperand(CapstoneDisassembler disassembler, ref NativeM68KOperand nativeOperand) {
            this.AddressMode = nativeOperand.AddressMode;
            this.Type = nativeOperand.Type;
            // ...
            //
            // ...
            switch (this.Type) {
                case M68KOperandType.BranchDisplacement:
                    this._branchDisplacement = new M68KBranchDisplacementOperandValue(ref nativeOperand.BranchDisplacement);
                    break;
                case M68KOperandType.DImmediate:
                    this._dImmediate = nativeOperand.Value.DImmediate;
                    break;
                case M68KOperandType.Immediate:
                    this._immediate = nativeOperand.Value.Immediate;
                    break;
                case M68KOperandType.Memory:
                    this._memory = new M68KMemoryOperandValue(disassembler, ref nativeOperand.Memory);
                    break;
                case M68KOperandType.Register:
                    this._register = M68KRegister.TryCreate(disassembler, nativeOperand.Value.Register);
                    break;
                case M68KOperandType.RegisterBits:
                    this._registerBits = nativeOperand.RegisterBits;
                    break;
                case M68KOperandType.RegisterPair:
                    var register0 = M68KRegister.TryCreate(disassembler, nativeOperand.Value.RegisterPair.Register0);
                    var register1 = M68KRegister.TryCreate(disassembler, nativeOperand.Value.RegisterPair.Register1);
                    this._registerPair = Tuple.Create(register0, register1);
                    break;
                case M68KOperandType.SImmediate:
                    this._sImmediate = nativeOperand.Value.SImmediate;
                    break;
            }
        }
    }
}