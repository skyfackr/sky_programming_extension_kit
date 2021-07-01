using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using FluentAssert;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nito.AsyncEx.Synchronous;

namespace SPEkit.SemaphoreSlimAttribute.Tests
{
    [TestClass]
    [SuppressMessage("ReSharper", "MethodSupportsCancellation")]
    public partial class SlotWaitAttributeTests
    {
        [TestMethod]
        [Timeout(400)]
        public void SlotWaitAttributeCtorTest()
        {
            new SlotWaitAttribute().IsInitialized().ShouldBeFalse();
            new SlotWaitAttribute(1).CurrentCount.ShouldBeEqualTo(1);
        }

        [TestMethod]
        [Timeout(1210 + 100 + 100 + 1500 + 200 * 3 + 250)]
        public void SyncWaitingLogicTest()
        {
            var a = 0;
            Task.Run(() => { SWLogic(() => { a = 1; }); });
            a.ShouldBeEqualTo(0);
            var se = AbstractSlot.GetAbstractSlotAttribute(GetType().GetMethod(nameof(SWLogic)));
            se.CurrentCount.ShouldBeEqualTo(0);
            se.Release();
            Thread.Sleep(200);
            a.ShouldBeEqualTo(1);
            se.CurrentCount.ShouldBeEqualTo(1);
            se.Wait(100).ShouldBeTrue();
            se.CurrentCount.ShouldBeEqualTo(0);
            se.Option.SetTimeOut(100);
            try
            {
                ExcSWLogic();
                Thread.Sleep(200);
                throw new AssertFailedException();
            }
            catch (WaitCancelledOrFailedException e)
            {
                Trace.WriteLine(e);
                e.Reasons.ShouldBeEqualTo(CancelFlag.Timeout);
            }


            var tks = new CancellationTokenSource();
            se.Option.ClearOption().SetCancellationToken(tks.Token);
            se.Release();
            ExcSWLogic();
            Thread.Sleep(200);
            se.Wait();
            var task = Task.Run(() =>
            {
                //se.Option.WaitingTimePerWait.ShouldBeNull();
                //se.Option.Token.ShouldNotBeNull();
                try
                {
                    ExcSWLogic();
                    Thread.Sleep(200);
                    throw new AssertFailedException();
                }
                catch (WaitCancelledOrFailedException e)
                {
                    Trace.WriteLine(e);
                    e.Reasons.ShouldBeEqualTo(CancelFlag.CancelledByToken);
                }
            });
            tks.Cancel();
            task.WaitAndUnwrapException();
            se.Option.SetTimeOut(TimeSpan.FromMilliseconds(100));
            try
            {
                ExcSWLogic();
                Thread.Sleep(200);
                throw new AssertFailedException();
            }
            catch (WaitCancelledOrFailedException e)
            {
                Trace.WriteLine(e);
                e.Reasons.ShouldBeEqualTo(CancelFlag.CancelledByToken);
            }
        }
    }
}