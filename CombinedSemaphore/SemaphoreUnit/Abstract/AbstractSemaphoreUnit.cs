using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SPEkit.CombinedSemaphore.error;

namespace SPEkit.CombinedSemaphore.Unit
{
    public abstract partial class SemaphoreUnit
    {
        /// <summary>
        /// 获取当前信号量实例的装箱对象
        /// </summary>
        /// <returns></returns>
        public abstract object GetCurrentSemaphore();

        /// <summary>
        /// 获取当前<see cref="Semaphore"/>信号量实例
        /// </summary>
        /// <exception cref="TypeCannotConvertException"></exception>
        /// <returns></returns>
        /// <remarks>如果不兼容会导致无法转换并丢出异常</remarks>
        public virtual Semaphore GetCurrentSemaphoreAsWin32()
        {
            throw new TypeCannotConvertException(typeof(Semaphore));
        }

        /// <summary>
        /// 获取当前<see cref="SemaphoreSlim"/>信号量实例
        /// </summary>
        /// <exception cref="TypeCannotConvertException"></exception>
        /// <returns></returns>
        /// <remarks>如果不兼容会导致无法转换并丢出异常</remarks>
        public virtual SemaphoreSlim GetCurrentSemaphoreAsSlim()
        {
            throw new TypeCannotConvertException(typeof(SemaphoreSlim));
        }

        ///<inheritdoc cref="SemaphoreSlim.Release()"/>
        public abstract int Release();

        ///<inheritdoc cref="SemaphoreSlim.Release(int)"/>
        public abstract int Release(int releaseCount);

        ///<inheritdoc cref="SemaphoreSlim.Wait()"/>
        public abstract void Wait();

        ///<inheritdoc cref="SemaphoreSlim.Wait(int)"/>
        public abstract bool Wait(int millisecondsTimeout);

        ///<inheritdoc cref="SemaphoreSlim.Wait(int,CancellationToken)"/>
        public abstract bool Wait(int millisecondsTimeout, CancellationToken cancellationToken);

        ///<inheritdoc cref="SemaphoreSlim.Wait(CancellationToken)"/>
        public abstract void Wait(CancellationToken cancellationToken);

        ///<inheritdoc cref="SemaphoreSlim.Wait(TimeSpan)"/>
        public abstract bool Wait(TimeSpan timeout);

        ///<inheritdoc cref="SemaphoreSlim.Wait(TimeSpan,CancellationToken)"/>
        public abstract bool Wait(TimeSpan timeout, CancellationToken cancellationToken);

        ///<inheritdoc cref="SemaphoreSlim.WaitAsync()"/>
        public abstract Task WaitAsync();

        ///<inheritdoc cref="SemaphoreSlim.WaitAsync(int)"/>
        public abstract Task<bool> WaitAsync(int millisecondsTimeout);

        ///<inheritdoc cref="SemaphoreSlim.WaitAsync(int,CancellationToken)"/>
        public abstract Task<bool> WaitAsync(int millisecondsTimeout, CancellationToken cancellationToken);

        ///<inheritdoc cref="SemaphoreSlim.WaitAsync(CancellationToken)"/>
        public abstract Task WaitAsync(CancellationToken cancellationToken);

        ///<inheritdoc cref="SemaphoreSlim.WaitAsync(TimeSpan)"/>
        public abstract Task<bool> WaitAsync(TimeSpan timeout);

        ///<inheritdoc cref="SemaphoreSlim.WaitAsync(TimeSpan,CancellationToken)"/>
        public abstract Task<bool> WaitAsync(TimeSpan timeout, CancellationToken cancellationToken);

        /// <summary>
        /// 获取当前信号量实例的一个可用的<see cref="WaitHandle"/>
        /// </summary>
        /// <returns>如果信号量为<see cref="Semaphore"/>实例，会返回此信号量实例且不会触发<see cref="ObjectDisposedException"/></returns>
        /// <exception cref="ObjectDisposedException"></exception>
        public abstract WaitHandle GetWaitHandle();
    }
}
