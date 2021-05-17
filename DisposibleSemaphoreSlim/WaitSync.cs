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
        ///<inheritdoc cref="SemaphoreSlim.Wait()"/>
        public DisposableSemaphoreUnit Wait()
        {
            using (_disposeLock.ReaderLock())
            {
                _assertNotDisposed();
                _instance.Wait();
                return _createUnit(true);
            }
        }
        ///<inheritdoc cref="SemaphoreSlim.Wait(CancellationToken)"/>
        public DisposableSemaphoreUnit Wait(CancellationToken token)
        {
            using (_disposeLock.ReaderLock())
            {
                _assertNotDisposed();
                _instance.Wait(token);
                return _createUnit(true);
            }
        }
        ///<inheritdoc cref="SemaphoreSlim.Wait(int)"/>
        public DisposableSemaphoreUnit Wait(int ms)
        {
            using (_disposeLock.ReaderLock())
            {
                _assertNotDisposed();

                return _createUnit(_instance.Wait(ms));
            }
        }
        ///<inheritdoc cref="SemaphoreSlim.Wait(int,CancellationToken)"/>
        public DisposableSemaphoreUnit Wait(int ms, CancellationToken token)
        {
            using (_disposeLock.ReaderLock())
            {
                _assertNotDisposed();

                return _createUnit(_instance.Wait(ms,token));
            }
        }
        ///<inheritdoc cref="SemaphoreSlim.Wait(TimeSpan)"/>
        public DisposableSemaphoreUnit Wait(TimeSpan time)
        {
            using (_disposeLock.ReaderLock())
            {
                _assertNotDisposed();

                return _createUnit(_instance.Wait(time));
            }
        }
        ///<inheritdoc cref="SemaphoreSlim.Wait(TimeSpan,CancellationToken)"/>
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
