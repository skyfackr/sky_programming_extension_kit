using System.Threading;

namespace SPEkit.SemaphoreSlimAttribute
{
    public abstract partial class AbstractSlot
    {
        protected AbstractSlot(SemaphoreSlim semaphore = null, WaitingOption option = null) : this(option)
        {
            if (semaphore != null) Initialize(semaphore);
        }

        protected AbstractSlot(int initialCount, int? maxCount = null, WaitingOption option = null) : this(option)
        {
            Initialize(maxCount == null
                ? new SemaphoreSlim(initialCount)
                : new SemaphoreSlim(initialCount, maxCount.Value));
        }

        private AbstractSlot(WaitingOption option)
        {
            m_option = option ?? new WaitingOption();
        }
    }
}