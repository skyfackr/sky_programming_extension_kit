using System.Diagnostics;
using System.Threading;
using FluentAssert;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// ReSharper disable IdentifierTypo

namespace SPEkit.SemaphoreSlimAttribute.Tests
{
    [TestClass]
    public partial class AbstractSlotTests
    {
        [TestMethod]
        [Timeout(2000)]
        public void ErrorsTest()
        {
            Assert.ThrowsException<NotInitializedException>(ASNotInitializedFunc);

            var se = GetType().GetMethod(nameof(ASNotInitializedFunc)).GetAbstractSlotAttribute();
            Trace.WriteLine($"{se}");
            GetType().GetMethod(nameof(ASNotInitializedFunc)).TryGetAbstractSlotAttribute().ShouldBeEqualTo(se);
            se.SetSemaphore(new SemaphoreSlim(1));
            Trace.WriteLine($"{se}");
            Assert.ThrowsException<AssertFailedException>(ASNotInitializedFunc);
            var newse = new SlotWaitAsyncAttribute();
            Assert.ThrowsException<AmbiguousAssignedException>(() =>
            {
                newse.RuntimeInitialize(GetType().GetMethod(nameof(ASNotInitializedFunc)));
            });
            GetType().GetMethod(nameof(ASNotRegister)).TryGetAbstractSlotAttribute().ShouldBeNull();
            Assert.ThrowsException<MethodNotRegisteredException>(() =>
            {
                _ = GetType().GetMethod(nameof(ASNotRegister)).GetAbstractSlotAttribute();
            });
        }
    }
}