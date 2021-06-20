using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace SPEkit.DisposableSemaphoreSlim
{
    public sealed partial class DisposableSemaphoreSlim : IDisposable
    {
        //所有async重载咋又不跟着MSDN走。。。。。
        //鉴于这玩意不讲武德会弹出其他异常，所以用一个包装函数转换下从而符合MSDN
        private static async Task ConvertException(Func<Task> act)
        {
            try
            {
                await act().ConfigureAwait(false);
            }
            catch (TaskCanceledException e)
            {
                throw new OperationCanceledException(e.ToString(), e);
            }
        }

        private static async Task<bool> ConvertException(Func<Task<bool>> act)
        {
            try
            {
                return await act().ConfigureAwait(false);
            }
            catch (TaskCanceledException e)
            {
                throw new OperationCanceledException(e.ToString(), e);
            }
        }

        /// <inheritdoc cref="SemaphoreSlim.WaitAsync()" />
        public async Task<DisposableSemaphoreUnit> WaitAsync()
        {
            using (await _disposeLock.ReaderLockAsync().ConfigureAwait(false))
            {
                _assertNotDisposed();
                await _instance.WaitAsync().ConfigureAwait(false);
                return _createUnit(true);
            }
        }

        /// <inheritdoc cref="SemaphoreSlim.WaitAsync(CancellationToken)" />
        public async Task<DisposableSemaphoreUnit> WaitAsync(CancellationToken token)
        {
            using (await _disposeLock.ReaderLockAsync(token).ConfigureAwait(false))
            {
                _assertNotDisposed();
                await ConvertException(async () => await _instance.WaitAsync(token).ConfigureAwait(false))
                    .ConfigureAwait(false);
                return _createUnit(true);
            }
        }

        /// <inheritdoc cref="SemaphoreSlim.WaitAsync(int)" />
        public async Task<DisposableSemaphoreUnit> WaitAsync(int ms)
        {
            var time = new Stopwatch();
            time.Restart();
            //var token = new CancellationTokenSource(ms).Token;
            using (await _disposeLock.ReaderLockAsync(new CancellationTokenSource(ms).Token).ConfigureAwait(false))
            {
                _assertNotDisposed();
                time.Stop();
                ms -= (int) time.ElapsedMilliseconds;
                return ms <= 0 ? _createUnit(false) : _createUnit(await _instance.WaitAsync(ms).ConfigureAwait(false));
            }
        }

        /// <inheritdoc cref="SemaphoreSlim.WaitAsync(int,CancellationToken)" />
        public async Task<DisposableSemaphoreUnit> WaitAsync(int ms, CancellationToken token)
        {
            var time = new Stopwatch();
            time.Restart();
            using (await _disposeLock
                .ReaderLockAsync(CancellationTokenSource
                    .CreateLinkedTokenSource(token, new CancellationTokenSource(ms).Token).Token).ConfigureAwait(false))
            {
                _assertNotDisposed();
                time.Stop();
                ms -= (int) time.ElapsedMilliseconds;
                if (ms <= 0) return _createUnit(false);
                return _createUnit(
                    await ConvertException(async () => await _instance.WaitAsync(ms, token).ConfigureAwait(false))
                        .ConfigureAwait(false));
            }
        }

        /// <inheritdoc cref="SemaphoreSlim.WaitAsync(TimeSpan)" />
        public async Task<DisposableSemaphoreUnit> WaitAsync(TimeSpan time)
        {
            var timer = new Stopwatch();
            timer.Restart();
            using (await _disposeLock.ReaderLockAsync(new CancellationTokenSource(time).Token).ConfigureAwait(false))
            {
                _assertNotDisposed();
                timer.Stop();
                time -= TimeSpan.FromMilliseconds(timer.ElapsedMilliseconds);
                return time.Milliseconds <= 0
                    ? _createUnit(false)
                    : _createUnit(await _instance.WaitAsync(time).ConfigureAwait(false));
            }
        }

        /// <inheritdoc cref="SemaphoreSlim.WaitAsync(TimeSpan,CancellationToken)" />
        public async Task<DisposableSemaphoreUnit> WaitAsync(TimeSpan time, CancellationToken token)
        {
            var timer = new Stopwatch();
            timer.Restart();
            using (await _disposeLock
                .ReaderLockAsync(CancellationTokenSource
                    .CreateLinkedTokenSource(token, new CancellationTokenSource(time).Token).Token)
                .ConfigureAwait(false))
            {
                _assertNotDisposed();
                timer.Stop();
                time -= TimeSpan.FromMilliseconds(timer.ElapsedMilliseconds);
                if (time.Milliseconds <= 0) return _createUnit(false);
                return _createUnit(
                    await ConvertException(async () => await _instance.WaitAsync(time, token).ConfigureAwait(false))
                        .ConfigureAwait(false));
            }
        }
    }
}