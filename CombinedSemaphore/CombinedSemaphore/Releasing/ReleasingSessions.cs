using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SPEkit.CombinedSemaphore.Unit;

namespace SPEkit.CombinedSemaphore.Utils
{
    internal class ReleasingSessions
    {
        internal readonly SemaphoreUnit m_unit;

        internal ReleasingSessions(SemaphoreUnit unit)
        {
            m_unit = unit;
        }

        internal bool IsReleased { get; private set; }

        internal void Released(bool isReleased)
        {
            IsReleased = isReleased;
        }

        internal void Recovery(CancellationToken token)
        {
            if (!IsReleased) return;
            try
            {
                m_unit.Wait(token);
            }
            catch (OperationCanceledException)
            {
            }
            catch (ObjectDisposedException)
            {
            }
        }
    }

    public class ReleaseRecoverySession
    {
        private readonly CancellationTokenSource m_tks;
        public readonly SemaphoreUnit[] Units;

        internal ReleaseRecoverySession(IEnumerable<SemaphoreUnit> units, CancellationTokenSource tks)
        {
            Units = units.ToArray();
            m_tks = tks;
        }

        public bool IsRecoveryCompleted { get; internal set; } = false;
        public bool IsRecoveryCancelled { get; private set; }

        public void Cancel()
        {
            m_tks.Cancel();
            IsRecoveryCancelled = true;
        }
    }
}