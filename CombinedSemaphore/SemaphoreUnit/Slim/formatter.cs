namespace SPEkit.CombinedSemaphore.Unit
{
    public sealed partial class SemaphoreSlimUnit
    {
        public override string ToString()
        {
            return base.ToString() + $", {nameof(CurrentCount)}:{CurrentCount}";
        }
    }
}