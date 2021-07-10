namespace SPEkit.CombinedSemaphore.Unit
{
    public sealed partial class SemaphoreSlimUnit
    {
        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing) m_semaphoreSlim?.Dispose();
        }
    }
}