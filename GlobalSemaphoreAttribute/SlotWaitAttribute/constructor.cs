namespace SPEkit.SemaphoreSlimAttribute
{
    public sealed partial class SlotWaitAttribute
    {
        /// <inheritdoc />
        public SlotWaitAttribute()
        {
        }

        /// <inheritdoc />
        public SlotWaitAttribute(int initialCount, int maxCount = -1) : base(initialCount, maxCount)
        {
        }
    }
}