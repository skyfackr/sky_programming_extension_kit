using System.Threading;

namespace SPEkit.SemaphoreSlimAttribute
{
    public sealed partial class SlotWaitAsyncAttribute
    {
        /// <inheritdoc />
        public SlotWaitAsyncAttribute(SemaphoreSlim semaphore = null, WaitingOption option = null) : base(semaphore,
            option)
        {
        }

        /// <inheritdoc />
        public SlotWaitAsyncAttribute(int initialCount, int? maxCount = null, WaitingOption option = null) : base(
            initialCount, maxCount, option)
        {
        }
    }
}