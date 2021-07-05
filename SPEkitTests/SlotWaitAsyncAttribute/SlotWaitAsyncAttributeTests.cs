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
    public partial class SlotWaitAsyncAttributeTests
    {
        [TestMethod]
        [Timeout(1000)]
        public void SlotWaitAsyncAttributeCtorTest()
        {
            new SlotWaitAsyncAttribute().IsInitialized().ShouldBeFalse();
            new SlotWaitAsyncAttribute(1).CurrentCount.ShouldBeEqualTo(1);
        }

        [TestMethod]
        [Timeout(4000)]
        public void LogicTest()
        {
            LogicTestAsync().WaitAndUnwrapException();
        }
#pragma warning disable CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
        [SuppressMessage("ReSharper", "MethodSupportsCancellation")]
        private async Task LogicTestAsync()
        {
            //无参数测试
            var a = 0;
#pragma warning disable 4014
            var t1 = Task.Run(async () =>
#pragma warning restore 4014
            {
                await SWALogic(async () => { a++; });
            });
            a.ShouldBeEqualTo(0);
            var se = AbstractSlot.GetAbstractSlotAttribute(GetType().GetMethod(nameof(SWALogic)));
            se.CurrentCount.ShouldBeEqualTo(0);
            se.Release();
            await t1;
            a.ShouldBeEqualTo(1);
            se.CurrentCount.ShouldBeEqualTo(1);
            (await se.WaitAsync(100)).ShouldBeTrue();
            se.CurrentCount.ShouldBeEqualTo(0);
            //单超时
            se.Option.SetTimeOut(150);
            try
            {
                await SWALogic(async () => { });
                throw new AssertFailedException();
                //Trace.WriteLine("err1");
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
            await SWALogic(async () => { a += 2; });
            await Task.Delay(150);
            a.ShouldBeEqualTo(3);
            (await se.WaitAsync(100)).ShouldBeTrue();
            //单token 过程中取消
            var task = Task.Run(async () =>
            {
                try
                {
                    await SWALogic(async () => { });
                    throw new AssertFailedException();
                }
                catch (WaitCancelledOrFailedException e)
                {
                    Trace.WriteLine(e);
                    e.Reasons.ShouldBeEqualTo(CancelFlag.CancelledByToken);
                    e.IsExecuted.ShouldBeFalse();
                }
            });
            await Task.Delay(100);
            task.IsCompleted.ShouldBeFalse();
            tks.Cancel();
            await task;
            //都有 token已取消
            se.Option.SetTimeOut(TimeSpan.FromMilliseconds(100));
            try
            {
                await SWALogic(async () => { });
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
                await SWAReleaseMaxExceed();
            }
            catch (WaitCancelledOrFailedException e)
            {
                Trace.WriteLine(e);
                e.Reasons.ShouldBeEqualTo(CancelFlag.MaxCountExceeded);
                e.IsExecuted.ShouldBeTrue();
            }
        }
#pragma warning restore CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
        [TestMethod]
        [Timeout(1000)]
        public void ErrorExecuteTest()
        {
            var tks = new CancellationTokenSource();
            var se = GetType().GetMethod(nameof(SWAErrorExec)).GetAbstractSlotAttribute();
            se.CurrentCount.ShouldBeEqualTo(1);
            var task = SWAErrorExec(tks.Token);
            while (task.Status is TaskStatus.WaitingToRun or TaskStatus.Created) Thread.Sleep(100);
            se.CurrentCount.ShouldBeEqualTo(0);
            tks.Cancel();
            Thread.Sleep(100);
            se.CurrentCount.ShouldBeEqualTo(1);
            Assert.ThrowsExceptionAsync<NotSupportedException>(async () =>
            {
                await task;
            });
        }
    }
}