using System;
using FluentAssert;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SPEkit.SemaphoreSlimAttribute.Tests
{
    [TestClass]
    public class SlotWaitAsyncAttributeTests
    {
        [TestMethod]
        [Timeout(1000)]
        public void SlotWaitAsyncAttributeCtorTest()
        {
            new SlotWaitAsyncAttribute().IsInitialized().ShouldBeFalse();
            new SlotWaitAsyncAttribute(1).CurrentCount.ShouldBeEqualTo(1);
        }

        [TestMethod]
        [Timeout(3000)]
        public void LogicTest()
        {
            throw new NotImplementedException();
        }
    }
}