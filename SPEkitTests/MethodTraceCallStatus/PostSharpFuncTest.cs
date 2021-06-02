using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using FluentAssert;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace SPEkit.UnitTestExtension.Tests
{
    public partial class MethodTraceCallStatusAttributeTests
    {
        [TestMethod]
        [Timeout(2000)]
        public void SuccessTest()
        {
            var method = SuccessTestFunc();
            var attr = _extractAttribute(method);
            for (var i = 1; i <= 4; i++) SuccessTestFunc();
            var dict = attr.GetSessions();
            dict.Count.ShouldBeEqualTo(5);
            foreach (var callSession in dict.Values)
                callSession.Status.ShouldBeEqualTo(MethodTraceCallStatusAttribute.TraceStatus.Success);
            Trace.WriteLine(attr.ToFixed().ToJson(Formatting.Indented));
        }

        [TestMethod]
        [Timeout(5000)]
        public void RunningTest()
        {
            var method = _getMethod(nameof(RunningTestFunc));
            var attr = _extractAttribute(method);
            var tks = new CancellationTokenSource();
            for (var i = 1; i <= 5; i++) RunningTestFunc(tks.Token);
            var dict = attr.GetSessions();
            dict.Count.ShouldBeEqualTo(5);
            foreach (var callSession in dict.Values)
                callSession.Status.ShouldBeEqualTo(MethodTraceCallStatusAttribute.TraceStatus.Running);
            tks.Cancel();
            foreach (var callSession in dict.Values)
                callSession.Status.ShouldBeEqualTo(MethodTraceCallStatusAttribute.TraceStatus.Success);
            Trace.WriteLine(attr.ToFixed().ToJson(Formatting.Indented));
        }

        [TestMethod]
        [Timeout(1000)]
        public void ExceptionTest()
        {
            var method = _getMethod(nameof(ExceptionTestFunc));
            var attr = _extractAttribute(method);
            for (var i = 1; i <= 5; i++) Assert.ThrowsException<ArgumentException>(ExceptionTestFunc);
            var dict = attr.GetSessions();
            dict.Count.ShouldBeEqualTo(5);
            foreach (var callSession in dict.Values)
                callSession.Status.ShouldBeEqualTo(MethodTraceCallStatusAttribute.TraceStatus.Fail);
            Trace.WriteLine(attr.ToFixed().ToJson(Formatting.Indented));
        }

        [TestMethod]
        [Timeout(1000)]
        public void YieldTest()
        {
            var method = _getMethod(nameof(YieldTestFunc));
            var attr = _extractAttribute(method);
            attr.RemoveAll();
            attr.GetSessions().Count.ShouldBeEqualTo(0);
            foreach (var i in YieldTestFunc())
                attr.GetSessions().First().Value.Status
                    .ShouldBeEqualTo(i == 1
                        ? MethodTraceCallStatusAttribute.TraceStatus.Pause
                        : MethodTraceCallStatusAttribute.TraceStatus.Success);

            attr.GetSessions().Count.ShouldBeEqualTo(1);
            Trace.WriteLine(attr.ToFixed().ToJson(Formatting.Indented));
        }
    }
}