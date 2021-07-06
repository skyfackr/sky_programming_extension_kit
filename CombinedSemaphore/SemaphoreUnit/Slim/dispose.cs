namespace SPEkit.CombinedSemaphore.Unit
{
    public sealed partial class SemaphoreSlimUnit
    {
        protected override void Dispose(bool disposing)
        {
            if (disposing) m_semaphoreSlim?.Dispose();
        }
    }
}