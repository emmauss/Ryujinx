using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Gee.External.Capstone {
    /// <summary>
    ///     Native Disassembled Instruction Details.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    internal struct NativeInstructionDetail {
        /// <summary>
        ///     Implicitly Read Registers.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        public short[] ImplicitlyReadRegisters;

        /// <summary>
        ///     Implicitly Read Registers' Count.
        /// </summary>
        public byte ImplicitlyReadRegisterCount;

        /// <summary>
        ///     Implicitly Written Registers.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public short[] ImplicitlyWrittenRegisters;

        /// <summary>
        ///     Implicitly Written Registers' Count.
        /// </summary>
        public byte ImplicitlyWrittenRegisterCount;

        /// <summary>
        ///     Instruction's Groups.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Groups;

        /// <summary>
        ///     Instruction's Groups' Count.
        /// </summary>
        public byte GroupCount;
    }
}