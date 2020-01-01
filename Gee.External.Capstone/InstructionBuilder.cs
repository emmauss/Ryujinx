using System;

namespace Gee.External.Capstone {
    /// <summary>
    ///     Disassembled Instruction Builder.
    /// </summary>
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
    /// <typeparam name="TInstruction">
    ///     The type of the instruction.
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
    internal abstract class InstructionBuilder<TDetail, TDisassembleMode, TGroup, TGroupId, TInstruction, TId, TRegister, TRegisterId>
        where TDetail : InstructionDetail<TDetail, TDisassembleMode, TGroup, TGroupId, TInstruction, TId, TRegister, TRegisterId>
        where TDisassembleMode : Enum
        where TGroup : InstructionGroup<TGroupId>
        where TGroupId : Enum
        where TInstruction : Instruction<TInstruction, TDetail, TDisassembleMode, TGroup, TGroupId, TId, TRegister, TRegisterId>
        where TId : Enum
        where TRegister : Register<TRegisterId>
        where TRegisterId : Enum {
        /// <summary>
        ///     Get and Set Instruction's Address (EIP).
        /// </summary>
        internal long Address { get; private set; }

        /// <summary>
        ///     Get and Set Instruction's Machine Bytes.
        /// </summary>
        internal byte[] Bytes { get; private set; }

        /// <summary>
        ///     Get and Set Instruction's Details.
        /// </summary>
        internal TDetail Details { get; private set; }

        /// <summary>
        ///     Get and Set Instruction's Disassemble Architecture.
        /// </summary>
        internal DisassembleArchitecture DisassembleArchitecture { get; private set; }

        /// <summary>
        ///     Get and Set Instruction's Disassemble Mode.
        /// </summary>
        internal TDisassembleMode DisassembleMode { get; private set; }

        /// <summary>
        ///     Get and Set Instruction's Unique Identifier.
        /// </summary>
        internal TId Id { get; private set; }

        /// <summary>
        ///     Determine if Instruction is Skipped Data.
        /// </summary>
        internal bool IsSkippedData { get; private set; }

        /// <summary>
        ///     Get and Set Instruction's Mnemonic.
        /// </summary>
        internal string Mnemonic { get; private set; }

        /// <summary>
        ///     Get and Set Instruction's Operand Text.
        /// </summary>
        internal string Operand { get; private set; }

        /// <summary>
        ///     Create an Instruction Builder.
        /// </summary>
        internal InstructionBuilder() {
            this.Address = 0;
            this.Bytes = new byte[0];
            this.Details = null;
            this.Id = default;
            this.IsSkippedData = false;
            this.Mnemonic = null;
            this.Operand = null;
        }

        /// <summary>
        ///     Build an Instruction.
        /// </summary>
        /// <param name="disassembler">
        ///     A disassembler.
        /// </param>
        /// <param name="hInstruction">
        ///     An instruction handle.
        /// </param>
        internal virtual void Build(CapstoneDisassembler disassembler, NativeInstructionHandle hInstruction) {
            // ...
            //
            // Throws an exception if the operation fails.
            var nativeInstruction = NativeCapstone.GetInstruction(hInstruction);

            this.Address = nativeInstruction.Address;
            this.DisassembleArchitecture = disassembler.DisassembleArchitecture;
            this.DisassembleMode = this.CreateDisassembleMode(disassembler.NativeDisassembleMode);
            this.Id = this.CreateId(nativeInstruction.Id);
            this.IsSkippedData = disassembler.EnableSkipDataMode && !(nativeInstruction.Id > 0);
            this.Mnemonic = !CapstoneDisassembler.IsDietModeEnabled ? nativeInstruction.Mnemonic : null;
            this.Operand = !CapstoneDisassembler.IsDietModeEnabled ? nativeInstruction.Operand : null;
            // ...
            //
            // ...
            SetBytes(this, ref nativeInstruction);
            SetDetails(this, disassembler, hInstruction, ref nativeInstruction);

            // <summary>
            //      Set Instruction's Machine Bytes.
            // </summary>
            void SetBytes(InstructionBuilder<TDetail, TDisassembleMode, TGroup, TGroupId, TInstruction, TId, TRegister, TRegisterId> @this, ref NativeInstruction cNativeInstruction) {
                @this.Bytes = new byte[0];
                if (cNativeInstruction.Id >= 0) {
                    @this.Bytes = new byte[cNativeInstruction.Size];
                    for (var cI = 0; cI < @this.Bytes.Length; cI++) {
                        @this.Bytes[cI] = cNativeInstruction.Bytes[cI];
                    }
                }
            }

            // <summary>
            //      Set Instruction's Details.
            // </summary>
            void SetDetails(InstructionBuilder<TDetail, TDisassembleMode, TGroup, TGroupId, TInstruction, TId, TRegister, TRegisterId> @this, CapstoneDisassembler cDisassembler, NativeInstructionHandle cHInstruction, ref NativeInstruction cNativeInstruction) {
                var cHasDetails = cNativeInstruction.Details != IntPtr.Zero;
                var cIsInstructionDetailsEnabled = cDisassembler.EnableInstructionDetails;
                @this.Details = null;
                if (cHasDetails && cIsInstructionDetailsEnabled && cNativeInstruction.Id > 0) {
                    @this.Details = @this.CreateDetails(cDisassembler, cHInstruction);
                }
            }
        }

        /// <summary>
        ///     Create Instruction's Details.
        /// </summary>
        /// <param name="disassembler">
        ///     A disassembler.
        /// </param>
        /// <param name="hInstruction">
        ///     An instruction handle.
        /// </param>
        /// <returns>
        ///     The instruction's details.
        /// </returns>
        private protected abstract TDetail CreateDetails(CapstoneDisassembler disassembler, NativeInstructionHandle hInstruction);

        /// <summary>
        ///     Create Disassemble Mode.
        /// </summary>
        /// <param name="nativeDisassembleMode">
        ///     A native disassemble mode.
        /// </param>
        /// <returns>
        ///     A disassemble mode.
        /// </returns>
        private protected abstract TDisassembleMode CreateDisassembleMode(NativeDisassembleMode nativeDisassembleMode);

        /// <summary>
        ///     Create Instruction's Unique Identifier.
        /// </summary>
        /// <param name="id">
        ///     An instruction's unique identifier.
        /// </param>
        /// <returns>
        ///     The instruction's unique identifier.
        /// </returns>
        private protected abstract TId CreateId(int id);
    }
}