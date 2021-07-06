using System;
using System.Threading;
using System.Threading.Tasks;

namespace SPEkit.CombinedSemaphore.Unit
{
    public sealed partial class SemaphoreSlimUnit
    {
        public override int Release()
        {
            return m_semaphoreSlim.Release();
        }

        public override int Release(int releaseCount)
        {
            return m_semaphoreSlim.Release(releaseCount);
        }

        public override void Wait()
        {
            m_semaphoreSlim.Wait();
        }

        public override bool Wait(int millisecondsTimeout)
        {
            return m_semaphoreSlim.Wait(millisecondsTimeout);
        }

        public override bool Wait(int millisecondsTimeout, CancellationToken cancellationToken)
        {
            return m_semaphoreSlim.Wait(millisecondsTimeout, cancellationToken);
        }

        public override void Wait(CancellationToken cancellationToken)
        {
            m_semaphoreSlim.Wait(cancellationToken);
        }

        public override bool Wait(TimeSpan timeout)
        {
            return m_semaphoreSlim.Wait(timeout);
        }

        public override bool Wait(TimeSpan timeout, CancellationToken cancellationToken)
        {
            return m_semaphoreSlim.Wait(timeout, cancellationToken);
        }

        public override Task WaitAsync()
        {
            return m_semaphoreSlim.WaitAsync();
        }

        public override Task<bool> WaitAsync(int millisecondsTimeout)
        {
            return m_semaphoreSlim.WaitAsync(millisecondsTimeout);
        }

        public override Task<bool> WaitAsync(int millisecondsTimeout, CancellationToken cancellationToken)
        {
            return m_semaphoreSlim.WaitAsync(millisecondsTimeout, cancellationToken);
        }

        public override Task WaitAsync(CancellationToken cancellationToken)
        {
            return m_semaphoreSlim.WaitAsync(cancellationToken);
        }

        public override Task<bool> WaitAsync(TimeSpan timeout)
        {
            return m_semaphoreSlim.WaitAsync(timeout);
        }

        public override Task<bool> WaitAsync(TimeSpan timeout, CancellationToken cancellationToken)
        {
            return m_semaphoreSlim.WaitAsync(timeout, cancellationToken);
        }
    }
}