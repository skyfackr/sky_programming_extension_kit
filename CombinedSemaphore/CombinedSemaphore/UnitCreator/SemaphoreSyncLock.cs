using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;

namespace SPEkit.CombinedSemaphore.Utils
{
    [SuppressMessage("ReSharper", "TooWideLocalVariableScope")]
    [SuppressMessage("ReSharper", "InconsistentlySynchronizedField")]
    internal static class SemaphoreSyncLock
    {
        internal static readonly ConcurrentDictionary<Semaphore, (StrongBox<long>, AutoResetEvent)>
            Win32SemaphoresEvent = new();

        internal static readonly ConcurrentDictionary<SemaphoreSlim, (StrongBox<long>, AutoResetEvent)>
            SlimSemaphoresEvent = new();

        internal static void ReleaseAndSetAnother(Semaphore semaphore)
        {
            var (waiting, handle) = Win32SemaphoresEvent[semaphore];
            handle.Set();
            if (Interlocked.Decrement(ref waiting.Value) > 0) return;
            lock (Win32SemaphoresEvent)
            {
                if (Interlocked.Read(ref waiting.Value) > 0) return;
                Win32SemaphoresEvent.TryRemove(semaphore, out _);
                handle.Dispose();
            }
        }

        internal static void ReleaseAndSetAnother(SemaphoreSlim semaphore)
        {
            var (waiting, handle) = SlimSemaphoresEvent[semaphore];
            handle.Set();
            if (Interlocked.Decrement(ref waiting.Value) > 0) return;
            lock (SlimSemaphoresEvent)
            {
                if (Interlocked.Read(ref waiting.Value) > 0) return;
                SlimSemaphoresEvent.TryRemove(semaphore, out _);
                handle.Dispose();
            }
        }

        internal static IDisposable Wait(Semaphore semaphore)
        {
            StrongBox<long> waiting;
            AutoResetEvent handle;
            lock (Win32SemaphoresEvent)
            {
                (waiting, handle) = Win32SemaphoresEvent.GetOrAdd(semaphore,
                    se => (new StrongBox<long>(0), new AutoResetEvent(true)));
                Interlocked.Increment(ref waiting.Value);
            }

            handle.WaitOne();


            return new SemaphoreSyncLockWin32Unit(semaphore);
        }

        internal static IDisposable Wait(SemaphoreSlim semaphore)
        {
            StrongBox<long> waiting;
            AutoResetEvent handle;
            lock (SlimSemaphoresEvent)
            {
                (waiting, handle) = SlimSemaphoresEvent.GetOrAdd(semaphore,
                    se => (new StrongBox<long>(0), new AutoResetEvent(true)));
                Interlocked.Increment(ref waiting.Value);
            }

            handle.WaitOne();


            return new SemaphoreSyncLockSlimUnit(semaphore);
        }
    }

    internal class SemaphoreSyncLockWin32Unit : IDisposable
    {
        private readonly Semaphore m_semaphore;

        private volatile bool m_disposed;

        internal SemaphoreSyncLockWin32Unit(Semaphore semaphore)
        {
            m_semaphore = semaphore;
        }
        //private readonly object m_disposeLock = new();

        public void Dispose()
        {
            //lock (m_disposeLock)
            //{
            if (m_disposed) return;
            SemaphoreSyncLock.ReleaseAndSetAnother(m_semaphore);
            m_disposed = true;
            //}
        }
    }

    internal class SemaphoreSyncLockSlimUnit : IDisposable
    {
        private readonly SemaphoreSlim m_semaphore;

        private volatile bool m_disposed;

        internal SemaphoreSyncLockSlimUnit(SemaphoreSlim semaphore)
        {
            m_semaphore = semaphore;
        }

        //private readonly object m_disposeLock = new();
        public void Dispose()
        {
            //lock (m_disposeLock)
            //{
            if (m_disposed) return;
            SemaphoreSyncLock.ReleaseAndSetAnother(m_semaphore);
            m_disposed = true;
            //}
        }
    }
}