using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SPEkit.CombinedSemaphore.Unit;

namespace SPEkit.CombinedSemaphore.Utils
{
    internal class WaitingSessions
    {
        internal readonly bool IsEntered;
        internal readonly SemaphoreUnit Unit;

        internal WaitingSessions(SemaphoreUnit unit,bool isEntered = false)
        {
            IsEntered = isEntered;
            Unit = unit;
        }
    }
}
