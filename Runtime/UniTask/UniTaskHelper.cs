using System;
using System.Collections.Generic;

namespace Bear
{
    public static class UniTaskHelper
    {
        private class CoroutineBlocker
        {
            private int count;

            private List<UniTask> tcss = new List<UniTask>();

            public CoroutineBlocker(int count)
            {
                this.count = count;
            }

            public async UniTask WaitAsync()
            {
                --this.count;
                if (this.count < 0)
                {
                    return;
                }
                if (this.count == 0)
                {
                    List<UniTask> t = this.tcss;
                    this.tcss = null;
                    foreach (UniTask ttcs in t)
                    {
                        ttcs.SetResult();
                    }

                    return;
                }
                UniTask tcs = UniTask.Create(true);

                tcss.Add(tcs);
                await tcs;
            }
        }

        public static async UniTask<bool> WaitAny<T>(UniTask<T>[] tasks, UniCancellationToken cancellationToken = null)
        {
            if (tasks.Length == 0)
            {
                return false;
            }

            CoroutineBlocker coroutineBlocker = new CoroutineBlocker(2);

            foreach (UniTask<T> task in tasks)
            {
                RunOneTask(task).Coroutine();
            }

            async UniVoid RunOneTask(UniTask<T> task)
            {
                await task;
                await coroutineBlocker.WaitAsync();
            }

            await coroutineBlocker.WaitAsync();

            if (cancellationToken == null)
            {
                return true;
            }

            return !cancellationToken.IsCancel();
        }

        public static async UniTask<bool> WaitAny(UniTask[] tasks, UniCancellationToken cancellationToken = null)
        {
            if (tasks.Length == 0)
            {
                return false;
            }

            CoroutineBlocker coroutineBlocker = new CoroutineBlocker(2);

            foreach (UniTask task in tasks)
            {
                RunOneTask(task).Coroutine();
            }

            async UniVoid RunOneTask(UniTask task)
            {
                await task;
                await coroutineBlocker.WaitAsync();
            }

            await coroutineBlocker.WaitAsync();

            if (cancellationToken == null)
            {
                return true;
            }

            return !cancellationToken.IsCancel();
        }

        public static async UniTask<bool> WaitAll<T>(UniTask<T>[] tasks, UniCancellationToken cancellationToken = null)
        {
            if (tasks.Length == 0)
            {
                return false;
            }

            CoroutineBlocker coroutineBlocker = new CoroutineBlocker(tasks.Length + 1);

            foreach (UniTask<T> task in tasks)
            {
                RunOneTask(task).Coroutine();
            }

            async UniVoid RunOneTask(UniTask<T> task)
            {
                await task;
                await coroutineBlocker.WaitAsync();
            }

            await coroutineBlocker.WaitAsync();

            if (cancellationToken == null)
            {
                return true;
            }

            return !cancellationToken.IsCancel();
        }

        public static async UniTask<bool> WaitAll<T>(List<UniTask<T>> tasks, UniCancellationToken cancellationToken = null)
        {
            if (tasks.Count == 0)
            {
                return false;
            }

            CoroutineBlocker coroutineBlocker = new CoroutineBlocker(tasks.Count + 1);

            foreach (UniTask<T> task in tasks)
            {
                RunOneTask(task).Coroutine();
            }

            async UniVoid RunOneTask(UniTask<T> task)
            {
                await task;
                await coroutineBlocker.WaitAsync();
            }

            await coroutineBlocker.WaitAsync();

            if (cancellationToken == null)
            {
                return true;
            }

            return !cancellationToken.IsCancel();
        }

        public static async UniTask<bool> WaitAll(UniTask[] tasks, UniCancellationToken cancellationToken = null)
        {
            if (tasks.Length == 0)
            {
                return false;
            }

            CoroutineBlocker coroutineBlocker = new CoroutineBlocker(tasks.Length + 1);

            foreach (UniTask task in tasks)
            {
                RunOneTask(task).Coroutine();
            }

            await coroutineBlocker.WaitAsync();

            async UniVoid RunOneTask(UniTask task)
            {
                await task;
                await coroutineBlocker.WaitAsync();
            }

            if (cancellationToken == null)
            {
                return true;
            }

            return !cancellationToken.IsCancel();
        }

        public static async UniTask<bool> WaitAll(List<UniTask> tasks, UniCancellationToken cancellationToken = null)
        {
            if (tasks.Count == 0)
            {
                return false;
            }

            CoroutineBlocker coroutineBlocker = new CoroutineBlocker(tasks.Count + 1);

            foreach (UniTask task in tasks)
            {
                RunOneTask(task).Coroutine();
            }

            await coroutineBlocker.WaitAsync();

            async UniVoid RunOneTask(UniTask task)
            {
                await task;
                await coroutineBlocker.WaitAsync();
            }

            if (cancellationToken == null)
            {
                return true;
            }

            return !cancellationToken.IsCancel();
        }
    }
}