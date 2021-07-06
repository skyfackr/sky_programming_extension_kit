using System;
using System.Threading;
using System.Threading.Tasks;

namespace SPEkit.CombinedSemaphore.Unit
{
    public sealed partial class SemaphoreWin32Unit
    {
        public override int Release()
        {
            return m_semaphore.Release();
        }

        public override int Release(int releaseCount)
        {
            return m_semaphore.Release(releaseCount);
        }

        public override void Wait()
        {
            m_semaphore.WaitOne();
        }

        public override bool Wait(int millisecondsTimeout)
        {
            return m_semaphore.WaitOne(millisecondsTimeout);
        }

        public override bool Wait(int millisecondsTimeout, CancellationToken cancellationToken)
        {
            return Wait(TimeSpan.FromMilliseconds(millisecondsTimeout), cancellationToken);
        }

        public override void Wait(CancellationToken cancellationToken)
        {
            var ans = WaitHandle.WaitAny(new[]
            {
                m_semaphore,
                cancellationToken.WaitHandle
            });
            if (ans == 1) return;
            throw new OperationCanceledException();
        }

        public override bool Wait(TimeSpan timeout)
        {
            return m_semaphore.WaitOne(timeout);
        }

        public override bool Wait(TimeSpan timeout, CancellationToken cancellationToken)
        {
            var ans = WaitHandle.WaitAny(new[]
            {
                m_semaphore,
                cancellationToken.WaitHandle
            }, timeout);
            return ans switch
            {
                WaitHandle.WaitTimeout => false,
                2 => throw new OperationCanceledException(),
                _ => true
            };
        }

        public override async Task WaitAsync()
        {
            await Task.Run(Wait);
        }

        public override async Task<bool> WaitAsync(int millisecondsTimeout)
        {
            return await Task.Run(() => Wait(millisecondsTimeout));
        }

        public override async Task<bool> WaitAsync(int millisecondsTimeout, CancellationToken cancellationToken)
        {
            return await Task.Run(() => Wait(millisecondsTimeout, cancellationToken), CancellationToken.None);
        }

        public override async Task WaitAsync(CancellationToken cancellationToken)
        {
            await Task.Run(() => Wait(cancellationToken), CancellationToken.None);
        }

        public override async Task<bool> WaitAsync(TimeSpan timeout)
        {
            return await Task.Run(() => Wait(timeout));
        }

        public override async Task<bool> WaitAsync(TimeSpan timeout, CancellationToken cancellationToken)
        {
            return await Task.Run(() => Wait(timeout, cancellationToken), CancellationToken.None);
        }
    }
}