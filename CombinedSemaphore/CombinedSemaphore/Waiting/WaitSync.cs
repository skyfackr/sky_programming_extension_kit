using System;
using System.Threading;
using System.Threading.Tasks;

namespace SPEkit.CombinedSemaphore.MainClass
{
    public sealed partial class CombinedSemaphore
    {
        //private SemaphoreUnit a;
        public void Wait()
        {
            WaitingProcess(unit =>
            {
                unit.Wait();
                return Task.FromResult(true);
            });
        }

        public bool Wait(int millisecondsTimeout)
        {
            return WaitingProcess(unit => Task.FromResult(unit.Wait(millisecondsTimeout)));
        }

        public bool Wait(int millisecondsTimeout, CancellationToken cancellationToken)
        {
            return WaitingProcess(unit => Task.FromResult(unit.Wait(millisecondsTimeout, cancellationToken)));
        }

        public void Wait(CancellationToken cancellationToken)
        {
            WaitingProcess(unit =>
            {
                unit.Wait(cancellationToken);
                return Task.FromResult(true);
            });
        }

        public bool Wait(TimeSpan timeout)
        {
            return WaitingProcess(unit => Task.FromResult(unit.Wait(timeout)));
        }

        public bool Wait(TimeSpan timeout, CancellationToken cancellationToken)
        {
            return WaitingProcess(unit => Task.FromResult(unit.Wait(timeout, cancellationToken)));
        }
    }
}