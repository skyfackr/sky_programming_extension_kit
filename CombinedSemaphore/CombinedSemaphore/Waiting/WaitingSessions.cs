using System;
using SPEkit.CombinedSemaphore.Unit;

namespace SPEkit.CombinedSemaphore.Utils
{
    internal class WaitingSessions
    {
        internal readonly SemaphoreUnit Unit;
        internal bool IsEntered;
        private bool m_isRecoveryFinished;

        internal WaitingSessions(SemaphoreUnit unit)
        {
            //IsEntered = isEntered;
            Unit = unit;
        }

        internal void Recovery()
        {
            if (!IsEntered) return;
            if (m_isRecoveryFinished) return;
            try
            {
                Unit.Release();
            }
            catch (ObjectDisposedException)
            {
            }

            m_isRecoveryFinished = true;
        }

        internal void Entered(bool isEntered)
        {
            IsEntered = isEntered;
        }
    }
}