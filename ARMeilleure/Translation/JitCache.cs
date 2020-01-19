using ARMeilleure.CodeGen;
using ARMeilleure.Memory;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ARMeilleure.Translation
{
    static class JitCache
    {
        private const int PageSize = 4 * 1024;
        private const int PageMask = PageSize - 1;

        private const int CodeAlignment = 4; // Bytes

        private const int CacheSize = 512 * 1024 * 1024;

        private static IntPtr _basePointer;

        private static JitCacheMemoryAllocator _allocator;

        private static Dictionary<int, JitCacheEntry> _cacheEntries;

        private static object _lock;

        static JitCache()
        {
            _basePointer = MemoryManagement.Allocate(CacheSize);

            int startOffset = 0;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                JitUnwindWindows.InstallFunctionTableHandler(_basePointer, CacheSize);

                // The first page is used for the table based SEH structs.
                startOffset = PageSize;
            }

            ReprotectRange(startOffset, CacheSize - startOffset);

            _allocator = new JitCacheMemoryAllocator(CacheSize, startOffset);

            _cacheEntries = new Dictionary<int, JitCacheEntry>();

            _lock = new object();
        }

        public static IntPtr Map(CompiledFunction func)
        {
            byte[] code = func.Code;

            lock (_lock)
            {
                int funcOffset = Allocate(code.Length);

                IntPtr funcPtr = _basePointer + funcOffset;

                Marshal.Copy(code, 0, funcPtr, code.Length);

                Add(new JitCacheEntry(funcOffset, code.Length, func.UnwindInfo));

                return funcPtr;
            }
        }

        private static void ReprotectRange(int offset, int size)
        {
            // Map pages that are already full as RX.
            // Map pages that are not full yet as RWX.
            // On unix, the address must be page aligned.
            int endOffs = offset + size;

            int pageStart = offset  & ~PageMask;
            int pageEnd   = endOffs & ~PageMask;

            int fullPagesSize = pageEnd - pageStart;

            if (fullPagesSize != 0)
            {
                IntPtr funcPtr = _basePointer + pageStart;

                MemoryManagement.Reprotect(funcPtr, (ulong)fullPagesSize, MemoryProtection.ReadWriteExecute);
            }
        }

        private static int Allocate(int codeSize)
        {
            codeSize = checked(codeSize + (CodeAlignment - 1)) & ~(CodeAlignment - 1);

            int allocOffset = _allocator.Allocate(codeSize);

            return allocOffset;
        }

        public static void Free(IntPtr address)
        {
            ulong offset = (ulong)address - (ulong)_basePointer;

            lock (_lock)
            {
                if (TryFind((int)offset, out JitCacheEntry entry))
                {
                    _cacheEntries.Remove((int)offset, out entry);

                    int size = checked(entry.Size + (CodeAlignment - 1)) & ~(CodeAlignment - 1);

                    _allocator.Free((int)entry.Offset, size);
                }
            }
        }

        private static void Add(JitCacheEntry entry)
        {
            _cacheEntries.Add(entry.Offset, entry);
        }

        public static bool TryFind(int offset, out JitCacheEntry entry)
        {
            lock (_lock)
            {
                if (_cacheEntries.TryGetValue(offset, out entry))
                {
                    return true;
                }
            }

            entry = default(JitCacheEntry);

            return false;
        }
    }
}