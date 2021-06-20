using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using FluentAssert;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace SPEkit.UnitTestExtension.Tests
{
    //[TestClass()]
    public partial class MethodTraceCallStatusAttributeTests
    {
        [TestMethod]
        [Timeout(1500)]
        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public void ToReadFriendlyTestNoParam()
        {
            var a = MethodTraceCallStatusAttribute.GetAttribute(MethodTraceCallStatusUtils.ReadFriendlyTestFunc());
            Trace.WriteLine(a.ToFixed().ToJson(Formatting.Indented));
            MethodTraceCallStatusUtils.ReadFriendlyTestFunc();
            Trace.WriteLine(a.ToFixed().ToReadFriendly());
            var b = MethodTraceCallStatusAttribute.GetAttribute(ReadFriendlyTestFunc2());
            Trace.WriteLine(b.ToFixed().ToJson(Formatting.Indented));
            Trace.WriteLine(b.ToFixed().ToReadFriendly());
        }

        [TestMethod]
        [Timeout(1500)]
        public void ToReadFriendlyAsyncTestNoParam()
        {
            var a = _getInstance().ToFixed();
            a.ToReadFriendlyAsync().GetAwaiter().GetResult().ShouldBeEqualTo(a.ToReadFriendly());
        }

        [TestMethod]
        [Timeout(500)]
        public void SetConverterTestInstance()
        {
            var me = _getInstance().ToFixed();
            //var ori = FixedMethodTraceCallStatus.Converter;
            FixedMethodTraceCallStatus.SetConverter(new ReadFriendlyConverterTestHelper());
            ConverterTester(me);
            FixedMethodTraceCallStatus.SetConverter(new DefaultReadFriendlyConverter());
            FixedMethodTraceCallStatus.Converter.ShouldBeOfType<DefaultReadFriendlyConverter>();
        }

        private static void ConverterTester(FixedMethodTraceCallStatus me)
        {
            FixedMethodTraceCallStatus.Converter.ShouldBeOfType<ReadFriendlyConverterTestHelper>();
            me.ToReadFriendly().ShouldBeEqualTo($"{nameof(ReadFriendlyConverterTestHelper.Convert)}{nameof(me)}{me}");
            me.ToReadFriendlyAsync().GetAwaiter().GetResult()
                .ShouldBeEqualTo($"{nameof(ReadFriendlyConverterTestHelper.ConvertAsync)}{nameof(me)}{me}");
        }

        [TestMethod]
        [Timeout(500)]
        public void SetConverterTestGeneric()
        {
            var me = _getInstance().ToFixed();
            //var ori = FixedMethodTraceCallStatus.Converter;
            FixedMethodTraceCallStatus.SetConverter<ReadFriendlyConverterTestHelper>();
            ConverterTester(me);
            FixedMethodTraceCallStatus.SetConverter<DefaultReadFriendlyConverter>();
            FixedMethodTraceCallStatus.Converter.ShouldBeOfType<DefaultReadFriendlyConverter>();
        }

        [TestMethod]
        [Timeout(1000)]
        public void SetConverterTestBoxed()
        {
            var me = _getInstance().ToFixed();
            //var ori = FixedMethodTraceCallStatus.Converter;
            FixedMethodTraceCallStatus.SetConverter(typeof(ReadFriendlyConverterTestHelper));
            ConverterTester(me);
            FixedMethodTraceCallStatus.SetConverter(typeof(DefaultReadFriendlyConverter));
            FixedMethodTraceCallStatus.Converter.ShouldBeOfType<DefaultReadFriendlyConverter>();
        }

        [TestMethod]
        [Timeout(1500)]
        public void ToReadFriendlyTestGeneric()
        {
            var a = _getInstance().ToFixed();
            a.ToReadFriendly<ReadFriendlyConverterTestHelper>()
                .ShouldBeEqualTo(new ReadFriendlyConverterTestHelper().Convert(a));
        }

        [TestMethod]
        [Timeout(1500)]
        public void ToReadFriendlyAsyncTestGeneric()
        {
            var a = _getInstance().ToFixed();
            a.ToReadFriendlyAsync<ReadFriendlyConverterTestHelper>().GetAwaiter().GetResult()
                .ShouldBeEqualTo(new ReadFriendlyConverterTestHelper().ConvertAsync(a).GetAwaiter().GetResult());
        }

        [TestMethod]
        [Timeout(1500)]
        public void ToReadFriendlyTestInstance()
        {
            var a = _getInstance().ToFixed();
            var b = new ReadFriendlyConverterTestHelper();
            a.ToReadFriendly(b).ShouldBeEqualTo(b.Convert(a));
        }

        [TestMethod]
        [Timeout(1500)]
        public void ToReadFriendlyAsyncTestInstance()
        {
            var a = _getInstance().ToFixed();
            var b = new ReadFriendlyConverterTestHelper();
            a.ToReadFriendlyAsync(b).GetAwaiter().GetResult()
                .ShouldBeEqualTo(b.ConvertAsync(a).GetAwaiter().GetResult());
        }

        [TestMethod]
        [Timeout(1500)]
        public void ToReadFriendlyTestBoxed()
        {
            var a = _getInstance().ToFixed();
            a.ToReadFriendly(typeof(ReadFriendlyConverterTestHelper))
                .ShouldBeEqualTo(new ReadFriendlyConverterTestHelper().Convert(a));
        }

        [TestMethod]
        [Timeout(1500)]
        public void ToReadFriendlyAsyncTestBoxed()
        {
            var a = _getInstance().ToFixed();
            a.ToReadFriendlyAsync(typeof(ReadFriendlyConverterTestHelper)).GetAwaiter().GetResult()
                .ShouldBeEqualTo(new ReadFriendlyConverterTestHelper().ConvertAsync(a).GetAwaiter().GetResult());
        }

        [TestMethod]
        [Timeout(2000)]
        public void SpecialFuncToReadFriendlyTest()
        {
            throw new NotImplementedException();
        }
    }
}