﻿using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security;

namespace Bear
{
    public struct UniAsyncTaskMethodBuilder
    {
        private UniTask tcs;

        // 1. Static Create method.
        [DebuggerHidden]
        public static UniAsyncTaskMethodBuilder Create()
        {
            UniAsyncTaskMethodBuilder builder = new UniAsyncTaskMethodBuilder() { tcs = UniTask.Create(true) };
            return builder;
        }

        // 2. TaskLike Task property.
        [DebuggerHidden]
        public UniTask Task => this.tcs;

        // 3. SetException
        [DebuggerHidden]
        public void SetException(Exception exception)
        {
            this.tcs.SetException(exception);
        }

        // 4. SetResult
        [DebuggerHidden]
        public void SetResult()
        {
            this.tcs.SetResult();
        }

        // 5. AwaitOnCompleted
        [DebuggerHidden]
        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : INotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            awaiter.OnCompleted(stateMachine.MoveNext);
        }

        // 6. AwaitUnsafeOnCompleted
        [DebuggerHidden]
        [SecuritySafeCritical]
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            awaiter.OnCompleted(stateMachine.MoveNext);
        }

        // 7. Start
        [DebuggerHidden]
        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            stateMachine.MoveNext();
        }

        // 8. SetStateMachine
        [DebuggerHidden]
        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
        }
    }

    public struct UniAsyncTaskMethodBuilder<T>
    {
        private UniTask<T> tcs;

        // 1. Static Create method.
        [DebuggerHidden]
        public static UniAsyncTaskMethodBuilder<T> Create()
        {
            UniAsyncTaskMethodBuilder<T> builder = new UniAsyncTaskMethodBuilder<T>() { tcs = UniTask<T>.Create(true) };
            return builder;
        }

        // 2. TaskLike Task property.
        [DebuggerHidden]
        public UniTask<T> Task => this.tcs;

        // 3. SetException
        [DebuggerHidden]
        public void SetException(Exception exception)
        {
            this.tcs.SetException(exception);
        }

        // 4. SetResult
        [DebuggerHidden]
        public void SetResult(T ret)
        {
            this.tcs.SetResult(ret);
        }

        // 5. AwaitOnCompleted
        [DebuggerHidden]
        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : INotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            awaiter.OnCompleted(stateMachine.MoveNext);
        }

        // 6. AwaitUnsafeOnCompleted
        [DebuggerHidden]
        [SecuritySafeCritical]
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            awaiter.OnCompleted(stateMachine.MoveNext);
        }

        // 7. Start
        [DebuggerHidden]
        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            stateMachine.MoveNext();
        }

        // 8. SetStateMachine
        [DebuggerHidden]
        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
        }
    }
}