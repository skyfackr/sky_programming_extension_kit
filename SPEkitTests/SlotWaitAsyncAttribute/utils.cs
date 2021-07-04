using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// ReSharper disable InconsistentNaming

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
    }
}
