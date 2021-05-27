#nullable enable
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using PostSharp.Aspects;
using JetBrains.Annotations;

namespace SPEkit.UnitTestExtension
{
    /// <summary>
    ///     对函数运行情况进行跟踪记录并通过 <see cref="Stopwatch" /> 计时
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public sealed partial class MethodTraceCallStatusAttribute : OnMethodBoundaryAspect
    {
    }

    /// <summary>
    ///     对<see cref="MethodTraceCallStatusAttribute" />标记的Method进行以Class为单位的统计或者输出json格式字符
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public sealed partial class MethodTraceCallStatusSummarizeAttribute : OnMethodBoundaryAspect
    {
    }

    /// <summary>
    ///     整个组件的开关，默认为开启，当关闭后，无论什么函数都会立刻返回null或default
    /// </summary>
    public static class TestExSwitch
    {
        /// <summary>
        ///     当前状态指示，注意关闭时无法阻止已启动的函数
        /// </summary>
        public static bool Status { get; set; } = true;

        /// <summary>
        ///     打开
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void On()
        {
            Status = true;
        }

        /// <summary>
        ///     关掉
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Off()
        {
            Status = false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool CheckOff()
        {
            return !Status;
            
        }

        [SourceTemplate]
        [Macro(Target = "null",Expression = "complete()")]
        internal static void CheckSwitch([CanBeNull] this object a)
        {
            //$ if (TestExSwitch.CheckOff()) return $null$;$END$
            
        }
        [SourceTemplate]
        //[Macro(Target = "null", Expression = "complete()")]
        internal static void CheckSwitchNull([CanBeNull] this object a)
        {
            //$ if (TestExSwitch.CheckOff()) return ;$END$

        }
    }
}