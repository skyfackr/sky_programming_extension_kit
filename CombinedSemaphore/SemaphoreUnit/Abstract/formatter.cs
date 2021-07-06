namespace SPEkit.CombinedSemaphore.Unit
{
    public abstract partial class SemaphoreUnit
    {
        public override string ToString()
        {
            return $"CurrentType:{GetCurrentSemaphore().GetType()}";
        }
    }
}