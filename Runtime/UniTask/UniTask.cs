using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace Bear
{
    [AsyncMethodBuilder(typeof (UniAsyncTaskMethodBuilder))]
    public class UniTask: ICriticalNotifyCompletion
    {
        public static Action<Exception> ExceptionHandler;
        
        public static UniTaskCompleted CompletedTask
        {
            get
            {
                return new UniTaskCompleted();
            }
        }

        private static readonly Queue<UniTask> queue = new Queue<UniTask>();

        /// <summary>
        /// 请不要随便使用UniTask的对象池，除非你完全搞懂了UniTask!!!
        /// 假如开启了池,await之后不能再操作UniTask，否则可能操作到再次从池中分配出来的UniTask，产生灾难性的后果
        /// SetResult的时候请现将tcs置空，避免多次对同一个UniTask SetResult
        /// </summary>
        public static UniTask Create(bool fromPool = false)
        {
            if (!fromPool)
            {
                return new UniTask();
            }
            
            if (queue.Count == 0)
            {
                return new UniTask() {fromPool = true};    
            }
            return queue.Dequeue();
        }

        private void Recycle()
        {
            if (!this.fromPool)
            {
                return;
            }
            
            this.state = AwaiterStatus.Pending;
            this.callback = null;
            queue.Enqueue(this);
            // 太多了，回收一下
            if (queue.Count > 1000)
            {
                queue.Clear();
            }
        }

        private bool fromPool;
        private AwaiterStatus state;
        private object callback; // Action or ExceptionDispatchInfo

        private UniTask()
        {
        }
        
        [DebuggerHidden]
        private async UniVoid InnerCoroutine()
        {
            await this;
        }

        [DebuggerHidden]
        public void Coroutine()
        {
            InnerCoroutine().Coroutine();
        }

        [DebuggerHidden]
        public UniTask GetAwaiter()
        {
            return this;
        }

        
        public bool IsCompleted
        {
            [DebuggerHidden]
            get
            {
                return this.state != AwaiterStatus.Pending;
            }
        }

        [DebuggerHidden]
        public void UnsafeOnCompleted(Action action)
        {
            if (this.state != AwaiterStatus.Pending)
            {
                action?.Invoke();
                return;
            }

            this.callback = action;
        }

        [DebuggerHidden]
        public void OnCompleted(Action action)
        {
            this.UnsafeOnCompleted(action);
        }

        [DebuggerHidden]
        public void GetResult()
        {
            switch (this.state)
            {
                case AwaiterStatus.Succeeded:
                    this.Recycle();
                    break;
                case AwaiterStatus.Faulted:
                    ExceptionDispatchInfo c = this.callback as ExceptionDispatchInfo;
                    this.callback = null;
                    this.Recycle();
                    c?.Throw();
                    break;
                default:
                    throw new NotSupportedException("UniTask does not allow call GetResult directly when task not completed. Please use 'await'.");
            }
        }

        [DebuggerHidden]
        public void SetResult()
        {
            if (this.state != AwaiterStatus.Pending)
            {
                throw new InvalidOperationException("TaskT_TransitionToFinal_AlreadyCompleted");
            }

            this.state = AwaiterStatus.Succeeded;

            Action c = this.callback as Action;
            this.callback = null;
            c?.Invoke();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerHidden]
        public void SetException(Exception e)
        {
            if (this.state != AwaiterStatus.Pending)
            {
                throw new InvalidOperationException("TaskT_TransitionToFinal_AlreadyCompleted");
            }

            this.state = AwaiterStatus.Faulted;

            Action c = this.callback as Action;
            this.callback = ExceptionDispatchInfo.Capture(e);
            c?.Invoke();
        }
    }

    [AsyncMethodBuilder(typeof (UniAsyncTaskMethodBuilder<>))]
    public class UniTask<T>: ICriticalNotifyCompletion
    {
        private static readonly Queue<UniTask<T>> queue = new Queue<UniTask<T>>();
        
        /// <summary>
        /// 请不要随便使用UniTask的对象池，除非你完全搞懂了UniTask!!!
        /// 假如开启了池,await之后不能再操作UniTask，否则可能操作到再次从池中分配出来的UniTask，产生灾难性的后果
        /// SetResult的时候请现将tcs置空，避免多次对同一个UniTask SetResult
        /// </summary>
        public static UniTask<T> Create(bool fromPool = false)
        {
            if (!fromPool)
            {
                return new UniTask<T>();
            }
            
            if (queue.Count == 0)
            {
                return new UniTask<T>() { fromPool = true };    
            }
            return queue.Dequeue();
        }
        
        private void Recycle()
        {
            if (!this.fromPool)
            {
                return;
            }
            this.callback = null;
            this.value = default;
            this.state = AwaiterStatus.Pending;
            queue.Enqueue(this);
            // 太多了，回收一下
            if (queue.Count > 1000)
            {
                queue.Clear();
            }
        }

        private bool fromPool;
        private AwaiterStatus state;
        private T value;
        private object callback; // Action or ExceptionDispatchInfo

        private UniTask()
        {
        }

        [DebuggerHidden]
        private async UniVoid InnerCoroutine()
        {
            await this;
        }

        [DebuggerHidden]
        public void Coroutine()
        {
            InnerCoroutine().Coroutine();
        }

        [DebuggerHidden]
        public UniTask<T> GetAwaiter()
        {
            return this;
        }

        [DebuggerHidden]
        public T GetResult()
        {
            switch (this.state)
            {
                case AwaiterStatus.Succeeded:
                    T v = this.value;
                    this.Recycle();
                    return v;
                case AwaiterStatus.Faulted:
                    ExceptionDispatchInfo c = this.callback as ExceptionDispatchInfo;
                    this.callback = null;
                    this.Recycle();
                    c?.Throw();
                    return default;
                default:
                    throw new NotSupportedException("ETask does not allow call GetResult directly when task not completed. Please use 'await'.");
            }
        }


        public bool IsCompleted
        {
            [DebuggerHidden]
            get
            {
                return state != AwaiterStatus.Pending;
            }
        } 

        [DebuggerHidden]
        public void UnsafeOnCompleted(Action action)
        {
            if (this.state != AwaiterStatus.Pending)
            {
                action?.Invoke();
                return;
            }

            this.callback = action;
        }

        [DebuggerHidden]
        public void OnCompleted(Action action)
        {
            this.UnsafeOnCompleted(action);
        }

        [DebuggerHidden]
        public void SetResult(T result)
        {
            if (this.state != AwaiterStatus.Pending)
            {
                throw new InvalidOperationException("TaskT_TransitionToFinal_AlreadyCompleted");
            }

            this.state = AwaiterStatus.Succeeded;

            this.value = result;

            Action c = this.callback as Action;
            this.callback = null;
            c?.Invoke();
        }
        
        [DebuggerHidden]
        public void SetException(Exception e)
        {
            if (this.state != AwaiterStatus.Pending)
            {
                throw new InvalidOperationException("TaskT_TransitionToFinal_AlreadyCompleted");
            }

            this.state = AwaiterStatus.Faulted;

            Action c = this.callback as Action;
            this.callback = ExceptionDispatchInfo.Capture(e);
            c?.Invoke();
        }
    }
}