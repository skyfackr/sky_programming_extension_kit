using System.Threading;

namespace SPEkit.SemaphoreSlimAttribute
{
    public sealed partial class SlotWaitAttribute
    {
        public SlotWaitAttribute(SemaphoreSlim semaphore = null, WaitingOption option = null) : base(semaphore, option)
        {
        }

        public SlotWaitAttribute(int initialCount, int? maxCount = null, WaitingOption option = null) : base(
            initialCount, maxCount, option)
        {
        }
    }
}