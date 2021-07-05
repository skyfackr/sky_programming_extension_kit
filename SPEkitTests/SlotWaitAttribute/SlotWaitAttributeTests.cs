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
            AbstractSlot.GetAbstractSlotAttribute(GetType().GetMethod(nameof(SWMakeReleaseMaxExceed))).CurrentCount
                .ShouldBeEqualTo(1);
        }

        [TestMethod]
        [Timeout(1210 + 100 + 100 + 1500 + 200 * 3 + 250 + 100)]
        public void SyncWaitingLogicTest()
        {
            //无参数
            var a = 0;
            Task.Run(() => { SWLogic(() => { a++; }); });
            a.ShouldBeEqualTo(0);
            var se = AbstractSlot.GetAbstractSlotAttribute(GetType().GetMethod(nameof(SWLogic)));
            se.CurrentCount.ShouldBeEqualTo(0);
            se.Release();
            Thread.Sleep(200);
            a.ShouldBeEqualTo(1);
            se.CurrentCount.ShouldBeEqualTo(1);
            se.Wait(100).ShouldBeTrue();
            se.CurrentCount.ShouldBeEqualTo(0);
            //单超时
            se.Option.SetTimeOut(100);
            try
            {
                ExcSWLogic();
                //Thread.Sleep(200);
                throw new AssertFailedException();
            }
            catch (WaitCancelledOrFailedException e)
            {
                Trace.WriteLine(e);
                e.Reasons.ShouldBeEqualTo(CancelFlag.Timeout);
                e.IsExecuted.ShouldBeFalse();
            }

            //单token 未取消
            var tks = new CancellationTokenSource();
            se.Option.ClearOption().SetCancellationToken(tks.Token);
            se.Release();
            SWLogic(() => { a += 2; });
            Thread.Sleep(200);
            a.ShouldBeEqualTo(3);
            se.Wait();
            //单token 过程中取消
            var task = Task.Run(() =>
            {
                //se.Option.WaitingTimePerWait.ShouldBeNull();
                //se.Option.Token.ShouldNotBeNull();
                try
                {
                    ExcSWLogic();
                    //Thread.Sleep(200);
                    throw new AssertFailedException();
                }
                catch (WaitCancelledOrFailedException e)
                {
                    Trace.WriteLine(e);
                    e.Reasons.ShouldBeEqualTo(CancelFlag.CancelledByToken);
                    e.IsExecuted.ShouldBeFalse();
                }
            });
            Thread.Sleep(100);
            task.IsCompleted.ShouldBeFalse();
            tks.Cancel();
            task.WaitAndUnwrapException();
            //单token 已取消
            //se.Release();
            //try
            //{
            //    ExcSWLogic();
            //    throw new AssertFailedException();
            //}
            //catch (WaitCancelledOrFailedException e)
            //{
            //    Trace.WriteLine(e);
            //    e.Reasons.ShouldBeEqualTo(CancelFlag.CancelledByToken);
            //}

            //都有 token已取消
            se.Option.SetTimeOut(TimeSpan.FromMilliseconds(100));
            try
            {
                ExcSWLogic();
                //Thread.Sleep(200);
                throw new AssertFailedException();
            }
            catch (WaitCancelledOrFailedException e)
            {
                Trace.WriteLine(e);
                e.Reasons.ShouldBeEqualTo(CancelFlag.CancelledByToken);
                e.IsExecuted.ShouldBeFalse();
            }

            //退出释放超出限制测试
            try
            {
                SWMakeReleaseMaxExceed();
            }
            catch (WaitCancelledOrFailedException e)
            {
                Trace.WriteLine(e);
                e.Reasons.ShouldBeEqualTo(CancelFlag.MaxCountExceeded);
                e.IsExecuted.ShouldBeTrue();
            }
        }

        [TestMethod]
        [Timeout(600)]
        public void ErrorExecuteTest()
        {
            var tks = new CancellationTokenSource();
            var se = GetType().GetMethod(nameof(SWErrorExec)).GetAbstractSlotAttribute();
            se.CurrentCount.ShouldBeEqualTo(1);
            var task = Task.Run(() =>
            {
                SWErrorExec(tks.Token);
                //Trace.WriteLine(1);
            });
            while (task.Status is TaskStatus.WaitingToRun or TaskStatus.Created) Thread.Sleep(100);
            se.CurrentCount.ShouldBeEqualTo(0);
            tks.Cancel();
            Thread.Sleep(100);
            se.CurrentCount.ShouldBeEqualTo(1);
            Assert.ThrowsExceptionAsync<NotSupportedException>(async () => { await task; });
        }
    }
}