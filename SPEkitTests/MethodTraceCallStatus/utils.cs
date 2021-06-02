using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using FluentAssert;
using JetBrains.Annotations;

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
        private void _test1()
        {
#pragma warning disable SecurityIntelliSenseCS // MS Security rules violation
            var ra = new Random();
#pragma warning restore SecurityIntelliSenseCS // MS Security rules violation
#pragma warning disable SCS0005 // Weak random number generator.
            _ = Math.Pow(ra.Next(2, 1000000), ra.Next(2, 100000));
#pragma warning restore SCS0005 // Weak random number generator.
        }

        [MethodTraceCallStatus]
        private void _test2()
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
            method.IsDefined(typeof(MethodTraceCallStatusAttribute)).ShouldBeTrue();
            return method.GetCustomAttribute<MethodTraceCallStatusAttribute>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private MethodInfo _getMethod(string name)
        {
            return typeof(MethodTraceCallStatusAttributeTests).GetMethod(name) ?? throw new ArgumentException("无法找到函数");
        }

        [MethodTraceCallStatus]
        private MethodInfo _test3()
        {
            return (MethodInfo) MethodBase.GetCurrentMethod();
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
    }
}