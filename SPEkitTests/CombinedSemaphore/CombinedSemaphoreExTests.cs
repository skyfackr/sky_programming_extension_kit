using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using FluentAssert;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            for (var i = 1; i <= rnd.Next(10, 100); i++) s2.Add(new Semaphore(1, 2));

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
            throw new NotImplementedException();
        }

        [TestMethod]
        [Timeout(500)]
        public void CombineTestUnit_IEUnit()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        [Timeout(500)]
        public void CombineTestIEUnit_IEUnit()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        [Timeout(500)]
        public void CombineTestWin32_ArrWin32()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        [Timeout(500)]
        public void CombineTestSlim_ArrSlim()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        [Timeout(500)]
        public void CombineTestUnit_ArrUnit()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        [Timeout(500)]
        public void CombineTestIEUnit_ArrUnit()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        [Timeout(500)]
        public void CombineTestIEWin32()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        [Timeout(500)]
        public void CombineTestIESlim()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        [Timeout(500)]
        public void CombineTestIEUnit()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        [Timeout(500)]
        public void AbsorbTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        [Timeout(500)]
        public void ToSemaphoreUnitTestWin32()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        [Timeout(500)]
        public void ToSemaphoreUnitTestSlim()
        {
            throw new NotImplementedException();
        }
    }
}