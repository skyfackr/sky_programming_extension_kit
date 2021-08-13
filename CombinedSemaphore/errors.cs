using System;
using SPEkit.CombinedSemaphore.Utils;

namespace SPEkit.CombinedSemaphore.error
{
    /// <summary>
    ///     类不支持，无法转换为对应类
    /// </summary>
    public sealed class TypeCannotConvertException : Exception
    {
        internal TypeCannotConvertException(Type type)
            : base($"This object cannot output as {type}")
        {
        }
    }

    /// <summary>
    ///     类不支持
    /// </summary>
    public sealed class TypeNotSupportedException : Exception
    {
        internal TypeNotSupportedException(Type type)
            : base($"Type {type} is not supported.")
        {
        }
    }
#pragma warning disable 1574, 1584, 1581, 1580
    /// <summary>
    ///     释放出错并触发了操作还原，可以通过<see cref="RecoverySession" />进行还原操作取消以及获取是否完成状态
    ///     也可以通过<see cref="MainClass.CombinedSemaphore.AllRecoveryCompleteEvent" />订阅完成事件
    /// </summary>
    public sealed class ReleaseFailedException : AggregateException
    {
        /// <summary>
        ///     本次释放失败产生的还原会话
        /// </summary>
        public readonly ReleaseRecoverySession RecoverySession;


        internal ReleaseFailedException(AggregateException ex, ReleaseRecoverySession session) : base(ex.Message,
            ex.InnerExceptions)
        {
            RecoverySession = session;
        }
    }
#pragma warning restore 1574, 1584, 1581, 1580
}