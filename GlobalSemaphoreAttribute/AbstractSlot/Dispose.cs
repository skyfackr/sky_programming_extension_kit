using System;
using System.Threading.Tasks;

namespace SPEkit.SemaphoreSlimAttribute
{
    public abstract partial class AbstractSlot : IDisposable, IAsyncDisposable
    {
        public async ValueTask DisposeAsync()
        {
            await Task.Run(Dispose);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing) CurrentSemaphore?.Dispose();
        }

        protected bool CheckDisposed()
        {
            try
            {
                _ = CurrentSemaphore.AvailableWaitHandle;
                return false;
            }
            catch (ObjectDisposedException)
            {
                return true;
            }
        }

        public virtual bool IsDisposed()
        {
            return CheckDisposed();
        }
    }
}