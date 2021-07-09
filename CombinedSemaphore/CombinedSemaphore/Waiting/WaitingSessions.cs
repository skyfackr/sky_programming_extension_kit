using System;
using SPEkit.CombinedSemaphore.Unit;

namespace SPEkit.CombinedSemaphore.Utils
{
    internal class WaitingSessions
    {
        internal readonly bool IsEntered;
        internal readonly SemaphoreUnit Unit;

        internal WaitingSessions(SemaphoreUnit unit, bool isEntered)
        {
            IsEntered = isEntered;
            Unit = unit;
        }

        internal void Recovery()
        {
            if (!IsEntered) return;
            try
            {
                Unit.Release();
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}