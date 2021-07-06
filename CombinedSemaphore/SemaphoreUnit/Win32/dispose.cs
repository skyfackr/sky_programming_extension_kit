namespace SPEkit.CombinedSemaphore.Unit
{
    public sealed partial class SemaphoreWin32Unit
    {
        protected override void Dispose(bool disposing)
        {
            if (disposing) m_semaphore?.Dispose();
        }
    }
}