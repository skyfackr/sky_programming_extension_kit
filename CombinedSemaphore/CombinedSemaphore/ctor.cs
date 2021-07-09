using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using SPEkit.CombinedSemaphore.Unit;
using SPEkit.CombinedSemaphore.Utils;

namespace SPEkit.CombinedSemaphore.MainClass
{
    public sealed partial class CombinedSemaphore
    {
        private CombinedSemaphore(WaitActionFlag? option = null)
        {
            m_option = option ?? WaitActionFlag.ThrowWhenDisposed;
        }

        public CombinedSemaphore(SemaphoreSlim semaphore, WaitActionFlag? option = null) : this(option)
        {
            m_units = CreateList(new[] {CreateUnit(semaphore)});
        }

        public CombinedSemaphore(Semaphore semaphore, WaitActionFlag? option = null) : this(option)
        {
            m_units = CreateList(new[] {CreateUnit(semaphore)});
        }

        public CombinedSemaphore(SemaphoreUnit semaphore, WaitActionFlag? option = null) : this(option)
        {
            m_units = CreateList(new[] {semaphore});
        }

        public CombinedSemaphore(IEnumerable<SemaphoreSlim> semaphores, WaitActionFlag? option = null) : this(option)
        {
            m_units = CreateList(CreateUnits(semaphores));
        }

        public CombinedSemaphore(IEnumerable<Semaphore> semaphores, WaitActionFlag? option = null) : this(option)
        {
            m_units = CreateList(CreateUnits(semaphores));
        }

        public CombinedSemaphore(IEnumerable<SemaphoreUnit> semaphores, WaitActionFlag? option = null) : this(option)
        {
            m_units = CreateList(semaphores);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IList<SemaphoreUnit> CreateList(IEnumerable<SemaphoreUnit> list)
        {
            return new SemaphoreUnitList(list.ToList());
        }
    }
}