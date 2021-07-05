using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace SPEkit.SemaphoreSlimAttribute.Tests
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public partial class SlotWaitAsyncAttributeTests
    {
        [SlotWaitAsync(0)]
        public static async Task SWALogic(Func<Task> a)
        {
            await a();
        }

        [SlotWaitAsync(1, 1)]
        public async Task SWAReleaseMaxExceed()
        {
            //var me = MethodBase.GetCurrentMethod();
            await Task.Run(() =>
            {
                GetType().GetMethod(nameof(SWAReleaseMaxExceed)).GetAbstractSlotAttribute().Release();
                //AbstractSlot.GetAbstractSlotAttribute(me).Release();
            });
        }

        [SlotWaitAsync(1)]
        public async Task SWAErrorExec(CancellationToken token)
        {
            await Task.Delay(-1, token);
            throw new NotSupportedException(nameof(SWAErrorExec));


        }
    }
}