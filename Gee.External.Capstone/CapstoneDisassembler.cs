using Gee.External.Capstone.Arm;
using Gee.External.Capstone.Arm64;
using Gee.External.Capstone.M68K;
using Gee.External.Capstone.Mips;
using Gee.External.Capstone.PowerPc;
using Gee.External.Capstone.X86;
using Gee.External.Capstone.XCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gee.External.Capstone {
    /// <summary>
    ///     Capstone Disassembler.
    /// </summary>
    public abstract class CapstoneDisassembler : IDisposable {
        /// <summary>
        ///     Determine if the ARM64 Architecture is Supported.
        /// </summary>
        /// <remarks>
        ///     Indicates if the ARM64 architecture is supported. A boolean true indicates it is supported. A boolean
        ///     false otherwise.
        /// </remarks>
        public static bool IsArm64Supported {
            get {
                var value = NativeCapstone.Query(NativeQueryOption.QueryArm64Architecture);
                return value;
            }
        }

        /// <summary>
        ///     Determine if the ARM Architecture is Supported.
        /// </summary>
        /// <remarks>
        ///     Indicates if the ARM architecture is supported. A boolean true indicates it is supported. A boolean
        ///     false otherwise.
        /// </remarks>
        public static bool IsArmSupported {
            get {
                var value = NativeCapstone.Query(NativeQueryOption.QueryArmArchitecture);
                return value;
            }
        }

        /// <summary>
        ///     Determine if Diet Mode is Enabled.
        /// </summary>
        /// <remarks>
        ///     Indicates if Diet Mode is enabled. A boolean true indicates it is enabled. A boolean false otherwise.
        /// </remarks>
        public static bool IsDietModeEnabled {
            get {
                var value = NativeCapstone.Query(NativeQueryOption.QueryDietMode);
                return value;
            }
        }

        /// <summary>
        ///     Determine if the Ethereum EVM Architecture is Supported.
        /// </summary>
        /// <remarks>
        ///     Indicates if the Ethereum EVM architecture is supported. A boolean true indicates it is supported. A
        ///     boolean false otherwise.
        /// </remarks>
        internal static bool IsEvmSupported {
            get {
                var value = NativeCapstone.Query(NativeQueryOption.QueryEvmArchitecture);
                return value;
            }
        }

        /// <summary>
        ///     Determine if the M680X Architecture is Supported.
        /// </summary>
        /// <remarks>
        ///     Indicates if the M680X architecture is supported. A boolean true indicates it is supported. A boolean
        ///     false otherwise.
        /// </remarks>
        internal static bool IsM680XSupported {
            get {
                var value = NativeCapstone.Query(NativeQueryOption.QueryM680XArchitecture);
                return value;
            }
        }

        /// <summary>
        ///     Determine if the M68K Architecture is Supported.
        /// </summary>
        /// <remarks>
        ///     Indicates if the M68K architecture is supported. A boolean true indicates it is supported. A boolean
        ///     false otherwise.
        /// </remarks>
        public static bool IsM68KSupported {
            get {
                var value = NativeCapstone.Query(NativeQueryOption.QueryM68KArchitecture);
                return value;
            }
        }

        /// <summary>
        ///     Determine if the MIPS Architecture is Supported.
        /// </summary>
        /// <remarks>
        ///     Indicates if the MIPS architecture is supported. A boolean true indicates it is supported. A boolean
        ///     false otherwise.
        /// </remarks>
        public static bool IsMipsSupported {
            get {
                var value = NativeCapstone.Query(NativeQueryOption.QueryMipsArchitecture);
                return value;
            }
        }

        /// <summary>
        ///     Determine if the PowerPC Architecture is Supported.
        /// </summary>
        /// <remarks>
        ///     Indicates if the PowerPC architecture is supported. A boolean true indicates it is supported. A
        ///     boolean false otherwise.
        /// </remarks>
        public static bool IsPowerPcSupported {
            get {
                var value = NativeCapstone.Query(NativeQueryOption.QueryPowerPcArchitecture);
                return value;
            }
        }

        /// <summary>
        ///     Determine if the SPARC Architecture is Supported.
        /// </summary>
        /// <remarks>
        ///     Indicates if the SPARC architecture is supported. A boolean true indicates it is supported. A boolean
        ///     false otherwise.
        /// </remarks>
        internal static bool IsSparcSupported {
            get {
                var value = NativeCapstone.Query(NativeQueryOption.QuerySparcArchitecture);
                return value;
            }
        }

        /// <summary>
        ///     Determine if the SystemZ Architecture is Supported.
        /// </summary>
        /// <remarks>
        ///     Indicates if the SystemZ architecture is supported. A boolean true indicates it is supported. A
        ///     boolean false otherwise.
        /// </remarks>
        internal static bool IsSystemZSupported {
            get {
                var value = NativeCapstone.Query(NativeQueryOption.QuerySystemZArchitecture);
                return value;
            }
        }

        /// <summary>
        ///     Determine if the TMS320C64X Architecture is Supported.
        /// </summary>
        /// <remarks>
        ///     Indicates if the TMS320C64X architecture is supported. A boolean true indicates it is supported. A
        ///     boolean false otherwise.
        /// </remarks>
        internal static bool IsTms320C64XSupported {
            get {
                var value = NativeCapstone.Query(NativeQueryOption.QueryTms320C64XArchitecture);
                return value;
            }
        }

        /// <summary>
        ///     Determine if X86 Reduce Mode is Enabled.
        /// </summary>
        /// <remarks>
        ///     Indicates if X86 Reduce Mode is enabled. A boolean true indicates it is enabled. A boolean false
        ///     otherwise.
        /// </remarks>
        public static bool IsX86ReduceModeEnabled {
            get {
                var value = NativeCapstone.Query(NativeQueryOption.QueryX86ReduceMode);
                return value;
            }
        }

        /// <summary>
        ///     Determine if the X86 Architecture is Supported.
        /// </summary>
        /// <remarks>
        ///     Indicates if the X86 architecture is supported. A boolean true indicates it is supported. A boolean
        ///     false otherwise.
        /// </remarks>
        public static bool IsX86Supported {
            get {
                var value = NativeCapstone.Query(NativeQueryOption.QueryX86Architecture);
                return value;
            }
        }

        /// <summary>
        ///     Determine if the XCore Architecture is Supported.
        /// </summary>
        /// <remarks>
        ///     Indicates if the XCore architecture is supported. A boolean true indicates it is supported. A boolean
        ///     false otherwise.
        /// </remarks>
        public static bool IsXCoreSupported {
            get {
                var value = NativeCapstone.Query(NativeQueryOption.QueryXCoreArchitecture);
                return value;
            }
        }

        /// <summary>
        ///     Get Capstone Library's Version.
        /// </summary>
        /// <value>
        ///     The Capstone library's version.
        /// </value>
        public static Version Version {
            get {
                var value = NativeCapstone.GetVersion();
                return value;
            }
        }

        /// <summary>
        ///     Get Disassemble Architecture.
        /// </summary>
        /// <remarks>
        ///     Represents the disassembler's hardware architecture.
        /// </remarks>
        public abstract DisassembleArchitecture DisassembleArchitecture { get; }

        /// <summary>
        ///     Enable or Disable Instruction Details.
        /// </summary>
        /// <exception cref="Gee.External.Capstone.CapstoneException">
        ///     Thrown if the instruction details option could not be set.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        ///     Thrown if the disassembler is disposed.
        /// </exception>
        public abstract bool EnableInstructionDetails { get; set; }

        /// <summary>
        ///     Enable or Disable Skip Data Mode.
        /// </summary>
        /// <exception cref="Gee.External.Capstone.CapstoneException">
        ///     Thrown if the Skip Data Mode option could not be set.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        ///     Thrown if the disassembler is disposed.
        /// </exception>
        public abstract bool EnableSkipDataMode { get; set; }

        /// <summary>
        ///     Get Disassembler's Handle.
        /// </summary>
        /// <remarks>
        ///     Represents the disassembler's native handle.
        /// </remarks>
        internal abstract NativeDisassemblerHandle Handle { get; }

        /// <summary>
        ///     Get Native Disassemble Mode.
        /// </summary>
        /// <remarks>
        ///     Represents the disassembler's native hardware mode.
        /// </remarks>
        internal abstract NativeDisassembleMode NativeDisassembleMode { get; }

        /// <summary>
        ///     Get and Set Skip Data Instruction Mnemonic.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">
        ///     Thrown if the value is a null reference.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        ///     Thrown if the disassembler is disposed.
        /// </exception>
        public abstract string SkipDataInstructionMnemonic { get; set; }

        /// <summary>
        ///     Create an ARM64 Disassembler.
        /// </summary>
        /// <param name="disassembleMode">
        ///     The hardware mode for the disassembler to use.
        /// </param>
        /// <returns>
        ///     An ARM64 disassembler.
        /// </returns>
        /// <exception cref="Gee.External.Capstone.CapstoneException">
        ///     Thrown if a disassembler could not be created.
        /// </exception>
        /// <exception cref="System.OutOfMemoryException">
        ///     Thrown if sufficient memory cannot be allocated to perform the operation as a rare indication that the
        ///     system is under heavy load.
        /// </exception>
        public static CapstoneArm64Disassembler CreateArm64Disassembler(Arm64DisassembleMode disassembleMode) {
            return new CapstoneArm64Disassembler(disassembleMode);
        }

        /// <summary>
        ///     Create an ARM Disassembler.
        /// </summary>
        /// <param name="disassembleMode">
        ///     The hardware mode for the disassembler to use.
        /// </param>
        /// <returns>
        ///     An ARM disassembler.
        /// </returns>
        /// <exception cref="Gee.External.Capstone.CapstoneException">
        ///     Thrown if a disassembler could not be created.
        /// </exception>
        /// <exception cref="System.OutOfMemoryException">
        ///     Thrown if sufficient memory cannot be allocated to perform the operation as a rare indication that the
        ///     system is under heavy load.
        /// </exception>
        public static CapstoneArmDisassembler CreateArmDisassembler(ArmDisassembleMode disassembleMode) {
            return new CapstoneArmDisassembler(disassembleMode);
        }

        /// <summary>
        ///     Create an M68K Disassembler.
        /// </summary>
        /// <param name="disassembleMode">
        ///     The hardware mode for the disassembler to use.
        /// </param>
        /// <returns>
        ///     An M68K disassembler.
        /// </returns>
        /// <exception cref="Gee.External.Capstone.CapstoneException">
        ///     Thrown if a disassembler could not be created.
        /// </exception>
        /// <exception cref="System.OutOfMemoryException">
        ///     Thrown if sufficient memory cannot be allocated to perform the operation as a rare indication that the
        ///     system is under heavy load.
        /// </exception>
        public static CapstoneM68KDisassembler CreateM68KDisassembler(M68KDisassembleMode disassembleMode) {
            return new CapstoneM68KDisassembler(disassembleMode);
        }

        /// <summary>
        ///     Create a MIPS Disassembler.
        /// </summary>
        /// <param name="disassembleMode">
        ///     The hardware mode for the disassembler to use.
        /// </param>
        /// <returns>
        ///     A MIPS disassembler.
        /// </returns>
        /// <exception cref="Gee.External.Capstone.CapstoneException">
        ///     Thrown if a disassembler could not be created.
        /// </exception>
        /// <exception cref="System.OutOfMemoryException">
        ///     Thrown if sufficient memory cannot be allocated to perform the operation as a rare indication that the
        ///     system is under heavy load.
        /// </exception>
        public static CapstoneMipsDisassembler CreateMipsDisassembler(MipsDisassembleMode disassembleMode) {
            return new CapstoneMipsDisassembler(disassembleMode);
        }

        /// <summary>
        ///     Create a PowerPC Disassembler.
        /// </summary>
        /// <param name="disassembleMode">
        ///     The hardware mode for the disassembler to use.
        /// </param>
        /// <returns>
        ///     A PowerPC disassembler.
        /// </returns>
        /// <exception cref="Gee.External.Capstone.CapstoneException">
        ///     Thrown if a disassembler could not be created.
        /// </exception>
        /// <exception cref="System.OutOfMemoryException">
        ///     Thrown if sufficient memory cannot be allocated to perform the operation as a rare indication that the
        ///     system is under heavy load.
        /// </exception>
        public static CapstonePowerPcDisassembler CreatePowerPcDisassembler(PowerPcDisassembleMode disassembleMode) {
            return new CapstonePowerPcDisassembler(disassembleMode);
        }

        /// <summary>
        ///     Create an X86 Disassembler.
        /// </summary>
        /// <param name="disassembleMode">
        ///     The hardware mode for the disassembler to use.
        /// </param>
        /// <returns>
        ///     An X86 disassembler.
        /// </returns>
        /// <exception cref="Gee.External.Capstone.CapstoneException">
        ///     Thrown if a disassembler could not be created.
        /// </exception>
        /// <exception cref="System.OutOfMemoryException">
        ///     Thrown if sufficient memory cannot be allocated to perform the operation as a rare indication that the
        ///     system is under heavy load.
        /// </exception>
        public static CapstoneX86Disassembler CreateX86Disassembler(X86DisassembleMode disassembleMode) {
            return new CapstoneX86Disassembler(disassembleMode);
        }

        /// <summary>
        ///     Create an XCore Disassembler.
        /// </summary>
        /// <param name="disassembleMode">
        ///     The hardware mode for the disassembler to use.
        /// </param>
        /// <returns>
        ///     An XCore disassembler.
        /// </returns>
        /// <exception cref="Gee.External.Capstone.CapstoneException">
        ///     Thrown if a disassembler could not be created.
        /// </exception>
        /// <exception cref="System.OutOfMemoryException">
        ///     Thrown if sufficient memory cannot be allocated to perform the operation as a rare indication that the
        ///     system is under heavy load.
        /// </exception>
        public static CapstoneXCoreDisassembler CreateXCoreDisassembler(XCoreDisassembleMode disassembleMode) {
            return new CapstoneXCoreDisassembler(disassembleMode);
        }

        /// <summary>
        ///     Throw an Exception if Diet Mode is Enabled.
        /// </summary>
        /// <exception cref="System.NotSupportedException">
        ///     Thrown if Diet Mode is enabled.
        /// </exception>
        internal static void ThrowIfDietModeIsEnabled() {
            if (CapstoneDisassembler.IsDietModeEnabled) {
                const string detailMessage = "An operation is not supported when Diet Mode is enabled.";
                throw new NotSupportedException(detailMessage);
            }
        }

        /// <summary>
        ///     Throw an Exception if a Value is a Null Reference.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the value.
        /// </typeparam>
        /// <param name="name">
        ///     The name of the parameter the value was passed as an argument to.
        /// </param>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        ///     Thrown if the value is a null reference.
        /// </exception>
        internal static void ThrowIfValueIsNullReference<T>(string name, T value) where T : class {
            if (value == null) {
                const string detailMessage = "A value cannot be a null reference.";
                throw new ArgumentNullException(name, detailMessage);
            }
        }

        /// <summary>
        ///     Dispose Object.
        /// </summary>
        public abstract void Dispose();
    }

    /// <summary>
    ///     Capstone Disassembler.
    /// </summary>
    /// <typeparam name="TDisassembleMode">
    ///     The type of the hardware mode for the disassembler to use.
    /// </typeparam>
    /// <typeparam name="TInstruction">
    ///     The type of the disassembled instructions.
    /// </typeparam>
    /// <typeparam name="TInstructionDetail">
    ///     The type of the instructions' details.
    /// </typeparam>
    /// <typeparam name="TInstructionGroup">
    ///     The type of the instructions' architecture specific instruction groups.
    /// </typeparam>
    /// <typeparam name="TInstructionGroupId">
    ///     The type of the instructions' architecture specific instruction group unique identifiers.
    /// </typeparam>
    /// <typeparam name="TInstructionId">
    ///     The type of the instructions' unique identifiers.
    /// </typeparam>
    /// <typeparam name="TRegister">
    ///     The type of the instructions' architecture specific registers.
    /// </typeparam>
    /// <typeparam name="TRegisterId">
    ///     The type of the instructions' architecture specific register unique identifiers.
    /// </typeparam>
    public abstract class CapstoneDisassembler<TDisassembleMode, TInstruction, TInstructionDetail, TInstructionGroup, TInstructionGroupId, TInstructionId, TRegister, TRegisterId> : CapstoneDisassembler
        where TDisassembleMode : Enum
        where TInstruction : Instruction<TInstruction, TInstructionDetail, TDisassembleMode, TInstructionGroup, TInstructionGroupId, TInstructionId, TRegister, TRegisterId>
        where TInstructionDetail : InstructionDetail<TInstructionDetail, TDisassembleMode, TInstructionGroup, TInstructionGroupId, TInstruction, TInstructionId, TRegister, TRegisterId>
        where TInstructionGroup : InstructionGroup<TInstructionGroupId>
        where TInstructionGroupId : Enum
        where TInstructionId : Enum
        where TRegister : Register<TRegisterId>
        where TRegisterId : Enum {
        /// <summary>
        ///     Disassemble Architecture.
        /// </summary>
        private readonly DisassembleArchitecture _disassembleArchitecture;

        /// <summary>
        ///     Disassemble Mode.
        /// </summary>
        private TDisassembleMode _disassembleMode;

        /// <summary>
        ///     Disassemble Syntax.
        /// </summary>
        private DisassembleSyntax _disassembleSyntax;

        /// <summary>
        ///     Enable Instruction Details Flag.
        /// </summary>
        private bool _enableInstructionDetails;

        /// <summary>
        ///     Enable Skip Data Mode Flag.
        /// </summary>
        private bool _enableSkipDataMode;

        /// <summary>
        ///     Disassembler's Handle.
        /// </summary>
        private readonly NativeDisassemblerHandle _handle;

        /// <summary>
        ///     Native Disassemble Mode.
        /// </summary>
        private NativeDisassembleMode _nativeDisassembleMode;

        /// <summary>
        ///     Skip Data Callback.
        /// </summary>
        private Func<byte[], long, long> _skipDataCallback;

        /// <summary>
        ///     Skip Data Instruction Mnemonic.
        /// </summary>
        private string _skipDataInstructionMnemonic;

        /// <summary>
        ///     Get Disassemble Architecture.
        /// </summary>
        /// <remarks>
        ///     Represents the disassembler's hardware architecture.
        /// </remarks>
        public override DisassembleArchitecture DisassembleArchitecture => this._disassembleArchitecture;

        /// <summary>
        ///     Get and Set Disassemble Mode.
        /// </summary>
        /// <remarks>
        ///     Represents the disassembler's hardware mode.
        /// </remarks>
        /// <exception cref="Gee.External.Capstone.CapstoneException">
        ///     Thrown if the disassemble mode option could not be set.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        ///     Thrown if the disassembler handle is disposed.
        /// </exception>
        public TDisassembleMode DisassembleMode {
            get => this._disassembleMode;
            set {
                // ...
                //
                // This is an ugly operation but it is the only way I am familiar with to convert a <c>System.Enum</c>
                // to a 32-bit integer to pass to the Capstone API. It should be relatively quick since
                // <c>System.Enum</c> implements <c>System.IConvertible</c>.
                var iDisassembleMode = Convert.ToInt32(value);
                var disassembleMode = (NativeDisassembleMode) iDisassembleMode;

                // ..
                //
                // Throws an exception if the operation fails.
                NativeCapstone.SetDisassembleModeOption(this._handle, disassembleMode);

                this._disassembleMode = value;
                this._nativeDisassembleMode = disassembleMode;
            }
        }

        /// <summary>
        ///     Get and Set Disassemble Syntax.
        /// </summary>
        /// <exception cref="Gee.External.Capstone.CapstoneException">
        ///     Thrown if the disassemble syntax option could not be set.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        ///     Thrown if the disassembler is disposed.
        /// </exception>
        public DisassembleSyntax DisassembleSyntax {
            get => this._disassembleSyntax;
            set {
                // ..
                //
                // Throws an exception if the operation fails.
                const NativeDisassemblerOptionType optionType = NativeDisassemblerOptionType.SetSyntax;
                var optionValue = (NativeDisassemblerOptionValue) value;
                NativeCapstone.SetDisassemblerOption(this._handle, optionType, optionValue);

                this._disassembleSyntax = value;
            }
        }

        /// <summary>
        ///     Enable or Disable Instruction Details.
        /// </summary>
        /// <exception cref="Gee.External.Capstone.CapstoneException">
        ///     Thrown if the instruction details option could not be set.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        ///     Thrown if the disassembler is disposed.
        /// </exception>
        public override bool EnableInstructionDetails {
            get => this._enableInstructionDetails;
            set {
                // ..
                //
                // Throws an exception if the operation fails.
                const NativeDisassemblerOptionType optionType = NativeDisassemblerOptionType.SetInstructionDetails;
                var optionValue = value ? NativeDisassemblerOptionValue.Enable : NativeDisassemblerOptionValue.Disable;
                NativeCapstone.SetDisassemblerOption(this._handle, optionType, optionValue);

                this._enableInstructionDetails = value;
            }
        }

        /// <summary>
        ///     Enable or Disable Skip Data Mode.
        /// </summary>
        /// <exception cref="Gee.External.Capstone.CapstoneException">
        ///     Thrown if the Skip Data Mode option could not be set.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        ///     Thrown if the disassembler is disposed.
        /// </exception>
        public override bool EnableSkipDataMode {
            get => this._enableSkipDataMode;
            set {
                // ..
                //
                // Throws an exception if the operation fails.
                const NativeDisassemblerOptionType optionType = NativeDisassemblerOptionType.SetSkipData;
                var optionValue = value ? NativeDisassemblerOptionValue.Enable : NativeDisassemblerOptionValue.Disable;
                NativeCapstone.SetDisassemblerOption(this._handle, optionType, optionValue);

                this._enableSkipDataMode = value;
            }
        }

        /// <summary>
        ///     Get Disassembler's Handle.
        /// </summary>
        /// <remarks>
        ///     Represents the disassembler's native handle.
        /// </remarks>
        internal override NativeDisassemblerHandle Handle => this._handle;

        /// <summary>
        ///     Get Native Disassemble Mode.
        /// </summary>
        /// <remarks>
        ///     Represents the disassembler's native hardware mode.
        /// </remarks>
        internal override NativeDisassembleMode NativeDisassembleMode => this._nativeDisassembleMode;

        /// <summary>
        ///     Get and Set Skip Data Callback.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">
        ///     Thrown if the disassembler is disposed.
        /// </exception>
        public Func<byte[], long, long> SkipDataCallback {
            get => this._skipDataCallback;
            set {
                this.ThrowIfDisassemblerIsDisposed();
                this._skipDataCallback = value;
            }
        }

        /// <summary>
        ///     Get and Set Skip Data Instruction Mnemonic.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">
        ///     Thrown if the value is a null reference.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        ///     Thrown if the disassembler is disposed.
        /// </exception>
        public override string SkipDataInstructionMnemonic {
            get => this._skipDataInstructionMnemonic;
            set {
                this.ThrowIfDisassemblerIsDisposed();
                CapstoneDisassembler.ThrowIfValueIsNullReference(nameof(this.SkipDataInstructionMnemonic), value);

                this._skipDataInstructionMnemonic = value;
            }
        }

        /// <summary>
        ///     Create a Disassembler.
        /// </summary>
        /// <param name="disassembleArchitecture">
        ///     The hardware architecture for the disassembler to use.
        /// </param>
        /// <param name="disassembleMode">
        ///     The hardware mode for the disassembler to use.
        /// </param>
        /// <exception cref="Gee.External.Capstone.CapstoneException">
        ///     Thrown if a disassembler could not be created.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        ///     Thrown if the disassemble architecture is invalid, or if the disassemble mode is invalid or
        ///     unsupported by the disassemble architecture.
        /// </exception>
        /// <exception cref="System.OutOfMemoryException">
        ///     Thrown if sufficient memory cannot be allocated to perform the operation as a rare indication that the
        ///     system is under heavy load.
        /// </exception>
        private protected CapstoneDisassembler(DisassembleArchitecture disassembleArchitecture, TDisassembleMode disassembleMode) {
            this._disassembleArchitecture = disassembleArchitecture;
            this._disassembleMode = disassembleMode;
            this._disassembleSyntax = DisassembleSyntax.Intel;
            this._skipDataInstructionMnemonic = ".byte";
            // ...
            //
            // ...
            this._nativeDisassembleMode = CreateNativeDisassembleMode(this);
            // ...
            //
            // ...
            this._handle = NativeCapstone.CreateDisassembler(this._disassembleArchitecture, this._nativeDisassembleMode);

            // <summary>
            //      Create Native Disassemble Mode.
            // </summary>
            NativeDisassembleMode CreateNativeDisassembleMode(CapstoneDisassembler<TDisassembleMode, TInstruction, TInstructionDetail, TInstructionGroup, TInstructionGroupId, TInstructionId, TRegister, TRegisterId> @this) {
                // ...
                //
                // This is an ugly operation but it is the only way I am familiar with to convert a <c>System.Enum</c> to
                // a 32-bit integer to pass to the Capstone API. It should be relatively quick since <c>System.Enum</c>
                // implements <c>System.IConvertible</c>.
                var cIDisassembleMode = Convert.ToInt32(@this._disassembleMode);
                return (NativeDisassembleMode) cIDisassembleMode;
            }
        }

        /// <summary>
        ///     Create an Instruction.
        /// </summary>
        /// <param name="hInstruction">
        ///     An instruction handle.
        /// </param>
        /// <returns>
        ///     An instruction.
        /// </returns>
        private protected abstract TInstruction CreateInstruction(NativeInstructionHandle hInstruction);

        /// <summary>
        ///     Disassemble Binary Code.
        /// </summary>
        /// <param name="binaryCode">
        ///     An array of bytes representing the binary code to disassemble.
        /// </param>
        /// <returns>
        ///     An array of disassembled instructions.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     Thrown if the binary code array is a null reference.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        ///     Thrown if the disassembler is disposed.
        /// </exception>
        public TInstruction[] Disassemble(byte[] binaryCode) {
            // ...
            //
            // Throws an exception if the operation fails.
            var instructions = this.Disassemble(binaryCode, 0);
            return instructions;
        }

        /// <summary>
        ///     Disassemble Binary Code.
        /// </summary>
        /// <param name="binaryCode">
        ///     An array of bytes representing the binary code to disassemble.
        /// </param>
        /// <param name="startingAddress">
        ///     The address of the first instruction in the binary code array.
        /// </param>
        /// <returns>
        ///     An array of disassembled instructions.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     Thrown if the binary code array is a null reference.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        ///     Thrown if the disassembler is disposed.
        /// </exception>
        public TInstruction[] Disassemble(byte[] binaryCode, long startingAddress) {
            // ...
            //
            // Throws an exception if the operation fails.
            var instructions = this.Disassemble(binaryCode, startingAddress, 0);
            return instructions;
        }

        /// <summary>
        ///     Disassemble Binary Code.
        /// </summary>
        /// <param name="binaryCode">
        ///     An array of bytes representing the binary code to disassemble.
        /// </param>
        /// <param name="startingAddress">
        ///     The address of the first instruction in the binary code array.
        /// </param>
        /// <param name="count">
        ///     The maximum number of instructions to disassemble.
        /// </param>
        /// <returns>
        ///     An array of disassembled instructions.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     Thrown if the binary code array is a null reference.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        ///     Thrown if the disassembler is disposed.
        /// </exception>
        public TInstruction[] Disassemble(byte[] binaryCode, long startingAddress, int count) {
            // ...
            //
            // Throws an exception if the operation fails.
            var instructionIterator = this.Iterate(binaryCode, startingAddress);

            // ...
            //
            // We want to emulate the Capstone API by treating a <c>0</c> as an indication that all instructions
            // should be disassembled.
            if (count != 0) {
                // ...
                //
                // If there are less instructions than indicated, all the instructions will be returned.
                instructionIterator = instructionIterator.Skip(0).Take(count);
            }

            var instructions = instructionIterator.ToArray();
            return instructions;
        }

        /// <summary>
        ///     Dispose Object.
        /// </summary>
        public override void Dispose() {
            // ...
            //
            // This operation is safe, even if it is invoked multiple times and the handle is already disposed.
            this._handle.Dispose();
        }

        /// <summary>
        ///     Get an Instruction Group's Name.
        /// </summary>
        /// <param name="instructionGroupId">
        ///     An instruction group's unique identifier.
        /// </param>
        /// <returns>
        ///     The instruction group's name.
        /// </returns>
        /// <exception cref="System.ArgumentException">
        ///     Thrown if the instruction group's unique identifier is invalid.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        ///     Thrown if the disassembler is disposed.
        /// </exception>
        /// <exception cref="System.NotSupportedException">
        ///     Thrown if Diet Mode is enabled.
        /// </exception>
        public string GetInstructionGroupName(TInstructionGroupId instructionGroupId) {
            this.ThrowIfDisassemblerIsDisposed();
            CapstoneDisassembler.ThrowIfDietModeIsEnabled();

            // ...
            //
            // This is an ugly operation but it is the only way I am familiar with to convert a <c>System.Enum</c> to
            // a 32-bit integer to pass to the Capstone API. It should be relatively quick since <c>System.Enum</c>
            // implements <c>System.IConvertible</c>.
            var iInstructionGroupId = Convert.ToInt32(instructionGroupId);

            // ...
            //
            // This operation will return a null reference if 1) the handle is disposed, 2) if diet mode is enabled,
            // or 3) if the register unique identifier is invalid. Unfortunately it does not differentiate between the
            // 3 conditions. However, because we already guarded against conditions 1 and 2, if it does return a null
            // reference, it must be because of condition 3.
            var instructionGroupName = NativeCapstone.GetInstructionGroupName(this._handle, iInstructionGroupId);
            if (instructionGroupName == null) {
                const string detailMessage = "An instruction group unique identifier is invalid.";
                throw new ArgumentException(detailMessage, nameof(instructionGroupId));
            }

            return instructionGroupName;
        }

        /// <summary>
        ///     Get a Register's Name.
        /// </summary>
        /// <param name="registerId">
        ///     A register's unique identifier.
        /// </param>
        /// <returns>
        ///     The register's name.
        /// </returns>
        /// <exception cref="System.ArgumentException">
        ///     Thrown if the register's unique identifier is invalid.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        ///     Thrown if the disassembler is disposed.
        /// </exception>
        /// <exception cref="System.NotSupportedException">
        ///     Thrown if diet mode is enabled.
        /// </exception>
        public string GetRegisterName(TRegisterId registerId) {
            this.ThrowIfDisassemblerIsDisposed();
            CapstoneDisassembler.ThrowIfDietModeIsEnabled();

            // ...
            //
            // This is an ugly operation but it is the only way I am familiar with to convert a <c>System.Enum</c> to
            // a 32-bit integer to pass to the Capstone API. It should be relatively quick since <c>System.Enum</c>
            // implements <c>System.IConvertible</c>.
            var iRegisterId = Convert.ToInt32(registerId);

            // ...
            //
            // This operation will return a null reference if 1) the handle is disposed, 2) if diet mode is enabled,
            // or 3) if the register unique identifier is invalid. Unfortunately it does not differentiate between the
            // 3 conditions. However, because we already guarded against conditions 1 and 2, if it does return a null
            // reference, it must be because of condition 3.
            var registerName = NativeCapstone.GetRegisterName(this._handle, iRegisterId);
            if (registerName == null) {
                const string detailMessage = "A register unique identifier is invalid.";
                throw new ArgumentException(detailMessage, nameof(registerId));
            }

            return registerName;
        }

        /// <summary>
        ///     Disassemble Binary Code Iteratively.
        /// </summary>
        /// <param name="binaryCode">
        ///     An array of bytes representing the binary code to disassemble.
        /// </param>
        /// <returns>
        ///     A deferred collection of disassembled instructions.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     Thrown if the binary code array is a null reference.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        ///     Thrown if the disassembler is disposed.
        /// </exception>
        public IEnumerable<TInstruction> Iterate(byte[] binaryCode) {
            // ...
            //
            // Throws an exception if the operation fails.
            return this.Iterate(binaryCode, 0x1000);
        }

        /// <summary>
        ///     Disassemble Binary Code Iteratively.
        /// </summary>
        /// <param name="binaryCode">
        ///     An array of bytes representing the binary code to disassemble.
        /// </param>
        /// <param name="startingAddress">
        ///     The address of the first instruction in the binary code array.
        /// </param>
        /// <returns>
        ///     A deferred collection of disassembled instructions.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     Thrown if the binary code array is a null reference.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        ///     Thrown if the disassembler is disposed.
        /// </exception>
        public IEnumerable<TInstruction> Iterate(byte[] binaryCode, long startingAddress) {
            CapstoneDisassembler.ThrowIfValueIsNullReference(nameof(binaryCode), binaryCode);

            var binaryCodeOffset = 0;

            // ...
            //
            // The Capstone API's iterative disassemble function has an interesting challenge when it is invoked with
            // Skip Data Mode enabled. Because it disassembles one instruction at a time, the binary code buffer that
            // it passes to the Skip Data Mode Callback is not the entire binary code buffer passed by the caller, but
            // rather only the slice that contains the identified invalid instruction. This makes it difficult for the
            // caller to perform any analysis in the Skip Data Mode Callback that might depend on inspecting the
            // entire binary code buffer. This isn't necessarily a deal breaker since a caller can work around this in
            // multiple ways.
            //
            // However, we'll do the hard work for the caller here and define the Skip Data Mode Callback as a proxy
            // closure that encloses over the entire binary code buffer and pass it to the actual callback defined
            // by the caller.
            NativeCapstone.SkipDataCallback callback = null;
            if (this.EnableSkipDataMode) {
                // ...
                //
                // Normally, delegates that are created for the purpose of being passed as function pointers to
                // unmanaged code, such as this Skip Data Mode Callback, need to be allocated using a method that
                // prevents them from being garbage collected. A delegate created as a local variable typically would
                // be a problem because it would go out of scope as soon as the function returns and thus be eligible
                // for garbage collection. A process crash is guaranteed if that happens while it is still referenced
                // by unmanaged code.
                //
                // However, because this method is an iterator method, when the compiler compiles it it will actually
                // create a new class for the iterator's state machine and every local variable defined will become a
                // private field defined by this new class, whose lifetime is actually tied to the lifetime of the
                // class. So the problem of garbage collection is actually avoided for the lifetime of the iterator.
                //
                // To make this work though, we MUST define a local variable for the delegate!
                if (this._skipDataCallback != null) {
                    callback = OnNativeSkipDataCallback;
                }

                // ...
                //
                // Throws an exception if the operation fails.
                var optionValue = new NativeSkipDataOptionValue();
                optionValue.Callback = callback;
                optionValue.InstructionMnemonic = this._skipDataInstructionMnemonic;
                optionValue.State = IntPtr.Zero;
                NativeCapstone.SetSkipDataOption(this._handle, ref optionValue);
            }

            try {
                // ...
                //
                // We allocate memory for one instruction and reuse it for every instruction in the binary code
                // buffer. Throws an exception if the operation fails.
                using (var hInstruction = NativeCapstone.CreateInstruction(this._handle)) {
                    while (!(binaryCodeOffset >= binaryCode.Length)) {
                        // ...
                        //
                        // Throws an exception if the operation fails.
                        var isDisassembled = NativeCapstone.Iterate(this._handle, binaryCode, ref binaryCodeOffset, ref startingAddress, hInstruction);
                        if (!isDisassembled) {
                            yield break;
                        }

                        var instruction = this.CreateInstruction(hInstruction);
                        yield return instruction;
                    }
                }
            }
            finally {
                if (callback != null) {
                    // ...
                    //
                    // Throws an exception if the operation fails. If the operation fails here, it could only be
                    // because the disassembler is disposed, which will have no side effects if it happens here.
                    var optionValue = new NativeSkipDataOptionValue();
                    optionValue.Callback = null;
                    optionValue.InstructionMnemonic = this._skipDataInstructionMnemonic;
                    optionValue.State = IntPtr.Zero;
                    NativeCapstone.SetSkipDataOption(this._handle, ref optionValue);
                }
            }

            // <summary>
            //      Native Skip Data Mode Callback.
            // </summary>
            IntPtr OnNativeSkipDataCallback(IntPtr cPBinaryCode, IntPtr cBinaryCodeSize, IntPtr cDataOffset, IntPtr pState) {
                // ...
                //
                // Normally, a closure enclosing over a variable modified from a loop, such as this method, is a
                // problem because the value of the variable is resolved at the time the closure is invoked and not
                // when the variable is captured. This can lead to unexpected behavior if the closure is invoked
                // outside the loop since the value of the captured variable will always be resolved to the last value
                // it was set to inside the loop.
                //
                // However, because this closure will always be invoked from inside a disassemble loop, and never from
                // outside of one, the variable value resolution behavior is exactly what we are looking for. We want
                // the Capstone API to invoke this callback with an updated value for the captured variable every
                // time!
                var cBytesToSkip = this.SkipDataCallback(binaryCode, binaryCodeOffset);
                return new IntPtr(cBytesToSkip);
            }
        }

        /// <summary>
        ///     Reset an Instruction's Mnemonic.
        /// </summary>
        /// <param name="instructionId">
        ///     An instruction unique identifier.
        /// </param>
        /// <exception cref="Gee.External.Capstone.CapstoneException">
        ///     Thrown if the instruction mnemonic could not be reset.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        ///     Thrown if the disassembler is disposed.
        /// </exception>
        public void ResetInstructionMnemonic(TInstructionId instructionId) {
            // ...
            //
            // This is an ugly operation but it is the only way I am familiar with to convert a <c>System.Enum</c> to
            // a 32-bit integer to pass to the Capstone API. It should be relatively quick since <c>System.Enum</c>
            // implements <c>System.IConvertible</c>.
            var iInstructionId = Convert.ToInt32(instructionId);

            var optionValue = new NativeInstructionMnemonicOptionValue {
                InstructionId = iInstructionId,
                InstructionMnemonic = null
            };

            // ...
            //
            // Throws an exception if the operation fails.
            NativeCapstone.SetInstructionMnemonicOption(this._handle, ref optionValue);
        }

        /// <summary>
        ///     Set an Instruction's Mnemonic.
        /// </summary>
        /// <param name="instructionId">
        ///     An instruction's unique identifier.
        /// </param>
        /// <param name="instructionMnemonic">
        ///     A mnemonic to associate with the instruction.
        /// </param>
        /// <exception cref="Gee.External.Capstone.CapstoneException">
        ///     Thrown if the instruction mnemonic could not be set.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        ///     Thrown if the instruction mnemonic is a null reference.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        ///     Thrown if the disassembler is disposed.
        /// </exception>
        public void SetInstructionMnemonic(TInstructionId instructionId, string instructionMnemonic) {
            CapstoneDisassembler.ThrowIfValueIsNullReference(nameof(instructionMnemonic), instructionMnemonic);

            // ...
            //
            // This is an ugly operation but it is the only way I am familiar with to convert a <c>System.Enum</c> to
            // a 32-bit integer to pass to the Capstone API. It should be relatively quick since <c>System.Enum</c>
            // implements <c>System.IConvertible</c>.
            var iInstructionId = Convert.ToInt32(instructionId);

            var optionValue = new NativeInstructionMnemonicOptionValue {
                InstructionId = iInstructionId,
                InstructionMnemonic = instructionMnemonic
            };

            // ...
            //
            // Throws an exception if the operation fails.
            NativeCapstone.SetInstructionMnemonicOption(this._handle, ref optionValue);
        }

        /// <summary>
        ///     Throw an Exception if Disassembler is Disposed.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">
        ///     Thrown if the disassembler is disposed.
        /// </exception>
        private void ThrowIfDisassemblerIsDisposed() {
            if (this._handle.IsClosed) {
                const string detailMessage = "A disassembler is disposed.";
                throw new ObjectDisposedException(nameof(CapstoneDisassembler), detailMessage);
            }
        }
    }
}
