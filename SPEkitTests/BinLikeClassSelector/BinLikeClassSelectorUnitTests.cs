using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssert;
using SPEkit.BinLikeClassSelector;

namespace SPEkit.BinLikeClassSelector.Tests
{
    [TestClass]
    public class BinLikeClassSelectorUnitTests
    {
        private static readonly long _testBin = _byteLike2Long(1000111001);
        private static readonly long[] _testBinArrayMaker = {1000000000, 100000, 10000, 1000, 1};
        private static readonly long[] _testBinArray = (from i in _testBinArrayMaker select _byteLike2Long(i)).ToArray();

        private static long _byteLike2Long(long byteLike)
        {
            var a = byteLike.ToString();
            var ans = 0;
            for (var i = 1; i <= a.Length; i++)
                if (a[i - 1] == '1')
                    ans |= 2 << (a.Length - i + 1);

            return ans;
        }
        [TestInitialize()]
        public void Init()
        {
            Array.Sort(_testBinArray);
        }


        private static BinLikeClassSelectorUnit _getInstance()
        {
            return BinLikeClassSelector.CreateBinLikeClassSelectorUnit(_testBin);
        }

        [TestMethod]
        public void GetBinTest()
        {
            var a = _getInstance();
            Assert.AreEqual(_testBin, a.GetBin());
        }

        [TestMethod]
        public void GetValidBinListTest()
        {
            var a = _getInstance().GetValidBinList();
            Assert.IsInstanceOfType(a, typeof(List<long>));

            Trace.WriteLine("被测试数组");
            foreach (var aa in a.ToArray()) Trace.WriteLine(aa);
            Trace.WriteLine("基准数组");
            foreach (var aa in _testBinArray) Trace.WriteLine(aa);
            a.ShouldBeEqualTo(new List<long>(_testBinArray));
        }

        [TestMethod]
        public void GetValidBinArrayTest()
        {
            var a = _getInstance().GetValidBinArray();
            Trace.WriteLine("被测试数组");
            foreach (var aa in a) Trace.WriteLine(aa);
            Trace.WriteLine("基准数组");
            foreach (var aa in _testBinArray) Trace.WriteLine(aa);
            a.ShouldBeEqualTo(_testBinArray);
        }

        [TestMethod]
        public void MatchDoTest()
        {
#pragma warning disable CS0618 // 此测试方法专门测试该过时方法
            var a = _getInstance();
            var b = 1;
            var c = 1;
            BinLikeClassSelectorUnit.Executor de1 = delegate(BinLikeClassSelectorUnit me)
            {
                Assert.AreSame(a, me);
                b = 2;
            };

            BinLikeClassSelectorUnit.Executor de2 = delegate { c = 2; };
            a.MatchDo(_byteLike2Long(1001), de1).MatchDo(_byteLike2Long(1011), de2);
            Assert.AreEqual(2, b);
            Assert.AreEqual(1, c);
#pragma warning restore CS0618 // 此测试方法专门测试该过时方法
        }

        [TestMethod]
        public void MatchDoTest1()
        {
            var a = _getInstance();
            var b = 1;
            var c = 1;

            void de1(BinLikeClassSelectorUnit me)
            {
                Assert.AreSame(a, me);
                b = 2;
            }

            void de2(BinLikeClassSelectorUnit me)
            {
                c = 2;
            }

            a.MatchDo(_byteLike2Long(101000), (Action<BinLikeClassSelectorUnit>) de1)
                .MatchDo(_byteLike2Long(111100), (Action<BinLikeClassSelectorUnit>) de2);
            Assert.AreEqual(2, b);
            Assert.AreEqual(1, c);
        }

        [TestMethod]
        public void MatchTest()
        {
            var a = _getInstance();
            Assert.IsTrue(a.Match(_byteLike2Long(1000101000)));
            Assert.IsFalse(a.Match(_byteLike2Long(1000111011)));
        }

        [TestMethod]
        public void GetValidBinListAsyncTest()
        {
            var a = _getInstance();
            a.GetValidBinListAsync().GetAwaiter().GetResult().ShouldBeEqualTo(new List<long>(_testBinArray));
        }

        [TestMethod]
        public void GetValidBinArrayAsyncTest()
        {
            var a = _getInstance();
            a.GetValidBinArrayAsync().GetAwaiter().GetResult().ShouldBeEqualTo(_testBinArray);
        }

        [TestMethod]
        public void MatchAsyncTest()
        {
            var a = _getInstance();
            Assert.IsTrue(a.MatchAsync(_byteLike2Long(1000101000)).Result);
            Assert.IsFalse(a.MatchAsync(_byteLike2Long(1000111011)).Result);
        }
    }
}