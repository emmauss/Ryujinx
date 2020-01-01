using System;

namespace Gee.External.Capstone.X86 {
    /// <summary>
    ///     X86 Operand.
    /// </summary>
    public sealed class X86Operand {
        /// <summary>
        ///     Operand's Access Type.
        /// </summary>
        private readonly OperandAccessType _accessType;

        /// <summary>
        ///     Immediate Value.
        /// </summary>
        private readonly long _immediate;

        /// <summary>
        ///     Memory Value.
        /// </summary>
        private readonly X86MemoryOperandValue _memory;

        /// <summary>
        ///     Register Value.
        /// </summary>
        private readonly X86Register _register;

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
        ///     Get AVX Broadcast.
        /// </summary>
        public X86AvxBroadcast AvxBroadcast { get; }

        /// <summary>
        ///     Get AVX Zero Opmask.
        /// </summary>
        public bool AvxZeroOpMask { get; }

        /// <summary>
        ///     Get Immediate Value.
        /// </summary>
        /// <remarks>
        ///     Represents the operand's immediate value if, and only if, the operand's type is
        ///     <see cref="X86OperandType.Immediate" />. To determine the operand's type, call <see cref="Type" />.
        /// </remarks>
        /// <exception cref="System.InvalidOperationException">
        ///     Thrown if the operand's type is not <see cref="X86OperandType.Immediate" />.
        /// </exception>
        public long Immediate {
            get {
                if (this.Type != X86OperandType.Immediate) {
                    const string valueName = nameof(X86Operand.Immediate);
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
        ///     <see cref="X86OperandType.Memory" />. To determine the operand's type, call <see cref="Type" />.
        /// </remarks>
        /// <exception cref="System.InvalidOperationException">
        ///     Thrown if the operand's type is not <see cref="X86OperandType.Memory" />.
        /// </exception>
        public X86MemoryOperandValue Memory {
            get {
                if (this.Type != X86OperandType.Memory) {
                    const string valueName = nameof(X86Operand.Memory);
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
        ///     <see cref="X86OperandType.Register" />. To determine the operand's type, call <see cref="Type" />.
        /// </remarks>
        /// <exception cref="System.InvalidOperationException">
        ///     Thrown if the operand's type is not <see cref="X86OperandType.Register" />.
        /// </exception>
        public X86Register Register {
            get {
                if (this.Type != X86OperandType.Register) {
                    const string valueName = nameof(X86Operand.Register);
                    var detailMessage = $"A value ({valueName}) is invalid when the type is ({this.Type}).";
                    throw new InvalidOperationException(detailMessage);
                }

                return this._register;
            }
        }

        /// <summary>
        ///     Get Operand's Size.
        /// </summary>
        public byte Size { get; }

        /// <summary>
        ///     Get Operand's Type.
        /// </summary>
        public X86OperandType Type { get; }

        /// <summary>
        ///     Create X86 Operands.
        /// </summary>
        /// <param name="disassembler">
        ///     A disassembler.
        /// </param>
        /// <param name="nativeInstructionDetail">
        ///     A native X86 instruction detail.
        /// </param>
        /// <returns>
        ///     An array of X86 operands.
        /// </returns>
        internal static X86Operand[] Create(CapstoneDisassembler disassembler, ref NativeX86InstructionDetail nativeInstructionDetail) {
            var operands = new X86Operand[nativeInstructionDetail.OperandCount];
            for (var i = 0; i < operands.Length; i++) {
                ref var nativeOperand = ref nativeInstructionDetail.Operands[i];
                operands[i] = new X86Operand(disassembler, ref nativeOperand);
            }

            return operands;
        }

        /// <summary>
        ///     Create an X86 Operand.
        /// </summary>
        /// <param name="disassembler">
        ///     A disassembler.
        /// </param>
        /// <param name="nativeOperand">
        ///     A native X86 operand.
        /// </param>
        internal X86Operand(CapstoneDisassembler disassembler, ref NativeX86Operand nativeOperand) {
            this._accessType = !CapstoneDisassembler.IsDietModeEnabled ? nativeOperand.AccessType : OperandAccessType.Invalid;
            this.AvxBroadcast = nativeOperand.AvxBroadcast;
            this.AvxZeroOpMask = nativeOperand.AvxZeroOpMask;
            this.Size = nativeOperand.Size;
            this.Type = nativeOperand.Type;
            // ...
            //
            // ...
            switch (this.Type) {
                case X86OperandType.Immediate:
                    this._immediate = nativeOperand.Value.Immediate;
                    break;
                case X86OperandType.Memory:
                    this._memory = new X86MemoryOperandValue(disassembler, ref nativeOperand.Value.Memory);
                    break;
                case X86OperandType.Register:
                    this._register = X86Register.TryCreate(disassembler, nativeOperand.Value.Register);
                    break;
            }
        }
    }
}