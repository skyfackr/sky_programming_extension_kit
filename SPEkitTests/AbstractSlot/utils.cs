using Microsoft.VisualStudio.TestTools.UnitTesting;

// ReSharper disable InconsistentNaming

namespace SPEkit.SemaphoreSlimAttribute.Tests
{
    public partial class AbstractSlotTests
    {
        [SlotWait]
        private static void ASNotInitializedFunc()
        {
            throw new AssertFailedException();
        }
    }
}