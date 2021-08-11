using System;
using System.Threading;
using System.Threading.Tasks;

namespace SPEkit.CombinedSemaphore.Unit
{
    public sealed partial class SemaphoreSlimUnit
    {
        /// <inheritdoc />
        public override int Release()
        {
            return m_semaphoreSlim.Release();
        }

        /// <inheritdoc />
        public override int Release(int releaseCount)
        {
            return m_semaphoreSlim.Release(releaseCount);
        }

        /// <inheritdoc />
        public override void Wait()
        {
            m_semaphoreSlim.Wait();
        }

        /// <inheritdoc />
        public override bool Wait(int millisecondsTimeout)
        {
            return m_semaphoreSlim.Wait(millisecondsTimeout);
        }

        /// <inheritdoc />
        public override bool Wait(int millisecondsTimeout, CancellationToken cancellationToken)
        {
            return m_semaphoreSlim.Wait(millisecondsTimeout, cancellationToken);
        }

        /// <inheritdoc />
        public override void Wait(CancellationToken cancellationToken)
        {
            m_semaphoreSlim.Wait(cancellationToken);
        }

        /// <inheritdoc />
        public override bool Wait(TimeSpan timeout)
        {
            return m_semaphoreSlim.Wait(timeout);
        }

        /// <inheritdoc />
        public override bool Wait(TimeSpan timeout, CancellationToken cancellationToken)
        {
            return m_semaphoreSlim.Wait(timeout, cancellationToken);
        }

        /// <inheritdoc />
        public override Task WaitAsync()
        {
            return m_semaphoreSlim.WaitAsync();
        }

        /// <inheritdoc />
        public override Task<bool> WaitAsync(int millisecondsTimeout)
        {
            return m_semaphoreSlim.WaitAsync(millisecondsTimeout);
        }

        /// <inheritdoc />
        public override async Task<bool> WaitAsync(int millisecondsTimeout, CancellationToken cancellationToken)
        {
            try
            {
                return await m_semaphoreSlim.WaitAsync(millisecondsTimeout, cancellationToken).ConfigureAwait(false);
            }
            catch (TaskCanceledException e)
            {
                throw new OperationCanceledException(null, e, cancellationToken);
            }
        }

        /// <inheritdoc />
        public override async Task WaitAsync(CancellationToken cancellationToken)
        {
            try
            {
                await m_semaphoreSlim.WaitAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (TaskCanceledException e)
            {
                throw new OperationCanceledException(null, e, cancellationToken);
            }
        }

        /// <inheritdoc />
        public override Task<bool> WaitAsync(TimeSpan timeout)
        {
            return m_semaphoreSlim.WaitAsync(timeout);
        }

        /// <inheritdoc />
        public override async Task<bool> WaitAsync(TimeSpan timeout, CancellationToken cancellationToken)
        {
            try
            {
                return await m_semaphoreSlim.WaitAsync(timeout, cancellationToken).ConfigureAwait(false);
            }
            catch (TaskCanceledException e)
            {
                throw new OperationCanceledException(null, e, cancellationToken);
            }
        }
    }
}