using System;
using System.Threading;
using FluentAssert;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// ReSharper disable InconsistentNaming

namespace SPEkit.SemaphoreSlimAttribute.Tests
{
    [TestClass]
    public class WaitingOptionTests
    {
        [TestMethod]
        [Timeout(1500)]
        public void CreatorTest()
        {
            var o1 = WaitingOption.FromMS(100);
            var o2 = WaitingOption.FromTimeSpan(TimeSpan.FromMilliseconds(100));
            var tks = new CancellationTokenSource();
            var o3 = WaitingOption.FromCancellationToken(tks.Token);
            var o4 = WaitingOption.Create(tks.Token, TimeSpan.FromMilliseconds(100));
            o1.WaitingTimePerWait.ShouldBeEqualTo(o2.WaitingTimePerWait);
            o1.WaitingTimePerWait.ShouldBeEqualTo(o4.WaitingTimePerWait);
            o1.Token.ShouldBeNull();
            o2.Token.ShouldBeNull();
            o1.WaitingTimePerWait.ShouldNotBeNull();
            o3.Token.ShouldNotBeNull();
            o3.WaitingTimePerWait.ShouldBeNull();
            o3.Token.ShouldBeEqualTo(o4.Token);
        }
    }
}