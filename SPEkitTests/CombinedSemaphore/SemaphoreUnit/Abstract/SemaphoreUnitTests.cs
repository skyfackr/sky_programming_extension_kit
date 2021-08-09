using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using FluentAssert;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nito.AsyncEx.Synchronous;
using SPEkit.CombinedSemaphore.error;
using SPEkit.CombinedSemaphore.MainClass;

namespace SPEkit.CombinedSemaphore.Unit.Tests
{
    [TestClass]
    [SuppressMessage("ReSharper", "UnusedVariable")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "MethodSupportsCancellation")]
    [SuppressMessage("CodeQuality", "IDE0079:请删除不必要的忽略", Justification = "<挂起>")]
    public class SemaphoreUnitTests
    {
        private static SemaphoreUnit CreateWin32(int init, int max)
        {
            return new Semaphore(init, max).ToSemaphoreUnit();
        }

        private static SemaphoreUnit CreateSlim(int init, int max)
        {
            return new SemaphoreSlim(init, max).ToSemaphoreUnit();
        }

        private static (SemaphoreUnit, SemaphoreUnit) CreateAll(int init, int max)
        {
            return (CreateWin32(init, max), CreateSlim(init, max));
        }

        [TestMethod]
        [Timeout(200)]
        public void GetCurrentSemaphoreTest()
        {
            var s1 = new Semaphore(1, 2);
            var s1u = s1.ToSemaphoreUnit();
            var s2 = new SemaphoreSlim(1, 2);
            var s2u = s2.ToSemaphoreUnit();
            s1u.GetCurrentSemaphore().ShouldBeSameInstanceAs(s1).ShouldBeOfType<Semaphore>();
            s2u.GetCurrentSemaphore().ShouldBeSameInstanceAs(s2).ShouldBeOfType<SemaphoreSlim>();
        }

        [TestMethod]
        [Timeout(200)]
        public void GetCurrentSemaphoreAsWin32Test()
        {
            var s1 = new Semaphore(1, 2);
            var s1u = s1.ToSemaphoreUnit();

            s1u.GetCurrentSemaphoreAsWin32().ShouldBeSameInstanceAs(s1).ShouldBeOfType<Semaphore>();
            Assert.ThrowsException<TypeCannotConvertException>(s1u.GetCurrentSemaphoreAsSlim);
        }

        [TestMethod]
        [Timeout(200)]
        public void GetCurrentSemaphoreAsSlimTest()
        {
            var s1 = new SemaphoreSlim(1, 2);
            var s1u = s1.ToSemaphoreUnit();

            s1u.GetCurrentSemaphoreAsSlim().ShouldBeSameInstanceAs(s1).ShouldBeOfType<SemaphoreSlim>();
            Assert.ThrowsException<TypeCannotConvertException>(s1u.GetCurrentSemaphoreAsWin32);
        }

        [TestMethod]
        [Timeout(300)]
        public void ReleaseTestNoParam()
        {
            var (w, s) = CreateAll(1, 2);
            w.Release().ShouldBeEqualTo(2);
            s.Release().ShouldBeEqualTo(2);
            Assert.ThrowsException<SemaphoreFullException>(() => w.Release());
            Assert.ThrowsException<SemaphoreFullException>(() => s.Release());
        }

        [TestMethod]
        [Timeout(300)]
        public void ReleaseTestInt()
        {
            var (w, s) = CreateAll(1, 3);
            w.Release(2).ShouldBeEqualTo(3);
            s.Release(2).ShouldBeEqualTo(3);
            Assert.ThrowsException<SemaphoreFullException>(() => w.Release(2));
            Assert.ThrowsException<SemaphoreFullException>(() => s.Release(2));
        }

        [TestMethod]
        [Timeout(800)]
        public void WaitTestNoParam()
        {
            var (w, s) = CreateAll(1, 2);
            w.Wait();
            s.Wait();
        }

        [TestMethod]
        [Timeout(800)]
        public void WaitTestInt()
        {
            var (w, s) = CreateAll(1, 2);
            w.Wait(100).ShouldBeTrue();
            s.Wait(100).ShouldBeTrue();
            w.Wait(100).ShouldBeFalse();
            s.Wait(100).ShouldBeFalse();
        }

        [TestMethod]
        [Timeout(800)]
        public void WaitTestIntTk()
        {
            var (w, s) = CreateAll(1, 2);
            var tks = new CancellationTokenSource();
            w.Wait(100, tks.Token).ShouldBeTrue();
            s.Wait(100, tks.Token).ShouldBeTrue();
            w.Wait(100, tks.Token).ShouldBeFalse();
            s.Wait(100, tks.Token).ShouldBeFalse();
            tks.Cancel();
            Assert.ThrowsException<OperationCanceledException>(() => w.Wait(100, tks.Token));
            Assert.ThrowsException<OperationCanceledException>(() => s.Wait(100, tks.Token));
        }

        [TestMethod]
        [Timeout(800)]
        public void WaitTestTk()
        {
            var (w, s) = CreateAll(1, 2);
            var tks = new CancellationTokenSource();
            w.Wait(tks.Token);
            s.Wait(tks.Token);
            tks.Cancel();
            Assert.ThrowsException<OperationCanceledException>(() => w.Wait(tks.Token));
            Assert.ThrowsException<OperationCanceledException>(() => s.Wait(tks.Token));
        }

        [TestMethod]
        [Timeout(800)]
        public void WaitTestTs()
        {
            var (w, s) = CreateAll(1, 2);
            var ts100 = TimeSpan.FromMilliseconds(100);
            w.Wait(ts100).ShouldBeTrue();
            s.Wait(ts100).ShouldBeTrue();
            w.Wait(ts100).ShouldBeFalse();
            s.Wait(ts100).ShouldBeFalse();
        }

        [TestMethod]
        [Timeout(800)]
        public void WaitTestTsTk()
        {
            var (w, s) = CreateAll(1, 2);
            var ts100 = TimeSpan.FromMilliseconds(100);
            var tks = new CancellationTokenSource();
            w.Wait(ts100, tks.Token).ShouldBeTrue();
            s.Wait(ts100, tks.Token).ShouldBeTrue();
            w.Wait(ts100, tks.Token).ShouldBeFalse();
            s.Wait(ts100, tks.Token).ShouldBeFalse();
            tks.Cancel();
            Assert.ThrowsException<OperationCanceledException>(() => w.Wait(ts100, tks.Token));
            Assert.ThrowsException<OperationCanceledException>(() => s.Wait(ts100, tks.Token));
        }

        [TestMethod]
        [Timeout(800)]
        public void WaitAsyncTestNoParam()
        {
            var (w, s) = CreateAll(1, 2);
            w.WaitAsync().WaitAndUnwrapException();
            s.WaitAsync().WaitAndUnwrapException();
        }

        [TestMethod]
        [Timeout(800)]
        public void WaitAsyncTestInt()
        {
            var (w, s) = CreateAll(1, 2);
            w.WaitAsync(100).WaitAndUnwrapException().ShouldBeTrue();
            s.WaitAsync(100).WaitAndUnwrapException().ShouldBeTrue();
            w.WaitAsync(100).WaitAndUnwrapException().ShouldBeFalse();
            s.WaitAsync(100).WaitAndUnwrapException().ShouldBeFalse();
        }

        [TestMethod]
        [Timeout(800)]
        public void WaitAsyncTestIntTk()
        {
            var (w, s) = CreateAll(1, 2);
            var tks = new CancellationTokenSource();
            w.WaitAsync(100, tks.Token).WaitAndUnwrapException().ShouldBeTrue();
            s.WaitAsync(100, tks.Token).WaitAndUnwrapException().ShouldBeTrue();
            w.WaitAsync(100, tks.Token).WaitAndUnwrapException().ShouldBeFalse();
            s.WaitAsync(100, tks.Token).WaitAndUnwrapException().ShouldBeFalse();
            tks.Cancel();
            Assert.ThrowsException<OperationCanceledException>(() =>
                w.WaitAsync(100, tks.Token).WaitAndUnwrapException());
            Assert.ThrowsException<OperationCanceledException>(() =>
                s.WaitAsync(100, tks.Token).WaitAndUnwrapException());
        }

        [TestMethod]
        [Timeout(800)]
        public void WaitAsyncTestTk()
        {
            var (w, s) = CreateAll(1, 2);
            var tks = new CancellationTokenSource();
            w.WaitAsync(tks.Token).WaitAndUnwrapException();
            s.WaitAsync(tks.Token).WaitAndUnwrapException();
            tks.Cancel();
            Assert.ThrowsException<OperationCanceledException>(() => w.WaitAsync(tks.Token).WaitAndUnwrapException());
            Assert.ThrowsException<OperationCanceledException>(() => s.WaitAsync(tks.Token).WaitAndUnwrapException());
        }

        [TestMethod]
        [Timeout(800)]
        public void WaitAsyncTestTs()
        {
            var (w, s) = CreateAll(1, 2);
            var ts100 = TimeSpan.FromMilliseconds(100);
            w.WaitAsync(ts100).WaitAndUnwrapException().ShouldBeTrue();
            s.WaitAsync(ts100).WaitAndUnwrapException().ShouldBeTrue();
            w.WaitAsync(ts100).WaitAndUnwrapException().ShouldBeFalse();
            s.WaitAsync(ts100).WaitAndUnwrapException().ShouldBeFalse();
        }

        [TestMethod]
        [Timeout(800)]
        public void WaitAsyncTestTsTk()
        {
            var (w, s) = CreateAll(1, 2);
            var ts100 = TimeSpan.FromMilliseconds(100);
            var tks = new CancellationTokenSource();
            w.WaitAsync(ts100, tks.Token).WaitAndUnwrapException().ShouldBeTrue();
            s.WaitAsync(ts100, tks.Token).WaitAndUnwrapException().ShouldBeTrue();
            w.WaitAsync(ts100, tks.Token).WaitAndUnwrapException().ShouldBeFalse();
            s.WaitAsync(ts100, tks.Token).WaitAndUnwrapException().ShouldBeFalse();
            tks.Cancel();
            Assert.ThrowsException<OperationCanceledException>(() =>
                w.WaitAsync(ts100, tks.Token).WaitAndUnwrapException());
            Assert.ThrowsException<OperationCanceledException>(() =>
                s.WaitAsync(ts100, tks.Token).WaitAndUnwrapException());
        }

        [TestMethod]
        [Timeout(200)]
        public void GetWaitHandleTest()
        {
            var (w, s) = CreateAll(1, 2);
            w.GetWaitHandle().ShouldBeSameInstanceAs(w.GetCurrentSemaphoreAsWin32());
            s.GetWaitHandle().ShouldBeSameInstanceAs(s.GetCurrentSemaphoreAsSlim().AvailableWaitHandle);
            w.Dispose();
            s.Dispose();
            w.GetWaitHandle().ShouldBeSameInstanceAs(w.GetCurrentSemaphoreAsWin32());
            Assert.ThrowsException<ObjectDisposedException>(s.GetWaitHandle);
        }
    }
}