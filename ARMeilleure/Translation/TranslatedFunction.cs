using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace ARMeilleure.Translation
{
    class TranslatedFunction
    {
        public IntPtr Address { get; }

        private const int MinCallsForRejit = 100;

        private GuestFunction _func;

        private bool _rejit;
        private int  _callCount;

        public TranslatedFunction(GuestFunction func, bool rejit)
        {
            Address = Marshal.GetFunctionPointerForDelegate(func);

            _func  = func;
            _rejit = rejit;
        }

        public ulong Execute(State.ExecutionContext context)
        {
            return _func(context.NativeContextPtr);
        }

        public bool ShouldRejit()
        {
            return _rejit && Interlocked.Increment(ref _callCount) == MinCallsForRejit;
        }
    }
}