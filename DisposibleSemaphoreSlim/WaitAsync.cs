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
        //所有async重载咋又不跟着MSDN走。。。。。
        //鉴于这玩意不讲武德会弹出其他异常，所以用一个包装函数转换下从而符合MSDN
        private static async Task ConvertException(Func<Task> act)
        {
            try
            {
                await act();
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
                return await act();
            }
            catch (TaskCanceledException e)
            {
                throw new OperationCanceledException(e.ToString(), e);
            }
        }
        ///<inheritdoc cref="SemaphoreSlim.WaitAsync()"/>
        public async Task<DisposableSemaphoreUnit> WaitAsync()
        {
            using (await _disposeLock.ReaderLockAsync())
            {
                _assertNotDisposed();
                await _instance.WaitAsync();
                return _createUnit(true);
            }
        }

        ///<inheritdoc cref="SemaphoreSlim.WaitAsync(CancellationToken)"/>
        public async Task<DisposableSemaphoreUnit> WaitAsync(CancellationToken token)
        {
            using (await _disposeLock.ReaderLockAsync())
            {
                _assertNotDisposed();
                await ConvertException((async () => await _instance.WaitAsync(token)));
                return _createUnit(true);
            }
        }
        ///<inheritdoc cref="SemaphoreSlim.WaitAsync(int)"/>
        public async Task<DisposableSemaphoreUnit> WaitAsync(int ms)
        {
            using (await _disposeLock.ReaderLockAsync())
            {
                _assertNotDisposed();

                return _createUnit(await _instance.WaitAsync(ms));
            }
        }
        ///<inheritdoc cref="SemaphoreSlim.WaitAsync(int,CancellationToken)"/>
        public async Task<DisposableSemaphoreUnit> WaitAsync(int ms, CancellationToken token)
        {
            using (await _disposeLock.ReaderLockAsync())
            {
                _assertNotDisposed();

                return _createUnit(await ConvertException(async () => await _instance.WaitAsync(ms, token)));
            }
        }
        ///<inheritdoc cref="SemaphoreSlim.WaitAsync(TimeSpan)"/>
        public async Task<DisposableSemaphoreUnit> WaitAsync(TimeSpan time)
        {
            using (await _disposeLock.ReaderLockAsync())
            {
                _assertNotDisposed();

                return _createUnit(await _instance.WaitAsync(time));
            }
        }
        ///<inheritdoc cref="SemaphoreSlim.WaitAsync(TimeSpan,CancellationToken)"/>
        public async Task<DisposableSemaphoreUnit> WaitAsync(TimeSpan time, CancellationToken token)
        {
            using (await _disposeLock.ReaderLockAsync())
            {
                _assertNotDisposed();

                return _createUnit(await ConvertException(async () => await _instance.WaitAsync(time, token)));
            }
        }
    }
}
