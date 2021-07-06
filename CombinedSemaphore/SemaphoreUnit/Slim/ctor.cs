using System.Threading;

namespace SPEkit.CombinedSemaphore.Unit
{
    public sealed partial class SemaphoreSlimUnit
    {
        internal SemaphoreSlimUnit(SemaphoreSlim semaphore)
        {
            m_semaphoreSlim = semaphore;
        }
    }
}