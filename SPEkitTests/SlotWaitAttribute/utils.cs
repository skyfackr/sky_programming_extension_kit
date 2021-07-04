using System;
using System.Reflection;

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
        public static void SWMakeReleaseMaxExceed()
        {
            MethodBase.GetCurrentMethod().GetAbstractSlotAttribute().Release();
        }
    }
}