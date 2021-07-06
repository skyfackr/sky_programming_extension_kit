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
        public abstract object GetCurrentSemaphore();

        public virtual Semaphore GetCurrentSemaphoreAsWin32()
        {
            throw new TypeCannotConvertException(typeof(Semaphore));
        }

        public virtual SemaphoreSlim GetCurrentSemaphoreAsSlim()
        {
            throw new TypeCannotConvertException(typeof(SemaphoreSlim));
        }

        public abstract int Release();

        public abstract int Release(int releaseCount);

        public abstract void Wait();

        public abstract bool Wait(int millisecondsTimeout);

        public abstract bool Wait(int millisecondsTimeout, CancellationToken cancellationToken);

        public abstract void Wait(CancellationToken cancellationToken);

        public abstract bool Wait(TimeSpan timeout);

        public abstract bool Wait(TimeSpan timeout, CancellationToken cancellationToken);

        public abstract Task WaitAsync();

        public abstract Task<bool> WaitAsync(int millisecondsTimeout);

        public abstract Task<bool> WaitAsync(int millisecondsTimeout, CancellationToken cancellationToken);

        public abstract Task WaitAsync(CancellationToken cancellationToken);

        public abstract Task<bool> WaitAsync(TimeSpan timeout);

        public abstract Task<bool> WaitAsync(TimeSpan timeout, CancellationToken cancellationToken);

        public abstract WaitHandle GetWaitHandle();
    }
}
