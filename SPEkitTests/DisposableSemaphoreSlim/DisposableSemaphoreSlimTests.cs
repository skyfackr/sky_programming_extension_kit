using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using FluentAssert;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// ReSharper disable once CheckNamespace
namespace SPEkit.DisposableSemaphoreSlim.Tests
{
    [TestClass]
    public class DisposableSemaphoreSlimTests
    {
        private void AssertDisposedException(DisposableSemaphoreSlim instance)
        {
            instance.GetPrivate<bool>("_isDisposed").ShouldBeTrue();
            Assert.ThrowsException<ObjectDisposedException>(() => instance.Wait(1000));
        }

        #region FactoryTest

        [TestMethod]
        public void DisposeTest()
        {
            var a = new DisposableSemaphoreSlim(2);
            var a1 = a.Wait(1000);
            var a2 = a.Wait(1000);
            Assert.AreEqual(0, a.CurrentCount);
            Assert.IsTrue(a1.IsEntered);
            Assert.IsTrue(a2.IsEntered);
            a.GetPrivate<List<DisposableSemaphoreUnit>>("_unreleasedUnit").Count.ShouldNotBeEqualTo(0);
            a.Dispose();
            Assert.IsTrue(a.GetPrivate<bool>("_isDisposed"));
            a.GetPrivate<List<DisposableSemaphoreUnit>>("_unreleasedUnit").ShouldBeEmpty();
            Assert.IsTrue(a1.IsReleased);
            Assert.IsTrue(a2.IsReleased);
            Assert.ThrowsException<ObjectDisposedException>(() => { a.Wait(1000); });
            a.Dispose();
        }

        [TestMethod]
        public void DangerousGetSemaphoreSlimInstanceTest()
        {
            var a = new DisposableSemaphoreSlim();
#pragma warning disable CS0618 // 类型或成员已过时
            Assert.AreSame(a.DangerousGetSemaphoreSlimInstance(), a.GetPrivate<SemaphoreSlim>("_instance"));
#pragma warning restore CS0618 // 类型或成员已过时
        }

        [TestMethod]
        public void DisposableSemaphoreSlimTest0Param()
        {
            var a = new DisposableSemaphoreSlim();
            a.CurrentCount.ShouldBeEqualTo(1);
            using (var b = a.Wait(1000))
            {
                a.CurrentCount.ShouldBeEqualTo(0);
                b.IsEntered.ShouldBeTrue();
            }

            a.CurrentCount.ShouldBeEqualTo(1);
            
            Assert.AreEqual(a.Release(),a.Release()-1);
        }

        [TestMethod]
        public void DisposableSemaphoreSlimTest1Param()
        {
            var a = new DisposableSemaphoreSlim(5);
            a.CurrentCount.ShouldBeEqualTo(5);
            using (var b = a.Wait(1000))
            {
                a.CurrentCount.ShouldBeEqualTo(4);
                b.IsEntered.ShouldBeTrue();
            }

            a.CurrentCount.ShouldBeEqualTo(5);
            Assert.AreEqual(a.Release(), a.Release()-1);
        }

        [TestMethod]
        public void DisposableSemaphoreSlimTest2Param()
        {
            var a = new DisposableSemaphoreSlim(0, 5);
            a.CurrentCount.ShouldBeEqualTo(0);
            a.Release();
            a.CurrentCount.ShouldBeEqualTo(1);
            using (var b = a.Wait(1000))
            {
                a.CurrentCount.ShouldBeEqualTo(0);
                b.IsEntered.ShouldBeTrue();
                a.Release(4);
                a.CurrentCount.ShouldBeEqualTo(4);
            }

            a.CurrentCount.ShouldBeEqualTo(5);
            Assert.ThrowsException<SemaphoreFullException>(() => a.Release());
        }

        [TestMethod]
        public void ReleaseTest()
        {
            var a = new DisposableSemaphoreSlim(2);
            a.CurrentCount.ShouldBeEqualTo(2);
            a.Wait(1000).IsEntered.ShouldBeEqualTo(true);
            a.CurrentCount.ShouldBeEqualTo(1);
            a.Release().ShouldBeEqualTo(1);
            a.CurrentCount.ShouldBeEqualTo(2);
        }

        [TestMethod]
        public void ReleaseTest1Param()
        {
            var a = new DisposableSemaphoreSlim(3);
            a.CurrentCount.ShouldBeEqualTo(3);
            a.Wait(1000).IsEntered.ShouldBeEqualTo(true);
            a.CurrentCount.ShouldBeEqualTo(2);
            a.Wait(1000).IsEntered.ShouldBeEqualTo(true);
            a.CurrentCount.ShouldBeEqualTo(1);
            a.Release(2).ShouldBeEqualTo(1);
            a.CurrentCount.ShouldBeEqualTo(3);
        }

        #endregion

        #region WaitAsyncTest

        [TestMethod]
        [Timeout(1500)]
        public void WaitAsyncTest() //0param
        {
            var a = new DisposableSemaphoreSlim();
            DisposableSemaphoreUnit b;
            using (b = a.WaitAsync().GetAwaiter().GetResult())
            {
                b.IsReleased.ShouldBeFalse();
                b.IsEntered.ShouldBeTrue();
                a.CurrentCount.ShouldBeEqualTo(0);
            }

            b.IsReleased.ShouldBeTrue();
            a.CurrentCount.ShouldBeEqualTo(1);
            a.Dispose();
            AssertDisposedException(a);
        }

        [TestMethod]
        [Timeout(1500)]
        public void WaitAsyncTest1() //token
        {
            var a = new DisposableSemaphoreSlim();
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            token.IsCancellationRequested.ShouldBeFalse();
            DisposableSemaphoreUnit b;
            using (b = a.WaitAsync(token).GetAwaiter().GetResult())
            {
                b.IsReleased.ShouldBeFalse();
                b.IsEntered.ShouldBeTrue();
                a.CurrentCount.ShouldBeEqualTo(0);
            }

            b.IsReleased.ShouldBeTrue();
            a.CurrentCount.ShouldBeEqualTo(1);

            tokenSource.Cancel();
            token.IsCancellationRequested.ShouldBeTrue();
            Assert.ThrowsException<OperationCanceledException>(() => a.WaitAsync(token).GetAwaiter().GetResult());
            
            a.Dispose();
            AssertDisposedException(a);
        }

        [TestMethod]
        [Timeout(2500)]
        public void WaitAsyncTest2() //ms
        {
            var a = new DisposableSemaphoreSlim();
            DisposableSemaphoreUnit b;
            using (b = a.WaitAsync(1000).GetAwaiter().GetResult())
            {
                b.IsReleased.ShouldBeFalse();
                b.IsEntered.ShouldBeTrue();
                a.CurrentCount.ShouldBeEqualTo(0);
                using var c = a.WaitAsync(1000).GetAwaiter().GetResult();
                c.IsReleased.ShouldBeTrue();
                c.IsEntered.ShouldBeFalse();
            }

            b.IsReleased.ShouldBeTrue();
            a.CurrentCount.ShouldBeEqualTo(1);
            a.Dispose();
            AssertDisposedException(a);
        }

        [TestMethod]
        [Timeout(3000)]
        public void WaitAsyncTest3() //ms,token
        {
            var a = new DisposableSemaphoreSlim();
            DisposableSemaphoreUnit b;
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            token.IsCancellationRequested.ShouldBeFalse();
            using (b = a.WaitAsync(1000, token).GetAwaiter().GetResult())
            {
                b.IsReleased.ShouldBeFalse();
                b.IsEntered.ShouldBeTrue();
                a.CurrentCount.ShouldBeEqualTo(0);
                using var c = a.WaitAsync(1000, token).GetAwaiter().GetResult();
                c.IsReleased.ShouldBeTrue();
                c.IsEntered.ShouldBeFalse();
            }

            b.IsReleased.ShouldBeTrue();
            a.CurrentCount.ShouldBeEqualTo(1);
            tokenSource.Cancel();
            token.IsCancellationRequested.ShouldBeTrue();
            Assert.ThrowsException<OperationCanceledException>(
                () => a.WaitAsync(1000, token).GetAwaiter().GetResult());
            a.Dispose();
            AssertDisposedException(a);
        }

        [TestMethod]
        [Timeout(2500)]
        public void WaitAsyncTest4() //timespan
        {
            var a = new DisposableSemaphoreSlim();
            DisposableSemaphoreUnit b;
            var time = new TimeSpan(0, 0, 1);
            using (b = a.WaitAsync(time).GetAwaiter().GetResult())
            {
                b.IsReleased.ShouldBeFalse();
                b.IsEntered.ShouldBeTrue();
                a.CurrentCount.ShouldBeEqualTo(0);
                using var c = a.WaitAsync(time).GetAwaiter().GetResult();
                c.IsReleased.ShouldBeTrue();
                c.IsEntered.ShouldBeFalse();
            }

            b.IsReleased.ShouldBeTrue();
            a.CurrentCount.ShouldBeEqualTo(1);
            a.Dispose();
            AssertDisposedException(a);
        }

        [TestMethod]
        public void WaitAsyncTest5() //timespan,token
        {
            var a = new DisposableSemaphoreSlim();
            DisposableSemaphoreUnit b;
            var time = new TimeSpan(0, 0, 1);
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            token.IsCancellationRequested.ShouldBeFalse();
            using (b = a.WaitAsync(time, token).GetAwaiter().GetResult())
            {
                b.IsReleased.ShouldBeFalse();
                b.IsEntered.ShouldBeTrue();
                a.CurrentCount.ShouldBeEqualTo(0);
                using var c = a.WaitAsync(time, token).GetAwaiter().GetResult();
                c.IsReleased.ShouldBeTrue();
                c.IsEntered.ShouldBeFalse();
            }

            b.IsReleased.ShouldBeTrue();
            a.CurrentCount.ShouldBeEqualTo(1);
            tokenSource.Cancel();
            token.IsCancellationRequested.ShouldBeTrue();
            Assert.ThrowsException<OperationCanceledException>(
                () => a.WaitAsync(time, token).GetAwaiter().GetResult());
            a.Dispose();
            AssertDisposedException(a);
        }

        #endregion

        #region WaitSyncTest

        [TestMethod]
        [Timeout(1500)]
        public void WaitTest() //0param
        {
            var a = new DisposableSemaphoreSlim();
            DisposableSemaphoreUnit b;
            using (b = a.Wait())
            {
                b.IsReleased.ShouldBeFalse();
                b.IsEntered.ShouldBeTrue();
                a.CurrentCount.ShouldBeEqualTo(0);
            }

            b.IsReleased.ShouldBeTrue();
            a.CurrentCount.ShouldBeEqualTo(1);
            a.Dispose();
            AssertDisposedException(a);
        }

        [TestMethod]
        [Timeout(1500)]
        public void WaitTest1() //token
        {
            var a = new DisposableSemaphoreSlim();
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            token.IsCancellationRequested.ShouldBeFalse();
            DisposableSemaphoreUnit b;
            using (b = a.Wait(token))
            {
                b.IsReleased.ShouldBeFalse();
                b.IsEntered.ShouldBeTrue();
                a.CurrentCount.ShouldBeEqualTo(0);
            }

            b.IsReleased.ShouldBeTrue();
            a.CurrentCount.ShouldBeEqualTo(1);

            tokenSource.Cancel();
            token.IsCancellationRequested.ShouldBeTrue();
            Assert.ThrowsException<OperationCanceledException>(() => a.Wait(token));
            a.Dispose();
            AssertDisposedException(a);
        }

        [TestMethod]
        [Timeout(2500)]
        public void WaitTest2() //ms
        {
            var a = new DisposableSemaphoreSlim();
            DisposableSemaphoreUnit b;
            using (b = a.Wait(1000))
            {
                b.IsReleased.ShouldBeFalse();
                b.IsEntered.ShouldBeTrue();
                a.CurrentCount.ShouldBeEqualTo(0);
                using var c = a.Wait(1000);
                c.IsReleased.ShouldBeTrue();
                c.IsEntered.ShouldBeFalse();
            }

            b.IsReleased.ShouldBeTrue();
            a.CurrentCount.ShouldBeEqualTo(1);
            a.Dispose();
            AssertDisposedException(a);
        }

        [TestMethod]
        [Timeout(3000)]
        public void WaitTest3() //ms,token
        {
            var a = new DisposableSemaphoreSlim();
            DisposableSemaphoreUnit b;
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            token.IsCancellationRequested.ShouldBeFalse();
            using (b = a.Wait(1000, token))
            {
                b.IsReleased.ShouldBeFalse();
                b.IsEntered.ShouldBeTrue();
                a.CurrentCount.ShouldBeEqualTo(0);
                using var c = a.Wait(1000, token);
                c.IsReleased.ShouldBeTrue();
                c.IsEntered.ShouldBeFalse();
            }

            b.IsReleased.ShouldBeTrue();
            a.CurrentCount.ShouldBeEqualTo(1);
            tokenSource.Cancel();
            token.IsCancellationRequested.ShouldBeTrue();
            Assert.ThrowsException<OperationCanceledException>(
                () => a.Wait(1000, token));
            a.Dispose();
            AssertDisposedException(a);
        }

        [TestMethod]
        [Timeout(2500)]
        public void WaitTest4() //timespan
        {
            var a = new DisposableSemaphoreSlim();
            DisposableSemaphoreUnit b;
            var time = new TimeSpan(0, 0, 1);
            using (b = a.Wait(time))
            {
                b.IsReleased.ShouldBeFalse();
                b.IsEntered.ShouldBeTrue();
                a.CurrentCount.ShouldBeEqualTo(0);
                using var c = a.Wait(time);
                c.IsReleased.ShouldBeTrue();
                c.IsEntered.ShouldBeFalse();
            }

            b.IsReleased.ShouldBeTrue();
            a.CurrentCount.ShouldBeEqualTo(1);
            a.Dispose();
            AssertDisposedException(a);
        }

        [TestMethod]
        [Timeout(3000)]
        public void WaitTest5() //timespan,token
        {
            var a = new DisposableSemaphoreSlim();
            DisposableSemaphoreUnit b;
            var time = new TimeSpan(0, 0, 1);
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            token.IsCancellationRequested.ShouldBeFalse();
            using (b = a.Wait(time, token))
            {
                b.IsReleased.ShouldBeFalse();
                b.IsEntered.ShouldBeTrue();
                a.CurrentCount.ShouldBeEqualTo(0);
                using var c = a.Wait(time, token);
                c.IsReleased.ShouldBeTrue();
                c.IsEntered.ShouldBeFalse();
            }

            b.IsReleased.ShouldBeTrue();
            a.CurrentCount.ShouldBeEqualTo(1);
            tokenSource.Cancel();
            token.IsCancellationRequested.ShouldBeTrue();
            Assert.ThrowsException<OperationCanceledException>(
                () => a.Wait(time, token));
            a.Dispose();
            AssertDisposedException(a);
        }

        #endregion
    }
}