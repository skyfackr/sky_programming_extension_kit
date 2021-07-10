using System.Threading;

namespace SPEkit.CombinedSemaphore.Unit
{
    public sealed partial class SemaphoreWin32Unit
    {
        private readonly Semaphore m_semaphore;

        /// <inheritdoc />
        public override object GetCurrentSemaphore()
        {
            return m_semaphore;
        }

        /// <inheritdoc />
        public override Semaphore GetCurrentSemaphoreAsWin32()
        {
            return m_semaphore;
        }

        /// <inheritdoc />
        public override WaitHandle GetWaitHandle()
        {
            return GetCurrentSemaphoreAsWin32();
        }
    }
}