using System;
using System.Threading;
using SPEkit.BinLikeClassSelectors;

namespace SPEkit.CombinedSemaphore.Utils
{
    /// <summary>
    ///     等待设定用标志
    /// </summary>
    [Flags]
    public enum WaitActionFlag
    {
        /// <summary>
        ///     忽略disposed
        /// </summary>
        /// <remarks>与<see cref="ThrowWhenDisposed" />冲突，同时存在时，忽略此标志</remarks>
        IgnoreDisposed = 1 << 0, //Dispose second

        /// <summary>
        ///     不忽略disposed并丢异常
        /// </summary>
        /// <remarks>与<see cref="IgnoreDisposed" />冲突，同时存在时，以此标志为准</remarks>
        ThrowWhenDisposed = 1 << 1, //Dispose first

        /// <summary>
        ///     遇到<see cref="SemaphoreFullException" />时，启动还原并丢出异常
        /// </summary>
        /// <remarks>与<see cref="ContinueAndIgnoreWhenReleaseExceeded" />冲突，同时存在时，以此标志为准</remarks>
        RecoveryAndThrowWhenReleaseExceeded = 1 << 2, //RecoveryRelease first

        /// <summary>
        ///     遇到<see cref="SemaphoreFullException" />时，忽略并继续处理
        /// </summary>
        /// <remarks>与<see cref="RecoveryAndThrowWhenReleaseExceeded" />冲突，同时存在时，忽略此标志</remarks>
        ContinueAndIgnoreWhenReleaseExceeded = 1 << 3, //RecoveryRelease second

        //ThrowWhenReleaseExceeded=1<<4,//ThrowRelease first
        /// <summary>
        ///     全部标志的集合
        /// </summary>
        All = IgnoreDisposed | ThrowWhenDisposed | RecoveryAndThrowWhenReleaseExceeded |
              ContinueAndIgnoreWhenReleaseExceeded,

        /// <summary>
        ///     啥都没得
        /// </summary>
        None = 0
    }

    /// <summary>
    ///     <see cref="WaitActionFlag" />的拓展
    /// </summary>
    public static class WaitActionFlagEx
    {
        internal static WaitActionFlag IgnoreConflictFlag(this WaitActionFlag flag)
        {
            var sel = flag.CreateBinLikeClassSelectorUnit();
            //var ans = WaitActionFlag.None;
            sel.MatchDo(WaitActionFlag.IgnoreDisposed | WaitActionFlag.ThrowWhenDisposed, _ =>
            {
                sel = (sel.GetEnum<WaitActionFlag>() & (WaitActionFlag.All ^ WaitActionFlag.IgnoreDisposed))
                    .CreateBinLikeClassSelectorUnit();
            }).MatchDo(
                WaitActionFlag.RecoveryAndThrowWhenReleaseExceeded |
                WaitActionFlag.ContinueAndIgnoreWhenReleaseExceeded,
                _ =>
                {
                    sel = (sel.GetEnum<WaitActionFlag>() &
                           (WaitActionFlag.All ^ WaitActionFlag.ContinueAndIgnoreWhenReleaseExceeded))
                        .CreateBinLikeClassSelectorUnit();
                });
            return sel.GetEnum<WaitActionFlag>();
        }
    }
}