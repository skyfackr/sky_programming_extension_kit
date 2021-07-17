using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
//TODO code review
namespace SPEkit.CombinedSemaphore.Utils
{
    internal static class SemaphoreSyncLock
    {
        internal static readonly ConcurrentDictionary<Semaphore, (StrongBox<long>, AutoResetEvent)> Win32SemaphoresEvent = new();
        internal static readonly ConcurrentDictionary<SemaphoreSlim, (StrongBox<long>, AutoResetEvent)> SlimSemaphoresEvent = new();

        internal static void ReleaseAndSetAnother(Semaphore semaphore)
        {
            var (waiting, handle) = Win32SemaphoresEvent[semaphore];
            handle.Set();
            if (Interlocked.Decrement(ref waiting.Value) <= 0)
            {
                lock (Win32SemaphoresEvent)
                {
                    if (Interlocked.Read(ref waiting.Value) <= 0)
                    {
                        Win32SemaphoresEvent.TryRemove(semaphore, out _);
                    }
                }
            }
        }
        internal static void ReleaseAndSetAnother(SemaphoreSlim semaphore)
        {
            var (waiting, handle) = SlimSemaphoresEvent[semaphore];
            handle.Set();
            if (Interlocked.Decrement(ref waiting.Value)<=0)
            {
                lock (SlimSemaphoresEvent)
                {
                    if (Interlocked.Read(ref waiting.Value) <= 0)
                    {
                        SlimSemaphoresEvent.TryRemove(semaphore, out _);
                    }
                }
            }
        }

        internal static IDisposable Wait(Semaphore semaphore)
        {
            StrongBox<long> waiting;
            AutoResetEvent handle;
            lock (Win32SemaphoresEvent)
            {
                (waiting, handle) = Win32SemaphoresEvent.GetOrAdd(semaphore, se => (new StrongBox<long>(0), new AutoResetEvent(true)));
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
                (waiting, handle) = SlimSemaphoresEvent.GetOrAdd(semaphore, se => (new StrongBox<long>(0), new AutoResetEvent(true)));
                Interlocked.Increment(ref waiting.Value);
            }

            handle.WaitOne();
            
            
            return new SemaphoreSyncLockSlimUnit(semaphore);
        }
    }

    internal class SemaphoreSyncLockWin32Unit : IDisposable
    {
        private readonly Semaphore m_semaphore;

        internal SemaphoreSyncLockWin32Unit(Semaphore semaphore)
        {
            m_semaphore = semaphore;
        }

        private volatile bool m_disposed = false;
        private readonly object m_disposeLock = new();

        public void Dispose()
        {
            lock (m_disposeLock)
            {
                if (m_disposed) return; 
                SemaphoreSyncLock.ReleaseAndSetAnother(m_semaphore);
                m_disposed = true;
            }
        }
    }
    internal class SemaphoreSyncLockSlimUnit : IDisposable
    {
        private readonly SemaphoreSlim m_semaphore;

        internal SemaphoreSyncLockSlimUnit(SemaphoreSlim semaphore)
        {
            m_semaphore = semaphore;
        }

        private volatile bool m_disposed = false;
        private readonly object m_disposeLock = new();
        public void Dispose()
        {
            lock (m_disposeLock)
            {
                if (m_disposed) return;
                SemaphoreSyncLock.ReleaseAndSetAnother(m_semaphore);
                m_disposed = true;
            }
        }
    }
}
