using System;

namespace Gee.External.Capstone.XCore {
    /// <summary>
    ///     XCore Operand.
    /// </summary>
    public sealed class XCoreOperand {
        /// <summary>
        ///     Immediate Value.
        /// </summary>
        private readonly int _immediate;

        /// <summary>
        ///     Memory Value.
        /// </summary>
        private readonly XCoreMemoryOperandValue _memory;

        /// <summary>
        ///     Register Value.
        /// </summary>
        private readonly XCoreRegister _register;

        /// <summary>
        ///     Get Immediate Value.
        /// </summary>
        /// <remarks>
        ///     Represents the operand's immediate value if, and only if, the operand's type is
        ///     <see cref="XCoreOperandType.Immediate" />. To determine the operand's type, call <see cref="Type" />.
        /// </remarks>
        /// <exception cref="System.InvalidOperationException">
        ///     Thrown if the operand's type is not <see cref="XCoreOperandType.Immediate" />.
        /// </exception>
        public long Immediate {
            get {
                if (this.Type != XCoreOperandType.Immediate) {
                    const string valueName = nameof(XCoreOperand.Immediate);
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
        ///     <see cref="XCoreOperandType.Memory" />. To determine the operand's type, call <see cref="Type" />.
        /// </remarks>
        /// <exception cref="System.InvalidOperationException">
        ///     Thrown if the operand's type is not <see cref="XCoreOperandType.Memory" />.
        /// </exception>
        public XCoreMemoryOperandValue Memory {
            get {
                if (this.Type != XCoreOperandType.Memory) {
                    const string valueName = nameof(XCoreOperand.Memory);
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
        ///     <see cref="XCoreOperandType.Register" />. To determine the operand's type, call <see cref="Type" />.
        /// </remarks>
        /// <exception cref="System.InvalidOperationException">
        ///     Thrown if the operand's type is not <see cref="XCoreOperandType.Register" />.
        /// </exception>
        public XCoreRegister Register {
            get {
                if (this.Type != XCoreOperandType.Register) {
                    const string valueName = nameof(XCoreOperand.Register);
                    var detailMessage = $"A value ({valueName}) is invalid when the type is ({this.Type}).";
                    throw new InvalidOperationException(detailMessage);
                }

                return this._register;
            }
        }

        /// <summary>
        ///     Get Operand's Type.
        /// </summary>
        public XCoreOperandType Type { get; }

        /// <summary>
        ///     Create XCore Operands.
        /// </summary>
        /// <param name="disassembler">
        ///     A disassembler.
        /// </param>
        /// <param name="nativeInstructionDetail">
        ///     A native XCore instruction detail.
        /// </param>
        /// <returns>
        ///     An array of XCore operands.
        /// </returns>
        internal static XCoreOperand[] Create(CapstoneDisassembler disassembler, ref NativeXCoreInstructionDetail nativeInstructionDetail) {
            var operands = new XCoreOperand[nativeInstructionDetail.OperandCount];
            for (var i = 0; i < operands.Length; i++) {
                ref var nativeOperand = ref nativeInstructionDetail.Operands[i];
                operands[i] = new XCoreOperand(disassembler, ref nativeOperand);
            }

            return operands;
        }

        /// <summary>
        ///     Create an XCore Operand.
        /// </summary>
        /// <param name="disassembler">
        ///     A disassembler.
        /// </param>
        /// <param name="nativeOperand">
        ///     A native XCore operand.
        /// </param>
        internal XCoreOperand(CapstoneDisassembler disassembler, ref NativeXCoreOperand nativeOperand) {
            this.Type = nativeOperand.Type;
            // ...
            //
            // ...
            switch (this.Type) {
                case XCoreOperandType.Immediate:
                    this._immediate = nativeOperand.Value.Immediate;
                    break;
                case XCoreOperandType.Memory:
                    this._memory = new XCoreMemoryOperandValue(disassembler, ref nativeOperand.Value.Memory);
                    break;
                case XCoreOperandType.Register:
                    this._register = XCoreRegister.TryCreate(disassembler, nativeOperand.Value.Register);
                    break;
            }
        }
    }
}