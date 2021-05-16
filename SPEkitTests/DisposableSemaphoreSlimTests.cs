using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPEkit.DisposableSemaphoreSlim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssert;

// ReSharper disable once CheckNamespace
namespace SPEkit.DisposableSemaphoreSlim.Tests
{
    [TestClass()]
    public class DisposableSemaphoreSlimTests
    {
        [TestMethod()]
        public void DisposeTest()
        {
            var a = new DisposableSemaphoreSlim(2);
            var a1 = a.Wait(1000);
            var a2 = a.Wait(1000);
            Assert.AreEqual(0,a.CurrentCount);
            Assert.IsTrue(a1.IsEntered);
            Assert.IsTrue(a2.IsEntered);
            a.Dispose();
            Assert.IsTrue(a.GetPrivate<bool>("_isDisposed"));
            a.GetPrivate<List<DisposableSemaphoreUnit>>("_unreleasedUnit").ShouldBeEmpty();
            Assert.IsTrue(a1.IsReleased);
            Assert.IsTrue(a2.IsReleased);
            Assert.ThrowsException<ObjectDisposedException>(() =>
            {
                a.Wait(1000);
            });
        }

        [TestMethod()]
        public void DangerousGetSemaphoreSlimInstanceTest()
        {
            var a = new DisposableSemaphoreSlim();
#pragma warning disable CS0618 // 类型或成员已过时
            Assert.AreSame(a.DangerousGetSemaphoreSlimInstance(),a.GetPrivate<SemaphoreSlim>("_instance"));
#pragma warning restore CS0618 // 类型或成员已过时
        }

        [TestMethod()]
        public void DisposableSemaphoreSlimTest0Param()
        {
            var a = new DisposableSemaphoreSlim();
            a.CurrentCount.ShouldBeEqualTo(1);
            using (var b=a.Wait(1000))
            {
                a.CurrentCount.ShouldBeEqualTo(0);
                b.IsEntered.ShouldBeTrue();
            }

            a.CurrentCount.ShouldBeEqualTo(1);
            Assert.ThrowsException<SemaphoreFullException>((() => a.Release()));
        }

        [TestMethod()]
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
            Assert.ThrowsException<SemaphoreFullException>((() => a.Release()));
        }

        [TestMethod()]
        public void DisposableSemaphoreSlimTest2Param()
        {
            var a = new DisposableSemaphoreSlim(0,5);
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
            Assert.ThrowsException<SemaphoreFullException>((() => a.Release()));
        }

        [TestMethod()]
        public void ReleaseTest()
        {

        }

        [TestMethod()]
        public void ReleaseTest1()
        {

        }
    }
}