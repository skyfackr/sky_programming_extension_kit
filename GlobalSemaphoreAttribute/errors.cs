using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace SPEkit.SemaphoreSlimAttribute
{
    /// <summary>
    ///     表示<see cref="AbstractSlot" />未初始化
    /// </summary>
    public sealed class NotInitializedException : Exception
    {
        internal NotInitializedException(object obj) : base($"Class {obj} not initialized.")
        {
        }
    }

    /// <summary>
    ///     表示<see cref="AbstractSlot" />正在被尝试重复注册到一个函数上
    /// </summary>
    public sealed class AmbiguityAssignedException : Exception
    {
        internal AmbiguityAssignedException(MethodBase method) : base(
            $"method {method} is already assigned one attribute inherited from {nameof(AbstractSlot)}")
        {
        }
    }

    /// <summary>
    ///     表示一个方法并没有被注册<see cref="AbstractSlot" />衍生特性
    /// </summary>
    public sealed class MethodNotRegisteredException : Exception
    {
        internal MethodNotRegisteredException(MethodBase method)
            : base($"method {method} have not registered any attribute inherited from {nameof(AbstractSlot)}")
        {
        }
    }

    /// <summary>
    ///     若被注册函数等待及退出时出错或者等待因配置被取消，则抛出此异常
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("CodeQuality", "IDE0079:请删除不必要的忽略", Justification = "<挂起>")]
    public sealed class WaitCancelledOrFailedException : Exception
    {
        internal WaitCancelledOrFailedException(MethodBase method, CancelFlag reason, Exception inner = null,
            bool isExecuted = false)
            : base($"Semaphore waiting of method {method} interrupted because:{Enum.GetName(reason)}", inner)
        {
            Reasons = reason;
            IsExecuted = isExecuted;
        }

        /// <summary>
        ///     表示异常抛出原因
        /// </summary>
        public CancelFlag Reasons { get; }

        /// <summary>
        ///     表示函数是否有被调用
        /// </summary>
        public bool IsExecuted { get; }
    }
}