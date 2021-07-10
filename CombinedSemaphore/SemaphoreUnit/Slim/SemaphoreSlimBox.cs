using System.Threading;

namespace SPEkit.CombinedSemaphore.Unit
{
    public sealed partial class SemaphoreSlimUnit
    {
        private readonly SemaphoreSlim m_semaphoreSlim;

        ///<inheritdoc cref="SemaphoreSlim.CurrentCount"/>
        public int CurrentCount => m_semaphoreSlim.CurrentCount;

        /// <inheritdoc />
        public override object GetCurrentSemaphore()
        {
            return m_semaphoreSlim;
        }

        /// <inheritdoc />
        public override SemaphoreSlim GetCurrentSemaphoreAsSlim()
        {
            return m_semaphoreSlim;
        }

        /// <inheritdoc />
        public override WaitHandle GetWaitHandle()
        {
            return m_semaphoreSlim.AvailableWaitHandle;
        }
    }
}