using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Bear
{
    [AsyncMethodBuilder(typeof (AsyncUniTaskCompletedMethodBuilder))]
    public struct UniTaskCompleted: ICriticalNotifyCompletion
    {
        [DebuggerHidden]
        public UniTaskCompleted GetAwaiter()
        {
            return this;
        }

        [DebuggerHidden]
        public bool IsCompleted => true;

        [DebuggerHidden]
        public void GetResult()
        {
        }

        [DebuggerHidden]
        public void OnCompleted(Action continuation)
        {
        }

        [DebuggerHidden]
        public void UnsafeOnCompleted(Action continuation)
        {
        }
    }
}