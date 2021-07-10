namespace SPEkit.CombinedSemaphore.Unit
{
    public sealed partial class SemaphoreWin32Unit
    {
        /// <inheritdoc />
        public override string ToString()
        {
            return base.ToString() + ". Current count not available.";
        }
    }
}