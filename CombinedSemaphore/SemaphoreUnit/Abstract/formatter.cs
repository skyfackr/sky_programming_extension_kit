namespace SPEkit.CombinedSemaphore.Unit
{
    public abstract partial class SemaphoreUnit
    {
        /// <inheritdoc />
        public override string ToString()
        {
            return $"CurrentType:{GetCurrentSemaphore().GetType()}";
        }
    }
}