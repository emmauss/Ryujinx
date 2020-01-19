using System;
using System.Threading;
using System.Runtime.InteropServices;

namespace ARMeilleure.Translation
{
    class TranslatedFunction
    {
        public IntPtr Pointer => Marshal.GetFunctionPointerForDelegate(_func);

        public int EntryCount;

        private const int MinCallsForRejit = 100;

        private GuestFunction _func;

        private ulong _address;
        private bool  _rejit;
        private int   _callCount;

        public TranslatedFunction(GuestFunction func, ulong address, bool rejit)
        {
            _func = func;
            _rejit = rejit;
            _address = address;
        }

        public ulong Execute(State.ExecutionContext context)
        {
            if (Interlocked.Increment(ref EntryCount) == 0)
            {
                return _address;
            }

            var nextAddress = _func(context.NativeContextPtr);

            Interlocked.Decrement(ref EntryCount);

            return nextAddress;
        }

        public bool ShouldRejit()
        {
            return _rejit && Interlocked.Increment(ref _callCount) == MinCallsForRejit;
        }
    }
}