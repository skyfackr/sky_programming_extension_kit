using System;
using SPEkit.BinLikeClassSelectors;

namespace SPEkit.CombinedSemaphore.Utils
{
    [Flags]
    public enum WaitActionFlag
    {
        IgnoreDisposed = 1 << 0, //Dispose second
        ThrowWhenDisposed = 1 << 1, //Dispose first
        RecoveryAndThrowWhenReleaseExceeded = 1 << 2, //RecoveryRelease first
        ContinueAndIgnoreWhenReleaseExceeded = 1 << 3, //RecoveryRelease second

        //ThrowWhenReleaseExceeded=1<<4,//ThrowRelease first
        All = IgnoreDisposed | ThrowWhenDisposed | RecoveryAndThrowWhenReleaseExceeded |
              ContinueAndIgnoreWhenReleaseExceeded,
        None = 0
    }

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