namespace SPEkit.CombinedSemaphore.Unit
{
    public sealed partial class SemaphoreSlimUnit
    {
        /// <inheritdoc />
        public override string ToString()
        {
            return base.ToString() + $", {nameof(CurrentCount)}:{CurrentCount}";
        }
    }
}