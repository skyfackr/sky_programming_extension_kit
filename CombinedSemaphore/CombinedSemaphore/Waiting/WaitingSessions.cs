using System;
using SPEkit.CombinedSemaphore.Unit;

namespace SPEkit.CombinedSemaphore.Utils
{
    internal class WaitingSessions
    {
        internal readonly SemaphoreUnit Unit;
        internal bool IsEntered;

        internal WaitingSessions(SemaphoreUnit unit)
        {
            //IsEntered = isEntered;
            Unit = unit;
        }

        internal void Recovery()
        {
            if (!IsEntered) return;
            try
            {
                Unit.Release();
            }
            catch (ObjectDisposedException)
            {
            }
        }

        internal void Entered(bool isEntered)
        {
            IsEntered = isEntered;
        }
    }
}