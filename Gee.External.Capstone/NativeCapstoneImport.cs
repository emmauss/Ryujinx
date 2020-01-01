using System;
using System.Runtime.InteropServices;

namespace Gee.External.Capstone {
    /// <summary>
    ///     Native Capstone Import.
    /// </summary>
    internal static class NativeCapstoneImport {
        /// <summary>
        ///     Close a Disassembler
        /// </summary>
        /// <param name="pDissembler">
        ///     A pointer to a disassembler.
        /// </param>
        /// <returns>
        ///     A result code indicating the result of the operation.
        /// </returns>
        [DllImport("capstone", CallingConvention = CallingConvention.Cdecl, EntryPoint = "cs_close")]
        internal static extern NativeCapstoneResultCode CloseDisassembler(ref IntPtr pDissembler);

        /// <summary>
        ///     Create a Disassembler.
        /// </summary>
        /// <param name="disassembleArchitecture">
        ///     The hardware architecture for the disassembler to use.
        /// </param>
        /// <param name="disassembleMode">
        ///     The hardware mode for the disassembler to use.
        /// </param>
        /// <param name="pDisassembler">
        ///     A pointer that will be updated to reference the disassembler.
        /// </param>
        /// <returns>
        ///     A result code indicating the result of the operation.
        /// </returns>
        [DllImport("capstone", CallingConvention = CallingConvention.Cdecl, EntryPoint = "cs_open")]
        internal static extern NativeCapstoneResultCode CreateDisassembler(DisassembleArchitecture disassembleArchitecture, NativeDisassembleMode disassembleMode, ref IntPtr pDisassembler);

        /// <summary>
        ///     Create an Instruction..
        /// </summary>
        /// <param name="hDisassembler">
        ///     A disassembler handle.
        /// </param>
        /// <returns>
        ///     A pointer to the instruction.
        /// </returns>
        /// <exception cref="System.ObjectDisposedException">
        ///     Thrown if the disassembler handle is disposed.
        /// </exception>
        [DllImport("capstone", CallingConvention = CallingConvention.Cdecl, EntryPoint = "cs_malloc")]
        internal static extern IntPtr CreateInstruction(NativeDisassemblerHandle hDisassembler);

        /// <summary>
        ///     Disassemble Binary Code.
        /// </summary>
        /// <param name="hDisassembler">
        ///     A disassembler handle.
        /// </param>
        /// <param name="pCode">
        ///     A pointer to a buffer indicating the binary code to disassemble.
        /// </param>
        /// <param name="codeSize">
        ///     A platform dependent integer indicating the size, in bytes, of the binary code buffer.
        /// </param>
        /// <param name="startingAddress">
        ///     The address of the first instruction in the binary code buffer.
        /// </param>
        /// <param name="count">
        ///     The maximum number of instructions in the binary code buffer to disassemble. A <c>0</c> indicates all
        ///     instructions in the binary code buffer should be disassembled.
        /// </param>
        /// <param name="pInstructions">
        ///     A pointer that will be updated to reference the disassembled instructions.
        /// </param>
        /// <returns>
        ///     A platform dependent integer indicating the number of disassembled instructions if the binary code was
        ///     disassembled successfully. An <c>IntPtr.Zero</c> otherwise.
        /// </returns>
        /// <exception cref="System.ObjectDisposedException">
        ///     Thrown if the disassembler handle is disposed.
        /// </exception>
        [DllImport("capstone", CallingConvention = CallingConvention.Cdecl, EntryPoint = "cs_disasm")]
        internal static extern IntPtr Disassemble(NativeDisassemblerHandle hDisassembler, IntPtr pCode, IntPtr codeSize, long startingAddress, IntPtr count, ref IntPtr pInstructions);

        /// <summary>
        ///     Free Memory Allocated For Disassembled Instructions.
        /// </summary>
        /// <param name="pInstructions">
        ///     A pointer to disassembled instructions.
        /// </param>
        /// <param name="count">
        ///     The number of disassembled instructions.
        /// </param>
        [DllImport("capstone", CallingConvention = CallingConvention.Cdecl, EntryPoint = "cs_free")]
        internal static extern void FreeInstructions(IntPtr pInstructions, IntPtr count);

        /// <summary>
        ///     Get an Instruction's Accessed Registers.
        /// </summary>
        /// <param name="hDisassembler">
        ///     A disassembler handle.
        /// </param>
        /// <param name="hInstruction">
        ///     An instruction handle.
        /// </param>
        /// <param name="readRegisters">
        ///     An array that will be updated to indicate the instruction's read registers.
        /// </param>
        /// <param name="readRegistersCount">
        ///     An 8-bit integer that will be updated to indicate the number of read registers.
        /// </param>
        /// <param name="writtenRegisters">
        ///     An array that will be updated to indicate the instruction's written registers.
        /// </param>
        /// <param name="writtenRegistersCount">
        ///     An 8-bit integer that will be updated to indicate the number of written registers.
        /// </param>
        /// <returns>
        ///     A result code indicating the result of the operation.
        /// </returns>
        /// <exception cref="System.ObjectDisposedException">
        ///     Thrown if the disassembler handle is disposed, or if the instruction handle is disposed.
        /// </exception>
        [DllImport("capstone", CallingConvention = CallingConvention.Cdecl, EntryPoint = "cs_regs_access")]
        internal static extern NativeCapstoneResultCode GetAccessedRegisters(NativeDisassemblerHandle hDisassembler, NativeInstructionHandle hInstruction, short[] readRegisters, ref byte readRegistersCount, short[] writtenRegisters, ref byte writtenRegistersCount);

        /// <summary>
        ///     Get an Instruction Group's Name.
        /// </summary>
        /// <param name="hDisassembler">
        ///     A disassembler handle.
        /// </param>
        /// <param name="instructionGroupId">
        ///     An instruction group's unique identifier.
        /// </param>
        /// <returns>
        ///     A pointer to an ASCII string indicating the instruction group's name.
        /// </returns>
        /// <exception cref="System.ObjectDisposedException">
        ///     Thrown if the disassembler handle is disposed.
        /// </exception>
        [DllImport("capstone", CallingConvention = CallingConvention.Cdecl, EntryPoint = "cs_group_name")]
        internal static extern IntPtr GetInstructionGroupName(NativeDisassemblerHandle hDisassembler, int instructionGroupId);

        /// <summary>
        ///     Get Last Error Code.
        /// </summary>
        /// <param name="hDisassembler">
        ///     A disassembler handle.
        /// </param>
        /// <returns>
        ///     The error code of the last generated error.
        /// </returns>
        /// <exception cref="System.ObjectDisposedException">
        ///     Thrown if the disassembler handle is disposed.
        /// </exception>
        [DllImport("capstone", CallingConvention = CallingConvention.Cdecl, EntryPoint = "cs_errno")]
        internal static extern NativeCapstoneResultCode GetLastErrorCode(NativeDisassemblerHandle hDisassembler);

        /// <summary>
        ///     Get a Register's Name.
        /// </summary>
        /// <param name="hDisassembler">
        ///     A disassembler handle.
        /// </param>
        /// <param name="registerId">
        ///     A register's unique identifier.
        /// </param>
        /// <returns>
        ///     A pointer to an ASCII string indicating the register's name.
        /// </returns>
        /// <exception cref="System.ObjectDisposedException">
        ///     Thrown if the disassembler handle is disposed.
        /// </exception>
        [DllImport("capstone", CallingConvention = CallingConvention.Cdecl, EntryPoint = "cs_reg_name")]
        internal static extern IntPtr GetRegisterName(NativeDisassemblerHandle hDisassembler, int registerId);

        /// <summary>
        ///     Get Capstone Library's Version.
        /// </summary>
        /// <param name="majorVersion">
        ///     A 32-bit integer that will be updated to indicate the Capstone library's major version.
        /// </param>
        /// <param name="minorVersion">
        ///     A 32-bit integer that will be updated to indicate the Capstone library's minor version.
        /// </param>
        /// <returns>
        ///     A 32-bit integer indicating the Capstone library's major and minor version.
        /// </returns>
        [DllImport("capstone", CallingConvention = CallingConvention.Cdecl, EntryPoint = "cs_version")]
        internal static extern int GetVersion(ref int majorVersion, ref int minorVersion);

        /// <summary>
        ///     Disassemble Binary Code Iteratively.
        /// </summary>
        /// <param name="hDisassembler">
        ///     A disassembler handle.
        /// </param>
        /// <param name="pCode">
        ///     A pointer to a buffer indicating the binary code to disassemble.
        /// </param>
        /// <param name="codeSize">
        ///     A platform dependent integer indicating the size, in bytes, of the binary code buffer.
        /// </param>
        /// <param name="address">
        ///     The address of the first instruction in the binary code buffer.
        /// </param>
        /// <param name="hInstruction">
        ///     An instruction handle.
        /// </param>
        /// <returns>
        ///     A boolean true if an instruction was disassembled successfully. A boolean false otherwise.
        /// </returns>
        /// <exception cref="System.ObjectDisposedException">
        ///     Thrown if the disassembler handle is disposed, or if the instruction handle is disposed.
        /// </exception>
        [DllImport("capstone", CallingConvention = CallingConvention.Cdecl, EntryPoint = "cs_disasm_iter")]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool Iterate(NativeDisassemblerHandle hDisassembler, ref IntPtr pCode, ref IntPtr codeSize, ref long address, NativeInstructionHandle hInstruction);

        /// <summary>
        ///     Load a Library.
        /// </summary>
        /// <param name="libraryFilePath">
        ///     The absolute file path of the library to load.
        /// </param>
        /// <returns>
        ///      A pointer to the loaded library. An <c>IntPtr.Zero</c> indicates the library was not loaded.
        /// </returns>
        /// <see href="https://msdn.microsoft.com/en-us/library/windows/desktop/ms684175(v=vs.85).aspx"/>
        [DllImport("kernel32.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Ansi, EntryPoint = "LoadLibraryA", SetLastError = true)]
        internal static extern IntPtr LoadLibrary(string libraryFilePath);

        /// <summary>
        ///     Query an Option.
        /// </summary>
        /// <param name="queryOption">
        ///     An option to query.
        /// </param>
        /// <returns>
        ///     A boolean true if the option is supported. A boolean false otherwise.
        /// </returns>
        [DllImport("capstone", CallingConvention = CallingConvention.Cdecl, EntryPoint = "cs_support")]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool Query(NativeQueryOption queryOption);

        /// <summary>
        ///     Set a Disassembler Option.
        /// </summary>
        /// <param name="hDisassembler">
        ///     A disassembler handle.
        /// </param>
        /// <param name="optionType">
        ///     A type of option to set.
        /// </param>
        /// <param name="optionValue">
        ///     A platform dependent integer indicating the value to set the option to.
        /// </param>
        /// <returns>
        ///     A result code indicating the result of the operation.
        /// </returns>
        /// <exception cref="System.ObjectDisposedException">
        ///     Thrown if the disassembler handle is disposed.
        /// </exception>
        [DllImport("capstone", CallingConvention = CallingConvention.Cdecl, EntryPoint = "cs_option")]
        internal static extern NativeCapstoneResultCode SetDisassemblerOption(NativeDisassemblerHandle hDisassembler, NativeDisassemblerOptionType optionType, IntPtr optionValue);
    }
}