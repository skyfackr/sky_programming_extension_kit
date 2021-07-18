using System;
using System.Threading;
using System.Threading.Tasks;

namespace SPEkit.CombinedSemaphore.MainClass
{
    public sealed partial class CombinedSemaphore
    {
        //private SemaphoreUnit a;

        /// <inheritdoc cref="SemaphoreSlim.WaitAsync()" />
        public async Task WaitAsync()
        {
            await Task.Run(() => WaitingProcess(async unit =>
            {
                await unit.WaitAsync().ConfigureAwait(false);
                return true;
            })).ConfigureAwait(false);
        }

        /// <inheritdoc cref="SemaphoreSlim.WaitAsync(int)" />
        public async Task<bool> WaitAsync(int millisecondsTimeout)
        {
            return await Task.Run(() => WaitingProcess(unit => unit.WaitAsync(millisecondsTimeout)))
                .ConfigureAwait(false);
        }

        /// <inheritdoc cref="SemaphoreSlim.WaitAsync(int,CancellationToken)" />
        public async Task<bool> WaitAsync(int millisecondsTimeout, CancellationToken cancellationToken)
        {
            return await Task.Run(() => WaitingProcess(unit => unit.WaitAsync(millisecondsTimeout, cancellationToken)),
                CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc cref="SemaphoreSlim.WaitAsync(CancellationToken)" />
        public async Task WaitAsync(CancellationToken cancellationToken)
        {
            await Task.Run(() => WaitingProcess(async unit =>
            {
                await unit.WaitAsync(cancellationToken).ConfigureAwait(false);
                return true;
            }), CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc cref="SemaphoreSlim.WaitAsync(TimeSpan)" />
        public async Task<bool> WaitAsync(TimeSpan timeout)
        {
            return await Task.Run(() => WaitingProcess(unit => unit.WaitAsync(timeout)), CancellationToken.None)
                .ConfigureAwait(false);
        }

        /// <inheritdoc cref="SemaphoreSlim.WaitAsync(TimeSpan,CancellationToken)" />
        public async Task<bool> WaitAsync(TimeSpan timeout, CancellationToken cancellationToken)
        {
            return await Task.Run(() => WaitingProcess(unit => unit.WaitAsync(timeout, cancellationToken)),
                CancellationToken.None).ConfigureAwait(false);
        }
    }
}