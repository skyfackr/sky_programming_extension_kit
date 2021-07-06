using System.Threading;

namespace SPEkit.CombinedSemaphore.Unit
{
    public sealed partial class SemaphoreWin32Unit
    {
        internal SemaphoreWin32Unit(Semaphore semaphore)
        {
            m_semaphore = semaphore;
        }
    }
}