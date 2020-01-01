using System;

namespace Gee.External.Capstone {
    /// <summary>
    ///     Disassembled Instruction.
    /// </summary>
    /// <typeparam name="TSelf">
    ///     This type.
    /// </typeparam>
    /// <typeparam name="TDetail">
    ///     The type of the instruction's details.
    /// </typeparam>
    /// <typeparam name="TDisassembleMode">
    ///     The type of the hardware mode the instruction was disassembled for.
    /// </typeparam>
    /// <typeparam name="TGroup">
    ///     The type of the instruction's architecture specific instruction groups.
    /// </typeparam>
    /// <typeparam name="TGroupId">
    ///     The type of the instruction's architecture specific instruction group unique identifiers.
    /// </typeparam>
    /// <typeparam name="TId">
    ///     The type of the instruction's unique identifier.
    /// </typeparam>
    /// <typeparam name="TRegister">
    ///     The type of the instruction's architecture specific registers.
    /// </typeparam>
    /// <typeparam name="TRegisterId">
    ///     The type of the instruction's architecture specific register unique identifiers.
    /// </typeparam>
    public abstract class Instruction<TSelf, TDetail, TDisassembleMode, TGroup, TGroupId, TId, TRegister, TRegisterId>
        where TSelf : Instruction<TSelf, TDetail, TDisassembleMode, TGroup, TGroupId, TId, TRegister, TRegisterId>
        where TDetail : InstructionDetail<TDetail, TDisassembleMode, TGroup, TGroupId, TSelf, TId, TRegister, TRegisterId>
        where TDisassembleMode : Enum
        where TGroup : InstructionGroup<TGroupId>
        where TGroupId : Enum
        where TId : Enum
        where TRegister : Register<TRegisterId>
        where TRegisterId : Enum {
        /// <summary>
        ///     Instruction's Details.
        /// </summary>
        private readonly TDetail _details;

        /// <summary>
        ///     Instruction's Mnemonic.
        /// </summary>
        private readonly string _mnemonic;

        /// <summary>
        ///     Instruction's Operand Text.
        /// </summary>
        private readonly string _operand;

        /// <summary>
        ///     Get Instruction's Address (EIP).
        /// </summary>
        public long Address { get; }

        /// <summary>
        ///     Get Instruction's Machine Bytes.
        /// </summary>
        public byte[] Bytes { get; }

        /// <summary>
        ///     Get Instruction's Details.
        /// </summary>
        /// <remarks>
        ///     Represents the instruction's details if, and only if, the instruction was disassembled with details. A
        ///     null reference otherwise. To determine if the instruction was disassembled with details, call
        ///     <see cref="HasDetails" />. 
        /// </remarks>
        /// <exception cref="System.InvalidOperationException">
        ///     Thrown if the instruction was disassembled with no details.
        /// </exception>
        public TDetail Details {
            get {
                if (this._details == null) {
                    const string detailMessage = "An operation is invalid.";
                    throw new InvalidOperationException(detailMessage);
                }

                return this._details;
            }
        }

        /// <summary>
        ///     Get Instruction's Disassemble Architecture.
        /// </summary>
        /// <remarks>
        ///     Represents the hardware architecture the instruction was disassembled for.
        /// </remarks>
        public DisassembleArchitecture DisassembleArchitecture { get; }

        /// <summary>
        ///     Get Instruction's Disassemble Mode.
        /// </summary>
        /// <remarks>
        ///     Represents the hardware mode the instruction was disassembled for.
        /// </remarks>
        public TDisassembleMode DisassembleMode { get; }

        /// <summary>
        ///     Determine if Instruction Has Details.
        /// </summary>
        /// <remarks>
        ///     Indicates if the instruction was disassembled with details. A boolean true indicates the instruction
        ///     was disassembled with details. A boolean false otherwise. If the instruction was disassembled without
        ///     details, it is either because instruction details were disabled on the disassembler or instruction
        ///     details and Skip Data Mode were enabled on the disassembler and the instruction is a skipped data
        ///     instruction. To determine if the instruction is a skipped data instruction, call
        ///     <see cref="IsSkippedData" />.
        /// </remarks>
        public bool HasDetails => this._details != null;

        /// <summary>
        ///     Get Instruction's Unique Identifier.
        /// </summary>
        public TId Id { get; }

        /// <summary>
        ///     Determine if Diet Mode is Enabled.
        /// </summary>
        /// <remarks>
        ///     Indicates if Diet Mode is enabled. A boolean true indicates it is enabled. A boolean false otherwise.
        /// </remarks>
        public bool IsDietModeEnabled => CapstoneDisassembler.IsDietModeEnabled;

        /// <summary>
        ///     Determine if Instruction is Skipped Data.
        /// </summary>
        /// <remarks>
        ///     Indicates if the instruction is a skipped data instruction. A boolean true indicates the instruction
        ///     is a skipped data instruction. A boolean false otherwise.
        /// </remarks>
        public bool IsSkippedData { get; }

        /// <summary>
        ///     Get Instruction's Mnemonic.
        /// </summary>
        /// <remarks>
        ///     Represents the instruction's mnemonic if, and only if, Diet Mode is disabled. To determine if Diet
        ///     Mode is disabled, call <see cref="IsDietModeEnabled" />.
        /// </remarks>
        /// <exception cref="System.NotSupportedException">
        ///     Thrown if Diet Mode is enabled.
        /// </exception>
        public string Mnemonic {
            get {
                CapstoneDisassembler.ThrowIfDietModeIsEnabled();
                return this._mnemonic;
            }
        }

        /// <summary>
        ///     Get Instruction's Operand Text.
        /// </summary>
        /// <remarks>
        ///     Represents the instruction's operand text if, and only if, Diet Mode is disabled. To determine if Diet
        ///     Mode is disabled, call <see cref="IsDietModeEnabled" />.
        /// </remarks>
        /// <exception cref="System.NotSupportedException">
        ///     Thrown if Diet Mode is enabled.
        /// </exception>
        public string Operand {
            get {
                CapstoneDisassembler.ThrowIfDietModeIsEnabled();
                return this._operand;
            }
        }

        /// <summary>
        ///     Create a Disassembled Instruction.
        /// </summary>
        /// <param name="builder">
        ///     A builder to initialize the object with.
        /// </param>
        private protected Instruction(InstructionBuilder<TDetail, TDisassembleMode, TGroup, TGroupId, TSelf, TId, TRegister, TRegisterId> builder) {
            this.Address = builder.Address;
            this.Bytes = builder.Bytes;
            this._details = builder.Details;
            this.DisassembleArchitecture = builder.DisassembleArchitecture;
            this.DisassembleMode = builder.DisassembleMode;
            this.Id = builder.Id;
            this.IsSkippedData = builder.IsSkippedData;
            this._mnemonic = builder.Mnemonic;
            this._operand = builder.Operand;
        }
    }
}