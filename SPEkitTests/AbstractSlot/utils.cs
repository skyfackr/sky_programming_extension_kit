using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// ReSharper disable InconsistentNaming

namespace SPEkit.SemaphoreSlimAttribute.Tests
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public partial class AbstractSlotTests
    {
        [SlotWait]
        public static void ASNotInitializedFunc()
        {
            throw new AssertFailedException();
        }

        public static void ASNotRegister()
        {

        }

        [SlotWait(0)]
        public static void ASMakeTimeout()
        {
            throw new AssertFailedException();
        }
    }
}