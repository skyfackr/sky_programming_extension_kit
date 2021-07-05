using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FluentAssert;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nito.AsyncEx.Synchronous;
using SPEkit.BinLikeClassSelectors;

namespace SPEkit.SemaphoreSlimAttribute.Tests
{
    public partial class AbstractSlotTests
    {
        [TestMethod]
        [Timeout(300)]
        public void GetSelectorTests()
        {
            var se = GetType().GetMethod(nameof(ASMakeTimeout)).GetAbstractSlotAttribute();
            se.Option.SetTimeOut(1);
            try
            {
                ASMakeTimeout();
            }
            catch (WaitCancelledOrFailedException e)
            {
                e.Reasons.ShouldBeEqualTo(CancelFlag.Timeout);
                e.IsExecuted.ShouldBeFalse();
                var selector = e.Reasons.GetSelector();
                BinLikeClassSelector.CreateBinLikeClassSelectorUnit(Convert.ToInt64(e.Reasons))
                    .ShouldBeEqualTo(selector);
                selector.GetValidBinArray().ShouldBeEqualTo(new[]
                {
                    Convert.ToInt64(e.Reasons)
                });
            }
        }

        [TestMethod]
        [Timeout(1500)]
        [SuppressMessage("ReSharper", "IdentifierTypo")]
        public void ExecuteWithDisposedTest()
        {
            var se = GetType().GetMethod(nameof(ASDisposed)).GetAbstractSlotAttribute();
            se.Option.SetTimeOut(100);
            var task = Task.Run(() =>
            {
                try
                {
                    ASDisposed();
                }
                catch (WaitCancelledOrFailedException e)
                {
                    e.IsExecuted.ShouldBeFalse();
                    e.Reasons.ShouldBeEqualTo(CancelFlag.Timeout);
                }
            });
            task.WaitAndUnwrapException();
            se.IsDisposed().ShouldBeFalse();
            se.Dispose();
            se.IsDisposed().ShouldBeTrue();
            Assert.ThrowsException<AssertFailedException>(ASDisposed);

            var sea = GetType().GetMethod(nameof(ASDisposedAsync)).GetAbstractSlotAttribute();
            sea.Option.SetTimeOut(100);
            var taska = Task.Run(() =>
            {
                try
                {
                    ASDisposedAsync();
                }
                catch (WaitCancelledOrFailedException e)
                {
                    e.IsExecuted.ShouldBeFalse();
                    e.Reasons.ShouldBeEqualTo(CancelFlag.Timeout);
                }
            });
            taska.WaitAndUnwrapException();
            sea.IsDisposed().ShouldBeFalse();
            sea.Dispose();
            sea.IsDisposed().ShouldBeTrue();
            Assert.ThrowsExceptionAsync<AssertFailedException>(ASDisposedAsync);
        }

        [TestMethod]
        [Timeout(150)]
        public void MethodEqualsTest()
        {
            var me = GetType().GetMethod(nameof(ASMakeTimeout));
            var se = me.GetAbstractSlotAttribute();
            se.GetAssignedMethod().ShouldBeEqualTo(me);
        }
    }
}