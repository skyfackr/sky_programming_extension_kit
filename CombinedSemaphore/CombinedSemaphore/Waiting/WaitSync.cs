using System;
using System.Threading;
using System.Threading.Tasks;

namespace SPEkit.CombinedSemaphore.MainClass
{
    public sealed partial class CombinedSemaphore
    {
        //private SemaphoreUnit a;
        /// <inheritdoc cref="SemaphoreSlim.Wait()" />
        public void Wait()
        {
            WaitingProcess(unit =>
            {
                unit.Wait();
                return Task.FromResult(true);
            });
        }

        /// <inheritdoc cref="SemaphoreSlim.Wait(int)" />
        public bool Wait(int millisecondsTimeout)
        {
            return WaitingProcess(unit => Task.FromResult(unit.Wait(millisecondsTimeout)));
        }

        /// <inheritdoc cref="SemaphoreSlim.Wait(int,CancellationToken)" />
        public bool Wait(int millisecondsTimeout, CancellationToken cancellationToken)
        {
            return WaitingProcess(unit => Task.FromResult(unit.Wait(millisecondsTimeout, cancellationToken)));
        }


        /// <inheritdoc cref="SemaphoreSlim.Wait(CancellationToken)" />
        public void Wait(CancellationToken cancellationToken)
        {
            WaitingProcess(unit =>
            {
                unit.Wait(cancellationToken);
                return Task.FromResult(true);
            });
        }

        /// <inheritdoc cref="SemaphoreSlim.Wait(TimeSpan)" />
        public bool Wait(TimeSpan timeout)
        {
            return WaitingProcess(unit => Task.FromResult(unit.Wait(timeout)));
        }

        /// <inheritdoc cref="SemaphoreSlim.Wait(TimeSpan,CancellationToken)" />
        public bool Wait(TimeSpan timeout, CancellationToken cancellationToken)
        {
            return WaitingProcess(unit => Task.FromResult(unit.Wait(timeout, cancellationToken)));
        }
    }
}