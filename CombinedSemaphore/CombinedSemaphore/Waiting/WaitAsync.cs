using System;
using System.Threading;
using System.Threading.Tasks;

namespace SPEkit.CombinedSemaphore.MainClass
{
    public sealed partial class CombinedSemaphore
    {
        //private SemaphoreUnit a;
        public async Task WaitAsync()
        {
            await Task.Run(() => WaitingProcess(async unit =>
            {
                await unit.WaitAsync();
                return true;
            }));
        }

        public async Task<bool> WaitAsync(int millisecondsTimeout)
        {
            return await Task.Run(() => WaitingProcess(unit => WaitAsync(millisecondsTimeout)));
        }

        public async Task<bool> WaitAsync(int millisecondsTimeout, CancellationToken cancellationToken)
        {
            return await Task.Run(() => WaitingProcess(unit => WaitAsync(millisecondsTimeout, cancellationToken)),
                CancellationToken.None);
        }

        public async Task WaitAsync(CancellationToken cancellationToken)
        {
            await Task.Run(() => WaitingProcess(async unit =>
            {
                await unit.WaitAsync(cancellationToken);
                return true;
            }), CancellationToken.None);
        }

        public async Task<bool> WaitAsync(TimeSpan timeout)
        {
            return await Task.Run(() => WaitingProcess(unit => WaitAsync(timeout)), CancellationToken.None);
        }

        public async Task<bool> WaitAsync(TimeSpan timeout, CancellationToken cancellationToken)
        {
            return await Task.Run(() => WaitingProcess(unit => WaitAsync(timeout, cancellationToken)),
                CancellationToken.None);
        }
    }
}