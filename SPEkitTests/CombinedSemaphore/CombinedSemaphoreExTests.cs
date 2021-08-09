using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using FluentAssert;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPEkit.CombinedSemaphore.Unit;

namespace SPEkit.CombinedSemaphore.MainClass.Tests
{
    [TestClass]
    [SuppressMessage("Usage", "SecurityIntelliSenseCS:MS Security rules violation", Justification = "<挂起>")]
    [SuppressMessage("Security", "SCS0005:Weak random number generator.", Justification = "<挂起>")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    public class CombinedSemaphoreExTests
    {
        [TestMethod]
        [Timeout(500)]
        public void CombineTestWin32_IEWin32()
        {
            var rnd = new Random();
            var s1 = new Semaphore(1, 2);
            var s2 = new List<Semaphore>();
            var count = rnd.Next(10, 100);
            for (var i = 1; i <= count; i++) s2.Add(new Semaphore(1, 2));

            var c = s1.Combine(s2);
            c.GetAllSemaphoreWin32().Count().ShouldBeEqualTo(s2.Count + 1);
            for (var i = 1; i <= s2.Count; i++)
            {
                var index = new Index(i - 1);
                c[index].ShouldBeEqualTo(s2[index].ToSemaphoreUnit());
            }

            c[^1].ShouldBeEqualTo(s1.ToSemaphoreUnit());
        }

        [TestMethod]
        [Timeout(500)]
        public void CombineTestSlim_IESlim()
        {
            var rnd = new Random();
            var s1 = new SemaphoreSlim(1, 2);
            var s2 = new List<SemaphoreSlim>();
            var count = rnd.Next(10, 100);
            for (var i = 1; i <= count; i++) s2.Add(new SemaphoreSlim(1, 2));

            var c = s1.Combine(s2);
            c.GetAllSemaphoreWin32().Count().ShouldBeEqualTo(s2.Count + 1);
            for (var i = 1; i <= s2.Count; i++)
            {
                var index = new Index(i - 1);
                c[index].ShouldBeEqualTo(s2[index].ToSemaphoreUnit());
            }

            c[^1].ShouldBeEqualTo(s1.ToSemaphoreUnit());
        }

        [TestMethod]
        [Timeout(500)]
        public void CombineTestUnit_IEUnit()
        {
            var rnd = new Random();
            var s1 = new Semaphore(1, 2).ToSemaphoreUnit();
            var s2 = new List<SemaphoreUnit>();
            var count = rnd.Next(10, 100);
            for (var i = 1; i <= count; i++) s2.Add(new Semaphore(1, 2).ToSemaphoreUnit());

            var c = s1.Combine(s2);
            c.GetAllSemaphoreWin32().Count().ShouldBeEqualTo(s2.Count + 1);
            for (var i = 1; i <= s2.Count; i++)
            {
                var index = new Index(i - 1);
                c[index].ShouldBeEqualTo(s2[index]);
            }

            c[^1].ShouldBeEqualTo(s1);
        }

        [TestMethod]
        [Timeout(500)]
        public void CombineTestIEUnit_IEUnit()
        {
            var rnd = new Random();
            var s1 = new List<SemaphoreUnit>();
            var s2 = new List<SemaphoreUnit>();
            var count = rnd.Next(10, 100);
            for (var i = 1; i <= count; i++)
            {
                s1.Add(new SemaphoreSlim(1, 2).ToSemaphoreUnit());
                s2.Add(new Semaphore(1, 2).ToSemaphoreUnit());
            }

            s1.Combine(s2).ShouldContainAllInOrder(s2.Concat(s1));
        }

        [TestMethod]
        [Timeout(500)]
        public void CombineTestWin32_ArrWin32()
        {
            var rnd = new Random();
            var s1 = new Semaphore(1, 2);
            var s2 = new List<Semaphore>();
            var count = rnd.Next(10, 100);
            for (var i = 1; i <= count; i++) s2.Add(new Semaphore(1, 2));

            var c = s1.Combine(s2.ToArray());
            c.GetAllSemaphoreWin32().Count().ShouldBeEqualTo(s2.Count + 1);
            for (var i = 1; i <= s2.Count; i++)
            {
                var index = new Index(i - 1);
                c[index].ShouldBeEqualTo(s2[index].ToSemaphoreUnit());
            }

            c[^1].ShouldBeEqualTo(s1.ToSemaphoreUnit());
            var s3 = new Semaphore(1, 2);
            var s4 = new Semaphore(1, 3);
            s1.Combine(s3, s4).ShouldContainAllInOrder(new[] {s3, s4, s1}.Combine());
        }

        [TestMethod]
        [Timeout(500)]
        public void CombineTestSlim_ArrSlim()
        {
            var rnd = new Random();
            var s1 = new SemaphoreSlim(1, 2);
            var s2 = new List<SemaphoreSlim>();
            var count = rnd.Next(10, 100);
            for (var i = 1; i <= count; i++) s2.Add(new SemaphoreSlim(1, 2));

            var c = s1.Combine(s2.ToArray());
            c.GetAllSemaphoreWin32().Count().ShouldBeEqualTo(s2.Count + 1);
            for (var i = 1; i <= s2.Count; i++)
            {
                var index = new Index(i - 1);
                c[index].ShouldBeEqualTo(s2[index].ToSemaphoreUnit());
            }

            c[^1].ShouldBeEqualTo(s1.ToSemaphoreUnit());
            var s3 = new SemaphoreSlim(1, 2);
            var s4 = new SemaphoreSlim(1, 3);
            s1.Combine(s3, s4).ShouldContainAllInOrder(new[] {s3, s4, s1}.Combine());
        }

        [TestMethod]
        [Timeout(500)]
        public void CombineTestUnit_ArrUnit()
        {
            var rnd = new Random();
            var s1 = new Semaphore(1, 2).ToSemaphoreUnit();
            var s2 = new List<SemaphoreUnit>();
            var count = rnd.Next(10, 100);
            for (var i = 1; i <= count; i++) s2.Add(new Semaphore(1, 2).ToSemaphoreUnit());

            var c = s1.Combine(s2.ToArray());
            c.GetAllSemaphoreWin32().Count().ShouldBeEqualTo(s2.Count + 1);
            for (var i = 1; i <= s2.Count; i++)
            {
                var index = new Index(i - 1);
                c[index].ShouldBeEqualTo(s2[index]);
            }

            c[^1].ShouldBeEqualTo(s1);
            var s3 = new SemaphoreSlim(1, 2).ToSemaphoreUnit();
            var s4 = new SemaphoreSlim(1, 3).ToSemaphoreUnit();
            s1.Combine(s3, s4).ShouldContainAllInOrder(new[] {s3, s4, s1}.Combine());
        }

        [TestMethod]
        [Timeout(500)]
        public void CombineTestIEUnit_ArrUnit()
        {
            var rnd = new Random();
            var s1 = new List<SemaphoreUnit>();
            var s2 = new List<SemaphoreUnit>();
            var count = rnd.Next(10, 100);
            for (var i = 1; i <= count; i++)
            {
                s1.Add(new SemaphoreSlim(1, 2).ToSemaphoreUnit());
                s2.Add(new Semaphore(1, 2).ToSemaphoreUnit());
            }

            s1.Combine(s2.ToArray()).ShouldContainAllInOrder(s2.Concat(s1));
            var s3 = new List<SemaphoreUnit>();
            var s31 = new SemaphoreSlim(1, 2).ToSemaphoreUnit();
            var s32 = new SemaphoreSlim(1, 2).ToSemaphoreUnit();
            s3.Add(s31);
            s3.Add(s32);
            s3.AddRange(s1);
            s1.Combine(s31, s32).ShouldContainAllInOrder(s3);
        }

        [TestMethod]
        [Timeout(500)]
        public void CombineTestIEWin32()
        {
            var rnd = new Random();
            var s = new List<Semaphore>();
            var count = rnd.Next(10, 100);
            for (var i = 1; i <= count; i++) s.Add(new Semaphore(1, 2));
            s.Combine().GetAllSemaphoreWin32().ToList().ShouldContainAll(s).Count.ShouldBeEqualTo(count);
        }

        [TestMethod]
        [Timeout(500)]
        public void CombineTestIESlim()
        {
            var rnd = new Random();
            var s = new List<SemaphoreSlim>();
            var count = rnd.Next(10, 100);
            for (var i = 1; i <= count; i++) s.Add(new SemaphoreSlim(1, 2));
            s.Combine().GetAllSemaphoreSlim().ToList().ShouldContainAll(s).Count.ShouldBeEqualTo(count);
        }

        [TestMethod]
        [Timeout(500)]
        public void CombineTestIEUnit()
        {
            var rnd = new Random();
            var s = new List<SemaphoreUnit>();
            var count = rnd.Next(10, 100);
            for (var i = 1; i <= count; i++) s.Add(new Semaphore(1, 2).ToSemaphoreUnit());
            s.Combine().ShouldContainAllInOrder(s);
        }

        [TestMethod]
        [Timeout(500)]
        public void AbsorbTest()
        {
            var rnd = new Random();
            var s1 = new List<SemaphoreUnit>();
            var s2 = new List<SemaphoreUnit>();
            var count = rnd.Next(10, 100);
            for (var i = 1; i <= count; i++)
            {
                s1.Add(new SemaphoreSlim(1, 2).ToSemaphoreUnit());
                s2.Add(new Semaphore(1, 2).ToSemaphoreUnit());
            }

            var c1 = s1.Combine();
            var c2 = s2.Combine();
            c1.Absorb(c2);
            c2.Count.ShouldBeEqualTo(0);
            c1.Count.ShouldBeEqualTo(s1.Count + s2.Count);
            c1.ShouldContainAllInOrder(s1.Concat(s2));
        }

        [TestMethod]
        [Timeout(500)]
        public void ToSemaphoreUnitTestWin32()
        {
            var s = new Semaphore(1, 2);
            s.ToSemaphoreUnit().ShouldBeSameInstanceAs(CombinedSemaphore.CreateUnit(s));
        }

        [TestMethod]
        [Timeout(500)]
        public void ToSemaphoreUnitTestSlim()
        {
            var s = new SemaphoreSlim(1, 2);
            s.ToSemaphoreUnit().ShouldBeSameInstanceAs(CombinedSemaphore.CreateUnit(s));
        }
    }
}