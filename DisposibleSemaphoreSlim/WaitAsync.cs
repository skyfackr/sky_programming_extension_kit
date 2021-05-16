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
        public async Task<DisposableSemaphoreUnit> WaitAsync()
        {
            using (await _disposeLock.ReaderLockAsync())
            {
                _assertNotDisposed();
                await _instance.WaitAsync();
                return _createUnit(true);
            }
        }

        public async Task<DisposableSemaphoreUnit> WaitAsync(CancellationToken token)
        {
            using (await _disposeLock.ReaderLockAsync())
            {
                _assertNotDisposed();
                await _instance.WaitAsync(token);
                return _createUnit(true);
            }
        }

        public async Task<DisposableSemaphoreUnit> WaitAsync(int ms)
        {
            using (await _disposeLock.ReaderLockAsync())
            {
                _assertNotDisposed();

                return _createUnit(await _instance.WaitAsync(ms));
            }
        }

        public async Task<DisposableSemaphoreUnit> WaitAsync(int ms, CancellationToken token)
        {
            using (await _disposeLock.ReaderLockAsync())
            {
                _assertNotDisposed();

                return _createUnit(await _instance.WaitAsync(ms,token));
            }
        }

        public async Task<DisposableSemaphoreUnit> WaitAsync(TimeSpan time)
        {
            using (await _disposeLock.ReaderLockAsync())
            {
                _assertNotDisposed();

                return _createUnit(await _instance.WaitAsync(time));
            }
        }

        public async Task<DisposableSemaphoreUnit> WaitAsync(TimeSpan time, CancellationToken token)
        {
            using (await _disposeLock.ReaderLockAsync())
            {
                _assertNotDisposed();

                return _createUnit(await _instance.WaitAsync(time,token));
            }
        }
    }
}
