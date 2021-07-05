using System;

// ReSharper disable InconsistentNaming

namespace SPEkit.SemaphoreSlimAttribute.Tests
{
    public partial class SlotWaitAttributeTests
    {
        [SlotWait(0)]
        public static void SWLogic(Action a)
        {
            a();
        }

        private static void ExcSWLogic()
        {
            SWLogic(() => { });
        }

        [SlotWait(1, 1)]
        public void SWMakeReleaseMaxExceed()
        {
            GetType().GetMethod(nameof(SWMakeReleaseMaxExceed)).GetAbstractSlotAttribute().Release();
        }
    }
}