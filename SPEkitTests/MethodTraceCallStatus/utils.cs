using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using FluentAssert;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SPEkit.UnitTestExtension.Tests
{
    public partial class MethodTraceCallStatusAttributeTests
    {
        private static readonly string _test1Name = "_test1";
        private static readonly string _test2Name = "_test2";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private MethodTraceCallStatusAttribute _getInstance()
        {
            return new();
        }

        [MethodTraceCallStatus]
        public void _test1()
        {
#pragma warning disable SecurityIntelliSenseCS // MS Security rules violation
            var ra = new Random();
#pragma warning restore SecurityIntelliSenseCS // MS Security rules violation
#pragma warning disable SCS0005 // Weak random number generator.
            _ = Math.Pow(ra.Next(2, 1000000), ra.Next(2, 100000));
#pragma warning restore SCS0005 // Weak random number generator.
        }

        [MethodTraceCallStatus]
        public void _test2()
        {
#pragma warning disable SecurityIntelliSenseCS // MS Security rules violation
            var ra = new Random();
#pragma warning restore SecurityIntelliSenseCS // MS Security rules violation
#pragma warning disable SCS0005 // Weak random number generator.
            _ = Math.Pow(ra.Next(2, 1000000), ra.Next(2, 100000));
#pragma warning restore SCS0005 // Weak random number generator.
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private MethodTraceCallStatusAttribute _extractAttribute(MethodBase method)
        {
            MethodTraceCallStatusAttribute.IsRegistered(method).ShouldBeTrue();
            return MethodTraceCallStatusAttribute.GetAttribute(method);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private MethodInfo _getMethod(string name)
        {
            return typeof(MethodTraceCallStatusAttributeTests).GetMethod(name) ??
                   typeof(MethodTraceCallStatusAttributeTests).GetMethod(name, BindingFlags.Public) ??
                   typeof(MethodTraceCallStatusAttributeTests).GetMethod(name, BindingFlags.NonPublic) ??
                   typeof(MethodTraceCallStatusAttributeTests).GetMethod(name, BindingFlags.Static) ??
                   throw new ArgumentException("无法找到函数");
        }

        [MethodTraceCallStatus]
        public MethodInfo _test3()
        {
            return (MethodInfo) MethodBase.GetCurrentMethod();
        }

        [MethodTraceCallStatus]
        public MethodBase SuccessTestFunc()
        {
            return MethodBase.GetCurrentMethod();
        }

        [MethodTraceCallStatus]
        public void RunningTestFunc(CancellationToken token)
        {
            var tms = new Stopwatch();
            tms.Restart();
            while (!token.IsCancellationRequested && tms.Elapsed < TimeSpan.FromSeconds(100))
            {
            }

            tms.Stop();
        }

        [MethodTraceCallStatus]
        public void ExceptionTestFunc()
        {
            throw new ArgumentException();
        }

        [MethodTraceCallStatus]
        public IEnumerable<int> YieldTestFunc()
        {
            yield return 1;
            yield return 2;
        }

        [MethodTraceCallStatus]
        internal MethodBase ReadFriendlyTestFunc2()
        {
            return MethodBase.GetCurrentMethod();
        }

        [MethodTraceCallStatus]
        [Timeout(1)]
        public int SFTRFT_ReturnsAndParamsAndAttribute(int a, int b)
        {
            return a + b;
        }

        [MethodTraceCallStatus]
        public int SFTRFT_ExceptionWithReturn()
        {
            throw new Exception("with");
        }

        [MethodTraceCallStatus]
        public void SFTRFT_ExceptionWithoutReturn()
        {
            Exception a;
            try
            {
                throw new Exception("without3");
            }
            catch (Exception e)
            {
                a = new Exception("without2", e);
            }

            throw new ArgumentException("without1", a);
        }

        [MethodTraceCallStatus]
        public void SFTRFT_ExceptionLoop()
        {
            var a = new Exception();
            //Trace.WriteLine(0);
            a.SetPrivate("_innerException", a);
            //Trace.WriteLine(1);
            throw a;
        }
    }

    public static class MethodTraceCallStatusUtils
    {
        [SourceTemplate]
        [Macro(Target = "VAR", Editable = 1)]
        internal static void Extract([CanBeNull] this string method)
        {
            //$ var $VAR$=_extractAttribute(_getMethod($method$));
            //$ $END$
        }

        [MethodTraceCallStatus]
        internal static MethodBase ReadFriendlyTestFunc()
        {
            return MethodBase.GetCurrentMethod();
        }
    }

    internal class ReadFriendlyConverterTestHelper : IReadFriendlyConverter
    {
        public int MAX_EXCEPTION_WARP_INDEX { get; set; } = 3;

        //public readonly static string TestStr=
        public string Convert(FixedMethodTraceCallStatus me, int? maxExceptionIndex = null)
        {
            return $"{nameof(Convert)}{nameof(me)}{me}";
        }

        public Task<string> ConvertAsync(FixedMethodTraceCallStatus me, int? maxExceptionIndex = null,
            CancellationToken? token = null)
        {
            return Task.FromResult($"{nameof(ConvertAsync)}{nameof(me)}{me}");
        }
    }

    [AttributeUsage(AttributeTargets.All)]
    internal class NothingAttribute : Attribute
    {
    }
}