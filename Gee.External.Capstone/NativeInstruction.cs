using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Gee.External.Capstone {
    /// <summary>
    ///     Native Disassembled Instruction.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    internal struct NativeInstruction {
        /// <summary>
        ///     Instruction's Unique Identifier.
        /// </summary>
        public int Id;

        /// <summary>
        ///     Instruction's Address (EIP).
        /// </summary>
        public long Address;

        /// <summary>
        ///     Instruction's Size.
        /// </summary>
        public short Size;

        /// <summary>
        ///     Instruction's Machine Bytes.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] Bytes;

        /// <summary>
        ///     Instruction's Mnemonic.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string Mnemonic;

        /// <summary>
        ///     Instruction's Operand Text.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 160)]
        public string Operand;

        /// <summary>
        ///     Instruction's Details.
        /// </summary>
        /// <remarks>
        ///     Represents a pointer to the instruction's details in unmanaged memory. A <c>IntPtr.Zero</c> indicates
        ///     the instruction was disassembled without details.
        /// </remarks>
        public IntPtr Details;
    }
}