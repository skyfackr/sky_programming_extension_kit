using System;
using System.Reflection;
using System.Reflection.Emit;
using FluentAssert;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SPEkit.UnitTestExtension.Tests
{
    [TestClass]
    public partial class MethodTraceCallStatusAttributeTests
    {
        [TestMethod]
        [Timeout(1500)]
        public void EqualsTestGeneric()
        {
            var a = _getInstance();
            var b = _getInstance();
            a.Equals(a).ShouldBeTrue();
            a.Equals(b).ShouldBeFalse();
            a.Equals(null).ShouldBeFalse();
            var t1 = _extractAttribute(_getMethod(_test1Name));
            a.Equals(t1).ShouldBeFalse();
            var t2 = _extractAttribute(_getMethod(_test2Name));
            t1.Equals(t2).ShouldBeFalse();
        }

        [TestMethod]
        [Timeout(800)]
        public void EqualsTestBoxed()
        {
            var t1 = _extractAttribute(_getMethod(_test1Name));
            t1.Equals(t1).ShouldBeTrue();
            // ReSharper disable once SuspiciousTypeConversion.Global
            t1.Equals("1212121212").ShouldBeFalse();
        }

        [TestMethod]
        [Timeout(2000)]
        public void GetHashCodeTest()
        {
            var a = _getInstance();
            var b = _extractAttribute(_getMethod(_test1Name));
            var acode = a.GetHashCode();
            acode.ShouldBeEqualTo(HashCode.Combine(a.GetPrivate<int>("_baseHashCode")));
            a.RuntimeInitialize(new DynamicMethod("tester", null, null));
            a.GetHashCode().ShouldBeEqualTo(acode);
            var bcode = b.GetHashCode();
            bcode.ShouldBeEqualTo(HashCode.Combine(b.GetPrivate<int>("_baseHashCode"), b.Method));
            b.SetPrivate<MethodBase>("Method", null);
            b.GetHashCode().ShouldBeEqualTo(bcode);
        }
    }
}