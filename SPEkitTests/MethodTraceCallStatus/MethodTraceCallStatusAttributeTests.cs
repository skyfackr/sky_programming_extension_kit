using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using FluentAssert;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace SPEkit.UnitTestExtension.Tests
{
    public partial class MethodTraceCallStatusAttributeTests
    {
        [TestMethod]
        [Timeout(1500)]
        public void GetSessionTest()
        {
            var t3 = _test3();
            var t3attr = MethodTraceCallStatusAttribute.GetAttribute(t3);
            var t3dict =
                t3attr.GetPrivate<ConcurrentDictionary<object, MethodTraceCallStatusAttribute.CallSession>>(
                    "_sessions");
            t3dict.Keys.Any().ShouldBeTrue();
            foreach (var key in t3dict.Keys) t3attr.GetSession(key).ShouldBeSameInstanceAs(t3dict[key]);

            t3attr.GetSession(new object()).ShouldBeNull();
            TestExSwitch.Off();
            t3attr.GetSession(t3dict.Keys.First()).ShouldBeNull();
            TestExSwitch.On();
            t3attr.GetSession(t3dict.Keys.First()).ShouldNotBeNull();

            Trace.WriteLine(t3attr.ToFixed().ToJson(Formatting.Indented));
        }

        [TestMethod]
        [Timeout(1500)]
        public void GetSessionsTest()
        {
            var t3 = _test3();
            //Trace.WriteLine(TestExSwitch.Status);
            //var dbg1 = t3.GetCustomAttributes();
            var t3attr = MethodTraceCallStatusAttribute.GetAttribute(t3);
            var t3dict =
                t3attr.GetPrivate<ConcurrentDictionary<object, MethodTraceCallStatusAttribute.CallSession>>(
                    "_sessions");
            t3dict.Keys.Any().ShouldBeTrue();
            TestExSwitch.Off();
            t3attr.GetSessions()
                .ShouldBeEqualTo(ImmutableDictionary<object, MethodTraceCallStatusAttribute.CallSession>.Empty);
            TestExSwitch.On();
            t3dict.ToImmutableDictionary().ShouldBeEqualTo(t3attr.GetSessions());
            Trace.WriteLine(t3attr.ToFixed().ToJson(Formatting.Indented));
        }

        [TestMethod]
        [Timeout(600)]
        public void ToFixedTest()
        {
            var a = _getInstance();
            a.ToFixed().ShouldBeEqualTo(new FixedMethodTraceCallStatus(a));
        }

        [TestMethod]
        [Timeout(500)]
        public void ToStringTest()
        {
            var a = _extractAttribute(_getMethod(_test1Name));

            var Method = _getMethod(_test1Name);
            a.ToString().ShouldBeEqualTo($"{nameof(Method)}: {Method}");
            Trace.WriteLine(a.ToString());
        }
    }
}