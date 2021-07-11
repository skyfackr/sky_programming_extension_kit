using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;
using SPEkit.CombinedSemaphore.Unit;
using SPEkit.CombinedSemaphore.Utils;

namespace SPEkit.CombinedSemaphore.MainClass
{
    public sealed partial class CombinedSemaphore
    {
        private static readonly Dictionary<Semaphore, SemaphoreWin32Unit> s_win32Cache = new();
        private static readonly AsyncReaderWriterLock s_win32DisposeCheckLock = new();
        private static readonly Dictionary<SemaphoreSlim, SemaphoreSlimUnit> s_slimCache = new();
        private static readonly AsyncReaderWriterLock s_slimDisposeCheckLock = new();

        private static CleanerCirculation s_interval;
        /// <summary>
        /// 是否设置了自动循环缓存清理
        /// </summary>
        public static bool IsCleanIntervalSet => s_interval != null;

        /// <summary>
        /// 清理一次<see cref="CreateUnit(System.Threading.Semaphore)"/>的缓存，使缓存中（包括重载）的所有已经被dispose的对象删除
        /// </summary>
        /// <returns>被清除的缓存数量</returns>
        /// <remarks>此代码将分开执行<see cref="SemaphoreWin32Unit"/>和<see cref="SemaphoreSlimUnit"/>的缓存清理并受到读写锁管理，对应缓存清理期间
        /// 相对的<see cref="CreateUnit(System.Threading.Semaphore)"/>或<see cref="CreateUnit(System.Threading.SemaphoreSlim)"/>将被阻塞</remarks>
        public static int CleanCreateUnitCache()
        {
            var ans = 0;
            using (s_win32DisposeCheckLock.WriterLock())
            {
                var result1 = Parallel.ForEach(s_win32Cache, pair =>
                {
                    var (key, value) = pair;
                    try
                    {
                        var handle = value.GetWaitHandle().SafeWaitHandle;
                        if (handle.IsInvalid || handle.IsClosed) throw new ObjectDisposedException(null);
                    }
                    catch (ObjectDisposedException)
                    {
                        s_win32Cache.Remove(key);
                        Interlocked.Increment(ref ans);
                    }
                });
            }

            using (s_slimDisposeCheckLock.WriterLock())
            {
                var result2 = Parallel.ForEach(s_slimCache, pair =>
                {
                    var (key, value) = pair;
                    try
                    {
                        var handle = value.GetWaitHandle().SafeWaitHandle;
                        if (handle.IsInvalid || handle.IsClosed) throw new ObjectDisposedException(null);
                    }
                    catch (ObjectDisposedException)
                    {
                        s_slimCache.Remove(key);
                        Interlocked.Increment(ref ans);
                    }
                });
            }

            return ans;
        }

        /// <summary>
        /// 设置一个自动清理缓存循环，每隔指定时间调用一次<see cref="CleanCreateUnitCache"/>
        /// </summary>
        /// <param name="waitPerExecute">间歇时间</param>
        public static void SetCleanInterval(TimeSpan waitPerExecute)
        {
            lock (s_interval)
            {
                if (IsCleanIntervalSet) s_interval.Stop();
                s_interval = new CleanerCirculation(() =>
                {
                    var ans = CleanCreateUnitCache();
                    CompleteCleanOnceInInterval?.Invoke(ans);
                }, waitPerExecute);
            }
        }

        /// <summary>
        /// 终止<see cref="SetCleanInterval"/>的循环
        /// </summary>
        public static void StopCleanInterval()
        {
            lock (s_interval)
            {
                s_interval?.Stop();
                s_interval = null;
            }
        }

        /// <summary>
        /// 当缓存清理循环中调用完成一次时，调用此事件，传入一个<see cref="int"/>参数作为<see cref="CleanCreateUnitCache"/>的返回值
        /// </summary>
        [SuppressMessage("ReSharper", "EventNeverSubscribedTo.Global")]
        public static event Action<int> CompleteCleanOnceInInterval;
    }
}