using System;
using SPEkit.CombinedSemaphore.Unit;
using SPEkit.CombinedSemaphore.Utils;

namespace SPEkit.CombinedSemaphore.MainClass
{
    public sealed partial class CombinedSemaphore
    {
        private bool WaitingProcess(Action<SemaphoreUnit> act)
        {
            var option = m_option.IgnoreConflictFlag();
            //TODO 完成统一的等待步骤
        }
    }
}