namespace SPEkit.SemaphoreSlimAttribute
{
    public sealed partial class SlotWaitAsyncAttribute
    {
        /// <inheritdoc />
        public SlotWaitAsyncAttribute()
        {
        }

        /// <inheritdoc />
        public SlotWaitAsyncAttribute(int initialCount, int maxCount = -1) : base(initialCount, maxCount)
        {
        }
    }
}