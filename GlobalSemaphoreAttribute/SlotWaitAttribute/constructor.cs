using System.Threading;

namespace SPEkit.SemaphoreSlimAttribute
{
    public sealed partial class SlotWaitAttribute
    {
        /// <inheritdoc />
        public SlotWaitAttribute(SemaphoreSlim semaphore = null, WaitingOption option = null) : base(semaphore, option)
        {
        }

        /// <inheritdoc />
        public SlotWaitAttribute(int initialCount, int? maxCount = null, WaitingOption option = null) : base(
            initialCount, maxCount, option)
        {
        }
    }
}