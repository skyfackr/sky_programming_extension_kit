using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssert;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nito.AsyncEx.Synchronous;
using SPEkit.CombinedSemaphore.error;
using SPEkit.CombinedSemaphore.Unit;
using SPEkit.CombinedSemaphore.Utils;

namespace SPEkit.CombinedSemaphore.MainClass.Tests
{
    [TestClass]
    [SuppressMessage("Usage", "SecurityIntelliSenseCS:MS Security rules violation", Justification = "<挂起>")]
    [SuppressMessage("Security", "SCS0005:Weak random number generator.", Justification = "<挂起>")]
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
        [Timeout(200)]
        public void CreateUnitTestWin32()
        {
            var a = new Semaphore(1, 2);
            var b = CombinedSemaphore.CreateUnit(a);
            b.ShouldBeOfType<SemaphoreWin32Unit>();
            CombinedSemaphore.CreateUnit(a).ShouldBeSameInstanceAs(b);
        }

        [TestMethod]
        [Timeout(200)]
        public void CreateUnitTestSlim()
        {
            var a = new SemaphoreSlim(1, 2);
            var b = CombinedSemaphore.CreateUnit(a);
            b.ShouldBeOfType<SemaphoreSlimUnit>();
            CombinedSemaphore.CreateUnit(a).ShouldBeSameInstanceAs(b);
        }

        [TestMethod]
        [Timeout(200)]
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
        [SuppressMessage("ReSharper", "IdentifierTypo")]
        [SuppressMessage("Usage", "SecurityIntelliSenseCS:MS Security rules violation", Justification = "<挂起>")]
        [SuppressMessage("Security", "SCS0005:Weak random number generator.", Justification = "<挂起>")]
        [Timeout(350)]
        public void CreateUnitsTestWin32()
        {
            var sa = new Semaphore(1, 2);
            var sb = new Semaphore(1, 2);
            var rad = new Random();
            var sac = rad.Next(1, 100);
            var sbc = rad.Next(1, 100);
            var ses = new List<Semaphore>();
            for (var i = 1; i <= sac; i++) ses.Add(sa);

            for (var i = 1; i <= sbc; i++) ses.Add(sb);

            ses.Count.ShouldBeEqualTo(sac + sbc);
            var lists = CombinedSemaphore.CreateUnits(ses).AsParallel().ToList();
            lists.Count.ShouldBeEqualTo(sac + sbc);
            var dis = lists.Distinct().ToList();
            dis.Count().ShouldBeEqualTo(2);
            dis.Any(unit => unit.GetCurrentSemaphoreAsWin32() != sa && unit.GetCurrentSemaphoreAsWin32() != sb)
                .ShouldBeFalse();
        }

        [TestMethod]
        [SuppressMessage("ReSharper", "IdentifierTypo")]
        [SuppressMessage("Usage", "SecurityIntelliSenseCS:MS Security rules violation", Justification = "<挂起>")]
        [SuppressMessage("Security", "SCS0005:Weak random number generator.", Justification = "<挂起>")]
        [Timeout(350)]
        public void CreateUnitsTestSlim()
        {
            var sa = new SemaphoreSlim(1, 2);
            var sb = new SemaphoreSlim(1, 2);
            var rad = new Random();
            var sac = rad.Next(1, 100);
            var sbc = rad.Next(1, 100);
            var ses = new List<SemaphoreSlim>();
            for (var i = 1; i <= sac; i++) ses.Add(sa);

            for (var i = 1; i <= sbc; i++) ses.Add(sb);

            ses.Count.ShouldBeEqualTo(sac + sbc);
            var lists = CombinedSemaphore.CreateUnits(ses).AsParallel().ToList();
            lists.Count.ShouldBeEqualTo(sac + sbc);
            var dis = lists.Distinct().ToList();
            dis.Count().ShouldBeEqualTo(2);
            dis.Any(unit => unit.GetCurrentSemaphoreAsSlim() != sa && unit.GetCurrentSemaphoreAsSlim() != sb)
                .ShouldBeFalse();
        }

        [TestMethod]
        [Timeout(200)]
        public void CreateUnitsTestObj()
        {
            var a = new Semaphore(1, 2) as object;
            var b = new SemaphoreSlim(1, 2) as object;
            var c = 1 as object;
            var d = new[]
            {
                a, b
            };
            CombinedSemaphore.CreateUnits(d).ShouldContainAllInOrder(new[]
                {CombinedSemaphore.CreateUnit(a), CombinedSemaphore.CreateUnit(b)});
            d = d.Append(c).ToArray();
            Assert.ThrowsException<TypeNotSupportedException>(() => CombinedSemaphore.CreateUnits(d).ToList());
        }

        [TestMethod]
        [SuppressMessage("ReSharper", "IdentifierTypo")]
        [SuppressMessage("Usage", "SecurityIntelliSenseCS:MS Security rules violation", Justification = "<挂起>")]
        [SuppressMessage("Security", "SCS0005:Weak random number generator.", Justification = "<挂起>")]
        [Timeout(450)]
        public void CreateUnitsAsyncTestWin32()
        {
            var sa = new Semaphore(1, 2);
            var sb = new Semaphore(1, 2);
            var rad = new Random();
            var sac = rad.Next(1, 100);
            var sbc = rad.Next(1, 100);
            var ses = new List<Semaphore>();
            for (var i = 1; i <= sac; i++) ses.Add(sa);

            for (var i = 1; i <= sbc; i++) ses.Add(sb);

            ses.Count.ShouldBeEqualTo(sac + sbc);
            var listsMaker = CombinedSemaphore.CreateUnitsAsync(ses);
            var lists = MakeList(listsMaker).WaitAndUnwrapException();
            lists.Count.ShouldBeEqualTo(sac + sbc);
            var dis = lists.Distinct().ToList();
            dis.Count().ShouldBeEqualTo(2);
            dis.Any(unit => unit.GetCurrentSemaphoreAsWin32() != sa && unit.GetCurrentSemaphoreAsWin32() != sb)
                .ShouldBeFalse();
        }

        [SuppressMessage("ReSharper", "AsyncConverter.AsyncMethodNamingHighlighting")]
        private static async Task<List<SemaphoreUnit>> MakeList(IAsyncEnumerable<SemaphoreUnit> m)
        {
            var ans = new List<SemaphoreUnit>();
            await foreach (var unit in m.ConfigureAwait(false)) ans.Add(unit);

            return ans;
        }

        [TestMethod]
        [SuppressMessage("ReSharper", "IdentifierTypo")]
        [SuppressMessage("Usage", "SecurityIntelliSenseCS:MS Security rules violation", Justification = "<挂起>")]
        [SuppressMessage("Security", "SCS0005:Weak random number generator.", Justification = "<挂起>")]
        [Timeout(450)]
        public void CreateUnitsAsyncTestSlim()
        {
            var sa = new SemaphoreSlim(1, 2);
            var sb = new SemaphoreSlim(1, 2);
            var rad = new Random();
            var sac = rad.Next(1, 100);
            var sbc = rad.Next(1, 100);
            var ses = new List<SemaphoreSlim>();
            for (var i = 1; i <= sac; i++) ses.Add(sa);

            for (var i = 1; i <= sbc; i++) ses.Add(sb);

            ses.Count.ShouldBeEqualTo(sac + sbc);
            var listsMaker = CombinedSemaphore.CreateUnitsAsync(ses);
            var lists = MakeList(listsMaker).WaitAndUnwrapException();
            lists.Count.ShouldBeEqualTo(sac + sbc);
            var dis = lists.Distinct().ToList();
            dis.Count().ShouldBeEqualTo(2);
            dis.Any(unit => unit.GetCurrentSemaphoreAsSlim() != sa && unit.GetCurrentSemaphoreAsSlim() != sb)
                .ShouldBeFalse();
        }

        [TestMethod]
        [Timeout(300)]
        public void CreateUnitsAsyncTestObj()
        {
            var a = new Semaphore(1, 2) as object;
            var b = new SemaphoreSlim(1, 2) as object;
            var c = 1 as object;
            var d = new[]
            {
                a, b
            };
            MakeList(CombinedSemaphore.CreateUnitsAsync(d)).WaitAndUnwrapException().ShouldContainAllInOrder(new[]
                {CombinedSemaphore.CreateUnit(a), CombinedSemaphore.CreateUnit(b)});
            d = d.Append(c).ToArray();
            Assert.ThrowsException<TypeNotSupportedException>(() =>
                MakeList(CombinedSemaphore.CreateUnitsAsync(d)).WaitAndUnwrapException());
        }

        [TestMethod]
        [SuppressMessage("ReSharper", "IdentifierTypo")]
        [SuppressMessage("Usage", "SecurityIntelliSenseCS:MS Security rules violation", Justification = "<挂起>")]
        [SuppressMessage("Security", "SCS0005:Weak random number generator.", Justification = "<挂起>")]
        [Timeout(500)]
        public void CreateUnitsAsyncTestWin32Async()
        {
            var sa = new Semaphore(1, 2);
            var sb = new Semaphore(1, 2);
            var rad = new Random();
            var sac = rad.Next(1, 100);
            var sbc = rad.Next(1, 100);
            var ses = new List<Semaphore>();
            for (var i = 1; i <= sac; i++) ses.Add(sa);

            for (var i = 1; i <= sbc; i++) ses.Add(sb);

            ses.Count.ShouldBeEqualTo(sac + sbc);
            var sesa = MakeAsyncEnumerable(ses);
            var listsMaker = CombinedSemaphore.CreateUnitsAsync(sesa);
            var lists = MakeList(listsMaker).WaitAndUnwrapException();
            lists.Count.ShouldBeEqualTo(sac + sbc);
            var dis = lists.Distinct().ToList();
            dis.Count().ShouldBeEqualTo(2);
            dis.Any(unit => unit.GetCurrentSemaphoreAsWin32() != sa && unit.GetCurrentSemaphoreAsWin32() != sb)
                .ShouldBeFalse();
        }

        [TestMethod]
        [SuppressMessage("ReSharper", "IdentifierTypo")]
        [SuppressMessage("Usage", "SecurityIntelliSenseCS:MS Security rules violation", Justification = "<挂起>")]
        [SuppressMessage("Security", "SCS0005:Weak random number generator.", Justification = "<挂起>")]
        [Timeout(500)]
        public void CreateUnitsAsyncTestSlimAsync()
        {
            var sa = new SemaphoreSlim(1, 2);
            var sb = new SemaphoreSlim(1, 2);
            var rad = new Random();
            var sac = rad.Next(1, 100);
            var sbc = rad.Next(1, 100);
            var ses = new List<SemaphoreSlim>();
            for (var i = 1; i <= sac; i++) ses.Add(sa);

            for (var i = 1; i <= sbc; i++) ses.Add(sb);

            ses.Count.ShouldBeEqualTo(sac + sbc);
            var sesa = MakeAsyncEnumerable(ses);
            var listsMaker = CombinedSemaphore.CreateUnitsAsync(sesa);
            var lists = MakeList(listsMaker).WaitAndUnwrapException();
            lists.Count.ShouldBeEqualTo(sac + sbc);
            var dis = lists.Distinct().ToList();
            dis.Count().ShouldBeEqualTo(2);
            dis.Any(unit => unit.GetCurrentSemaphoreAsSlim() != sa && unit.GetCurrentSemaphoreAsSlim() != sb)
                .ShouldBeFalse();
        }

        [TestMethod]
        [Timeout(350)]
        public void CreateUnitsAsyncTestObjAsync()
        {
            var a = new Semaphore(1, 2) as object;
            var b = new SemaphoreSlim(1, 2) as object;
            var c = 1 as object;
            var darr = new[]
            {
                a, b
            };
            var d = MakeAsyncEnumerable(darr);
            MakeList(CombinedSemaphore.CreateUnitsAsync(d)).WaitAndUnwrapException().ShouldContainAllInOrder(new[]
                {CombinedSemaphore.CreateUnit(a), CombinedSemaphore.CreateUnit(b)});
            d = MakeAsyncEnumerable(darr.Append(c));
            Assert.ThrowsException<TypeNotSupportedException>(() =>
                MakeList(CombinedSemaphore.CreateUnitsAsync(d)).WaitAndUnwrapException());
        }

#pragma warning disable CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
        private static async IAsyncEnumerable<T> MakeAsyncEnumerable<T>(IEnumerable<T> m)
#pragma warning restore CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
        {
            foreach (var t in m) yield return t;
        }

        [TestMethod]
        [Timeout(200)]
        public void TryAddTestUnit()
        {
            var a = new SemaphoreSlim(1, 2).ToSemaphoreUnit();
            var b = new SemaphoreSlim(2, 3).ToSemaphoreUnit();
            var c = a.Combine(b);
            var d = new SemaphoreSlim(3, 4).ToSemaphoreUnit();
            c.Contains(d).ShouldBeFalse();
            c.Contains(a).ShouldBeTrue();
            c.TryAdd(b).ShouldBeFalse();
            c.TryAdd(d).ShouldBeTrue();
            c.Contains(d).ShouldBeTrue();
        }

        [TestMethod]
        [SuppressMessage("Security", "SCS0005:Weak random number generator.", Justification = "<挂起>")]
        [SuppressMessage("Usage", "SecurityIntelliSenseCS:MS Security rules violation", Justification = "<挂起>")]
        [Timeout(500)]
        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        public void GetWaitHandlesTest()
        {
            var rnd = new Random();
            var count = rnd.Next(10, 100);
            var origType = new int[count]; //1 win32 2 slim
            var orig = new SemaphoreUnit[count];
            for (var i = 1; i <= count; i++)
            {
                var index = new Index(i - 1);
                origType[index] = rnd.Next(1, 3);
                orig[index] = origType[index] == 1
                    ? new Semaphore(1, 2).ToSemaphoreUnit()
                    : new SemaphoreSlim(1, 2).ToSemaphoreUnit();
            }

            var handles = orig.Combine().GetWaitHandles();
            handles.Count().ShouldBeEqualTo(count);
            var dest = handles.ToArray();
            for (var i = 1; i <= count; i++)
            {
                var index = new Index(i - 1);
                if (origType[index] == 1)
                    dest[index].ShouldBeOfType<Semaphore>();
                else
                    dest[index].ShouldBeOfType<ManualResetEvent>();
            }
        }

        [TestMethod]
        [Timeout(200)]
        public void TryAddTestSlim()
        {
            var a = new SemaphoreSlim(1, 2);
            var b = new SemaphoreSlim(2, 3);
            var c = a.Combine(b);
            var d = new SemaphoreSlim(3, 4);
            c.Contains(d).ShouldBeFalse();
            c.Contains(a).ShouldBeTrue();
            c.TryAdd(b).ShouldBeFalse();
            c.TryAdd(d).ShouldBeTrue();
            c.Contains(d).ShouldBeTrue();
        }

        [TestMethod]
        [Timeout(200)]
        public void TryAddTestWin32()
        {
            var a = new Semaphore(1, 2);
            var b = new Semaphore(2, 3);
            var c = a.Combine(b);
            var d = new Semaphore(3, 4);
            c.Contains(d).ShouldBeFalse();
            c.Contains(a).ShouldBeTrue();
            c.TryAdd(b).ShouldBeFalse();
            c.TryAdd(d).ShouldBeTrue();
            c.Contains(d).ShouldBeTrue();
        }

        [TestMethod]
        [Timeout(200)]
        public void ContainsTestSlim()
        {
            var a = new SemaphoreSlim(1, 2);
            a.Combine().Contains(a).ShouldBeTrue();
            a.Combine().Contains(new SemaphoreSlim(1, 2)).ShouldBeFalse();
        }

        [TestMethod]
        [Timeout(200)]
        public void ContainsTestWin32()
        {
            var a = new Semaphore(1, 2);
            a.Combine().Contains(a).ShouldBeTrue();
            a.Combine().Contains(new Semaphore(1, 2)).ShouldBeFalse();
        }

        [TestMethod]
        [Timeout(150)]
        [SuppressMessage("Usage", "SecurityIntelliSenseCS:MS Security rules violation", Justification = "<挂起>")]
        [SuppressMessage("Security", "SCS0005:Weak random number generator.", Justification = "<挂起>")]
        public void GetUnitListTest()
        {
            var rnd = new Random();
            var count = rnd.Next(10, 20);
            var cse = new CombinedSemaphore(new SemaphoreSlim(1, 2));
            for (var i = 1; i <= count; i++)
                cse.Add(rnd.Next(1, 3) == 1
                    ? new SemaphoreSlim(1, 2).ToSemaphoreUnit()
                    : new Semaphore(1, 2).ToSemaphoreUnit());

            cse.GetUnitList().ShouldContainAllInOrder(cse);
        }

        [TestMethod]
        [Timeout(500)]
        [SuppressMessage("Usage", "SecurityIntelliSenseCS:MS Security rules violation", Justification = "<挂起>")]
        [SuppressMessage("Security", "SCS0005:Weak random number generator.", Justification = "<挂起>")]
        public void GetAllSemaphoreWin32Test()
        {
            var rnd = new Random();
            var count = rnd.Next(10, 50);
            var list = new SemaphoreUnit[count];
            for (var i = 1; i <= count; i++)
                list[i - 1] = rnd.Next(1, 3) == 1
                    ? new Semaphore(1, 2).ToSemaphoreUnit()
                    : new SemaphoreSlim(1, 2).ToSemaphoreUnit();

            foreach (var unit in list)
                if (unit is SemaphoreSlimUnit)
                    Assert.ThrowsException<TypeCannotConvertException>(unit.GetCurrentSemaphoreAsWin32);
                else (unit.GetCurrentSemaphoreAsWin32() != null).ShouldBeTrue();
        }

        [TestMethod]
        [Timeout(500)]
        [SuppressMessage("Usage", "SecurityIntelliSenseCS:MS Security rules violation", Justification = "<挂起>")]
        [SuppressMessage("Security", "SCS0005:Weak random number generator.", Justification = "<挂起>")]
        public void GetAllSemaphoreSlimTest()
        {
            var rnd = new Random();
            var count = rnd.Next(10, 50);
            var list = new SemaphoreUnit[count];
            for (var i = 1; i <= count; i++)
                list[i - 1] = rnd.Next(1, 3) == 1
                    ? new Semaphore(1, 2).ToSemaphoreUnit()
                    : new SemaphoreSlim(1, 2).ToSemaphoreUnit();

            foreach (var unit in list)
                if (unit is SemaphoreWin32Unit)
                    Assert.ThrowsException<TypeCannotConvertException>(unit.GetCurrentSemaphoreAsSlim);
                else (unit.GetCurrentSemaphoreAsSlim() != null).ShouldBeTrue();
        }

        [TestMethod]
        [Timeout(1000)]
        public void RemoveAllDisposedUnitTest()
        {
            var rnd = new Random();
            var orgCount = rnd.Next(10, 100);
            var disCount = 0;
            var list = new SemaphoreUnit[orgCount];
            for (var i = 1; i <= orgCount; i++)
            {
                var index = new Index(i - 1);
                list[index] = new SemaphoreSlim(1, 2).ToSemaphoreUnit();
                if (rnd.Next(1, 3) == 1)
                {
                    disCount++;
                    list[index].Dispose();
                }
            }

            var c = list.Combine();
            c.RemoveAllDisposedUnit();
            c.Count.ShouldBeEqualTo(orgCount - disCount);
        }

        [TestMethod]
        [Timeout(300)]
        public void WaitAsyncTestNoParam()
        {
            var list = CreateRndUnitList(10, 100, 1, 2);

            list.Combine().WaitAsync().WaitAndUnwrapException();
            foreach (var unit in list) unit.GetCurrentSemaphoreAsSlim().CurrentCount.ShouldBeEqualTo(0);
        }

        private static SemaphoreUnit[] CreateRndUnitList(int min, int max, int init, int maxCount)
        {
            var rnd = new Random();
            var count = rnd.Next(min, max);
            var list = new SemaphoreUnit[count];
            for (var i = 1; i <= count; i++) list[i - 1] = new SemaphoreSlim(init, maxCount).ToSemaphoreUnit();
            return list;
        }

        [TestMethod]
        [Timeout(400)]
        public void WaitAsyncTestInt()
        {
            var list = CreateRndUnitList(10, 100, 1, 2);
            var c = list.Combine();
            c.WaitAsync(400).WaitAndUnwrapException().ShouldBeTrue();
            c.WaitAsync(100).WaitAndUnwrapException().ShouldBeFalse();
            var task = c.WaitAsync(1000);
            task.IsCompleted.ShouldBeFalse();
            c.Release();
            task.WaitAndUnwrapException().ShouldBeTrue();
        }

        [TestMethod]
        [Timeout(400)]
        public void WaitAsyncTestIntTk()
        {
            var list = CreateRndUnitList(10, 100, 1, 2);
            var c = list.Combine();
            var tks = new CancellationTokenSource();
            c.WaitAsync(400, tks.Token).WaitAndUnwrapException().ShouldBeTrue();
            c.WaitAsync(100, tks.Token).WaitAndUnwrapException().ShouldBeFalse();
            var task1 = c.WaitAsync(1000, tks.Token);
            task1.IsCompleted.ShouldBeFalse();
            c.Release();
            task1.WaitAndUnwrapException().ShouldBeTrue();
            var task2 = c.WaitAsync(1000, tks.Token);
            task2.IsCompleted.ShouldBeFalse();
            tks.Cancel();
            Assert.ThrowsException<OperationCanceledException>(task2.WaitAndUnwrapException);
        }

        [TestMethod]
        [Timeout(300)]
        public void WaitAsyncTestTk()
        {
            var list = CreateRndUnitList(10, 100, 1, 2);
            var c = list.Combine();
            var tks = new CancellationTokenSource();
            c.WaitAsync(tks.Token).WaitAndUnwrapException(CancellationToken.None);

            var task2 = c.WaitAsync(tks.Token);
            task2.IsCompleted.ShouldBeFalse();
            tks.Cancel();
            Assert.ThrowsException<OperationCanceledException>(task2.WaitAndUnwrapException);
        }

        [TestMethod]
        [Timeout(400)]
        public void WaitAsyncTestTs()
        {
            var list = CreateRndUnitList(10, 100, 1, 2);
            var c = list.Combine();
            c.WaitAsync(TimeSpan.FromMilliseconds(400)).WaitAndUnwrapException().ShouldBeTrue();
            c.WaitAsync(TimeSpan.FromMilliseconds(100)).WaitAndUnwrapException().ShouldBeFalse();
            var task = c.WaitAsync(TimeSpan.FromMilliseconds(1000));
            task.IsCompleted.ShouldBeFalse();
            c.Release();
            task.WaitAndUnwrapException().ShouldBeTrue();
        }

        [TestMethod]
        [Timeout(400)]
        public void WaitAsyncTestTsTk()
        {
            var list = CreateRndUnitList(10, 100, 1, 2);
            var c = list.Combine();
            var tks = new CancellationTokenSource();
            c.WaitAsync(TimeSpan.FromMilliseconds(400), tks.Token).WaitAndUnwrapException().ShouldBeTrue();
            c.WaitAsync(TimeSpan.FromMilliseconds(100), tks.Token).WaitAndUnwrapException().ShouldBeFalse();
            var task1 = c.WaitAsync(TimeSpan.FromMilliseconds(1000), tks.Token);
            task1.IsCompleted.ShouldBeFalse();
            c.Release();
            task1.WaitAndUnwrapException().ShouldBeTrue();
            var task2 = c.WaitAsync(TimeSpan.FromMilliseconds(1000), tks.Token);
            task2.IsCompleted.ShouldBeFalse();
            tks.Cancel();
            Assert.ThrowsException<OperationCanceledException>(task2.WaitAndUnwrapException);
        }

        [TestMethod]
        public void WaitTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void WaitTest1()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void WaitTest2()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void WaitTest3()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void WaitTest4()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void WaitTest5()
        {
            throw new NotImplementedException();
        }
    }
}