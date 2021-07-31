﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FluentAssert;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPEkit.CombinedSemaphore.error;
using SPEkit.CombinedSemaphore.Unit;
using SPEkit.CombinedSemaphore.Utils;

namespace SPEkit.CombinedSemaphore.MainClass.Tests
{
    [TestClass]
    public class CombinedSemaphoreTests
    {
        [TestInitialize]
        public void TestInitCleanCache()
        {
            CombinedSemaphore.StopCleanInterval();
            typeof(CombinedSemaphore).GetPrivate<Dictionary<Semaphore, SemaphoreWin32Unit>>("s_win32Cache").Clear();
            typeof(CombinedSemaphore).GetPrivate<Dictionary<SemaphoreSlim, SemaphoreSlimUnit>>("s_slimCache").Clear();
        }

        [TestMethod]
        [Timeout(900)]
        public void ReleaseTest()
        {
            var a1 = new Semaphore(1, 2).ToSemaphoreUnit();
            var a2 = new SemaphoreSlim(2, 3).ToSemaphoreUnit();
            var a = new CombinedSemaphore(new[] {a1, a2});
            a2.GetCurrentSemaphoreAsSlim().CurrentCount.ShouldBeEqualTo(2);
            Assert.ThrowsException<TypeCannotConvertException>(a1.GetCurrentSemaphoreAsSlim);
            a.Release().ShouldBeEqualTo(new[] {2, 3});
            var rec = new Dictionary<ReleaseRecoverySession, Exception>();
            a.AllRecoveryCompleteEvent += (r, e) => { rec.Add(r, e); };
            try
            {
                a.Release();
                throw new AssertFailedException();
            }
            catch (ReleaseFailedException e)
            {
                e.RecoverySession.IsRecoveryCancelled.ShouldBeFalse();
                e.RecoverySession.WaitAsync().Wait();
                e.RecoverySession.IsRecoveryCompleted.ShouldBeTrue();
                Thread.Sleep(100);
                rec.Keys.Contains(e.RecoverySession).ShouldBeTrue();
                rec[e.RecoverySession].ShouldBeNull();
                rec.Remove(e.RecoverySession);
            }

            a2.GetCurrentSemaphoreAsSlim().CurrentCount.ShouldBeEqualTo(3);
            var a3 = new SemaphoreSlim(2);
            a.Add(a3.ToSemaphoreUnit());
            a.Option = WaitActionFlag.ContinueAndIgnoreWhenReleaseExceeded;
            a.Release();
            Thread.Sleep(100);
            a3.CurrentCount.ShouldBeEqualTo(2);
        }

        [TestMethod]
        [Timeout(150)]
        public void ReleaseTestInt()
        {
            var a1 = new SemaphoreSlim(1);
            var a2 = new SemaphoreSlim(2);
            var a = a1.Combine(a2);
            a.Release(2).ShouldBeEqualTo(new[] {3, 4});
            a1.CurrentCount.ShouldBeEqualTo(3);
            a2.CurrentCount.ShouldBeEqualTo(4);
        }

        [TestMethod]
        [Timeout(250)]
        public void CleanCreateUnitCacheTest()
        {
            var a1 = new SemaphoreSlim(1);
            var a2 = new SemaphoreSlim(1);
            var b1 = a1.ToSemaphoreUnit();
            var b2 = a2.ToSemaphoreUnit();
            a1.ToSemaphoreUnit().ShouldBeSameInstanceAs(b1);
            a2.ToSemaphoreUnit().ShouldBeSameInstanceAs(b2);
            a1.Dispose();
            CombinedSemaphore.CleanCreateUnitCache().ShouldBeEqualTo(1);
            a2.ToSemaphoreUnit().ShouldBeSameInstanceAs(b2);
            a1.ToSemaphoreUnit().ShouldNotBeSameInstanceAs(b1);
        }

        [TestMethod]
        [Timeout(500)]
        public void SetCleanIntervalTest()
        {
            var ans = new List<int>();
            CombinedSemaphore.CompleteCleanOnceInInterval += i => { ans.Add(i); };
            var a = new SemaphoreSlim(1);
            _ = a.ToSemaphoreUnit();
            a.Dispose();
            CombinedSemaphore.SetCleanInterval(TimeSpan.FromMilliseconds(20));
            Thread.Sleep(200);
            ans.First().ShouldBeEqualTo(1);
            ans.Remove(1);
            (from i in ans
                where i != 0
                select i).Any().ShouldBeFalse();
        }

        [TestMethod]
        [Timeout(100)]
        public void StopCleanIntervalTest()
        {
            CombinedSemaphore.IsCleanIntervalSet.ShouldBeFalse();
            CombinedSemaphore.SetCleanInterval(TimeSpan.FromDays(1));
            CombinedSemaphore.IsCleanIntervalSet.ShouldBeTrue();
            CombinedSemaphore.StopCleanInterval();
            CombinedSemaphore.IsCleanIntervalSet.ShouldBeFalse();
        }

        [TestMethod]
        public void CreateUnitTestWin32()
        {
            var a = new Semaphore(1, 2);
            var b = CombinedSemaphore.CreateUnit(a);
            b.ShouldBeOfType<SemaphoreWin32Unit>();
            CombinedSemaphore.CreateUnit(a).ShouldBeSameInstanceAs(b);
        }

        [TestMethod]
        public void CreateUnitTestSlim()
        {
            var a = new SemaphoreSlim(1, 2);
            var b = CombinedSemaphore.CreateUnit(a);
            b.ShouldBeOfType<SemaphoreSlimUnit>();
            CombinedSemaphore.CreateUnit(a).ShouldBeSameInstanceAs(b);
        }

        [TestMethod]
        public void CreateUnitTestObj()
        {
            var a = new SemaphoreSlim(1, 2) as object;
            var b = new Semaphore(1, 2) as object;
            var c = new SemaphoreSlim(1, 2).ToSemaphoreUnit() as object;
            var d = (object) 1;
            CombinedSemaphore.CreateUnit(a).ShouldBeOfType<SemaphoreSlimUnit>();
            CombinedSemaphore.CreateUnit(b).ShouldBeOfType<SemaphoreWin32Unit>();
            CombinedSemaphore.CreateUnit(c).ShouldBeSameInstanceAs(c);
            Assert.ThrowsException<TypeNotSupportedException>(() => CombinedSemaphore.CreateUnit(d));
        }

        [TestMethod]
        public void CreateUnitsTestWin32()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void CreateUnitsTestSlim()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void CreateUnitsTestObj()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void CreateUnitsAsyncTestWin32()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void CreateUnitsAsyncTestSlim()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void CreateUnitsAsyncTestObj()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void CreateUnitsAsyncTestWin32Async()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void CreateUnitsAsyncTestSlimAsync()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void CreateUnitsAsyncTestObjAsync()
        {
            throw new NotImplementedException();
        }
    }
}