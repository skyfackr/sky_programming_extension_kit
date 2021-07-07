using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SPEkit.CombinedSemaphore.Unit;

namespace SPEkit.CombinedSemaphore.MainClass
{
    public sealed partial class CombinedSemaphore
    {
        private readonly IList<SemaphoreUnit> m_units;


        public bool TryAdd(SemaphoreUnit semaphore)
        {
            AssertNotDisposed();
            if (Contains(semaphore)) return false;
            Add(semaphore);
            return true;
        }

        public IEnumerable<WaitHandle> GetWaitHandles()
        {
            AssertNotDisposed();
            return from unit in m_units
                select unit.GetWaitHandle();
        }

        public bool TryAdd(SemaphoreSlim semaphore)
        {
            AssertNotDisposed();
            return TryAdd(CreateUnit(semaphore));
        }

        public bool TryAdd(Semaphore semaphore)
        {
            AssertNotDisposed();
            return TryAdd(CreateUnit(semaphore));
        }

        public bool Contains(SemaphoreSlim semaphore)
        {
            AssertNotDisposed();
            return GetAllSemaphoreSlim().Contains(semaphore);
        }

        public bool Contains(Semaphore semaphore)
        {
            AssertNotDisposed();
            return GetAllSemaphoreWin32().Contains(semaphore);
        }

        public IList<SemaphoreUnit> GetUnitList()
        {
            AssertNotDisposed();
            return m_units.ToList();
        }

        public IEnumerable<Semaphore> GetAllSemaphoreWin32()
        {
            AssertNotDisposed();
            return from unit in m_units
                where unit.GetCurrentSemaphore() is Semaphore
                select unit.GetCurrentSemaphoreAsWin32();
        }

        public IEnumerable<SemaphoreSlim> GetAllSemaphoreSlim()
        {
            AssertNotDisposed();
            return from unit in m_units
                where unit.GetCurrentSemaphore() is SemaphoreSlim
                select unit.GetCurrentSemaphoreAsSlim();
        }

        public int RemoveAllDisposedUnit()
        {
            AssertNotDisposed();
            var ans = 0;
            var result = Parallel.ForEach(m_units, unit =>
            {
                try
                {
                    var handle = unit.GetWaitHandle().SafeWaitHandle;
                    if (handle.IsInvalid || handle.IsClosed) throw new ObjectDisposedException(null);
                }
                catch (ObjectDisposedException)
                {
                    m_units.Remove(unit);
                    Interlocked.Increment(ref ans);
                }
            });
            return ans;
        }
    }
}