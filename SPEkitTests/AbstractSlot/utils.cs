using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
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
            throw new AssertFailedException();
        }

        [SlotWait(0)]
        public static void ASMakeTimeout()
        {
            throw new AssertFailedException();
        }

        [SlotWait(0)]
        public static void ASDisposed()
        {
            throw new AssertFailedException();
        }

        [SlotWaitAsync(0)]
        public static Task ASDisposedAsync()
        {
            throw new AssertFailedException();
        }
    }
}