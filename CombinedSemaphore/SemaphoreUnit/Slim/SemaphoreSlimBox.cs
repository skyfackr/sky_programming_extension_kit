using System.Threading;

namespace SPEkit.CombinedSemaphore.Unit
{
    public sealed partial class SemaphoreSlimUnit
    {
        private readonly SemaphoreSlim m_semaphoreSlim;

        public int CurrentCount => m_semaphoreSlim.CurrentCount;

        public override object GetCurrentSemaphore()
        {
            return m_semaphoreSlim;
        }

        public override SemaphoreSlim GetCurrentSemaphoreAsSlim()
        {
            return m_semaphoreSlim;
        }

        public override WaitHandle GetWaitHandle()
        {
            return m_semaphoreSlim.AvailableWaitHandle;
        }
    }
}