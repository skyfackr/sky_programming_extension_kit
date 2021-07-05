using System;
using System.Threading;
using SPEkit.BinLikeClassSelectors;

namespace SPEkit.SemaphoreSlimAttribute
{
    /// <summary>
    ///     标记取消或运行失败原因
    /// </summary>
    [Flags]
    public enum CancelFlag
    {
        /// <summary>
        ///     被<see cref="CancellationToken" />取消等待
        /// </summary>
        CancelledByToken = 1 << 0,

        /// <summary>
        ///     超时导致等待终止
        /// </summary>
        Timeout = 1 << 1,

        /// <summary>
        ///     出现未知错误
        /// </summary>
        /// <remarks>如遇此标记错误请上报issue</remarks>
        Unknown = 1 << 2,

        /// <summary>
        ///     函数退出执行信号量释放时超出最大设置，出现此错误请检查是否在其他地方出现非预期的释放
        /// </summary>
        MaxCountExceeded = 1 << 3,

        /// <summary>
        ///     出现内部错误
        /// </summary>
        /// <remarks>如遇此标记错误请上报issue</remarks>
        InternalError = 1 << 4
    }

    /// <summary>
    ///     <see cref="CancelFlag" />扩展方法集
    /// </summary>
    public static class CancelFlagEx
    {
        /// <summary>
        ///     可以将一个<see cref="CancelFlag" />转换为<see cref="BinLikeClassSelectorUnit" />
        /// </summary>
        /// <param name="me"></param>
        /// <returns></returns>
        public static BinLikeClassSelectorUnit GetSelector(this CancelFlag me)
        {
            return BinLikeClassSelectors.BinLikeClassSelector.CreateBinLikeClassSelectorUnit(Convert.ToInt64(me));
        }
    }
}