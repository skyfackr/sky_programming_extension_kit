using System;
using System.Collections.Generic;
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
        public static bool IsCleanIntervalSet => s_interval != null;

        public static int CleanCreateUnitCache()
        {
            var ans = 0;
            using (s_win32DisposeCheckLock.WriterLock())
            {
                var result1 = Parallel.ForEach(s_win32Cache, pair =>
                {
                    try
                    {
                        var handle = pair.Value.GetWaitHandle().SafeWaitHandle;
                        if (handle.IsInvalid || handle.IsClosed) throw new ObjectDisposedException(null);
                    }
                    catch (ObjectDisposedException)
                    {
                        s_win32Cache.Remove(pair.Key);
                        Interlocked.Increment(ref ans);
                    }
                });
            }

            using (s_slimDisposeCheckLock.WriterLock())
            {
                var result2 = Parallel.ForEach(s_slimCache, pair =>
                {
                    try
                    {
                        var handle = pair.Value.GetWaitHandle().SafeWaitHandle;
                        if (handle.IsInvalid || handle.IsClosed) throw new ObjectDisposedException(null);
                    }
                    catch (ObjectDisposedException)
                    {
                        s_slimCache.Remove(pair.Key);
                        Interlocked.Increment(ref ans);
                    }
                });
            }

            return ans;
        }

        public static void SetCleanInterval(TimeSpan waitPerExecute)
        {
            lock (s_interval)
            {
                if (IsCleanIntervalSet) s_interval.Stop();
                s_interval = new CleanerCirculation(() => { _ = CleanCreateUnitCache(); }, waitPerExecute);
            }
        }

        public static void StopCleanInterval()
        {
            lock (s_interval)
            {
                s_interval.Stop();
                s_interval = null;
            }
        }
    }
}