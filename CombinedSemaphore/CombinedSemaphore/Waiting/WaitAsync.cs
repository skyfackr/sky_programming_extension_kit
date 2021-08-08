using System;
using System.Threading;
using System.Threading.Tasks;

namespace SPEkit.CombinedSemaphore.MainClass
{
    public sealed partial class CombinedSemaphore
    {
        //private SemaphoreUnit a;

        /// <inheritdoc cref="SemaphoreSlim.WaitAsync()" />
        public Task WaitAsync()
        {
            return Task.Run(() => WaitingProcess(async unit =>
            {
                await unit.WaitAsync().ConfigureAwait(false);
                return true;
            }));
        }

        /// <inheritdoc cref="SemaphoreSlim.WaitAsync(int)" />
        public Task<bool> WaitAsync(int millisecondsTimeout)
        {
            return Task.Run(() => WaitingProcess(unit => unit.WaitAsync(millisecondsTimeout)));
        }

        /// <inheritdoc cref="SemaphoreSlim.WaitAsync(int,CancellationToken)" />
        public Task<bool> WaitAsync(int millisecondsTimeout, CancellationToken cancellationToken)
        {
            return Task.Run(() => WaitingProcess(unit => unit.WaitAsync(millisecondsTimeout, cancellationToken)),
                CancellationToken.None);
        }

        /// <inheritdoc cref="SemaphoreSlim.WaitAsync(CancellationToken)" />
        public Task WaitAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() => WaitingProcess(async unit =>
            {
                await unit.WaitAsync(cancellationToken).ConfigureAwait(false);
                return true;
            }), CancellationToken.None);
        }

        /// <inheritdoc cref="SemaphoreSlim.WaitAsync(TimeSpan)" />
        public Task<bool> WaitAsync(TimeSpan timeout)
        {
            return Task.Run(() => WaitingProcess(unit => unit.WaitAsync(timeout)), CancellationToken.None);
        }

        /// <inheritdoc cref="SemaphoreSlim.WaitAsync(TimeSpan,CancellationToken)" />
        public Task<bool> WaitAsync(TimeSpan timeout, CancellationToken cancellationToken)
        {
            return Task.Run(() => WaitingProcess(unit => unit.WaitAsync(timeout, cancellationToken)),
                CancellationToken.None);
        }
    }
}