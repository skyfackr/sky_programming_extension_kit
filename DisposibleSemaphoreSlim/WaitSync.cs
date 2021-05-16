using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SPEkit.DisposableSemaphoreSlim
{
    public sealed partial class DisposableSemaphoreSlim : IDisposable
    {
        public DisposableSemaphoreUnit Wait()
        {
            using (_disposeLock.ReaderLock())
            {
                _assertNotDisposed();
                _instance.Wait();
                return _createUnit(true);
            }
        }

        public DisposableSemaphoreUnit Wait(CancellationToken token)
        {
            using (_disposeLock.ReaderLock())
            {
                _assertNotDisposed();
                _instance.Wait(token);
                return _createUnit(true);
            }
        }

        public DisposableSemaphoreUnit Wait(int ms)
        {
            using (_disposeLock.ReaderLock())
            {
                _assertNotDisposed();

                return _createUnit(_instance.Wait(ms));
            }
        }

        public DisposableSemaphoreUnit Wait(int ms, CancellationToken token)
        {
            using (_disposeLock.ReaderLock())
            {
                _assertNotDisposed();

                return _createUnit(_instance.Wait(ms,token));
            }
        }

        public DisposableSemaphoreUnit Wait(TimeSpan time)
        {
            using (_disposeLock.ReaderLock())
            {
                _assertNotDisposed();

                return _createUnit(_instance.Wait(time));
            }
        }

        public DisposableSemaphoreUnit Wait(TimeSpan time, CancellationToken token)
        {
            using (_disposeLock.ReaderLock())
            {
                _assertNotDisposed();

                return _createUnit(_instance.Wait(time,token));
            }
        }
    }
}
