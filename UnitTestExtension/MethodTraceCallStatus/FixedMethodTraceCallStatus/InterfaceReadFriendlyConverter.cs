using System;
using System.Threading;
using System.Threading.Tasks;

namespace SPEkit.UnitTestExtension
{
    /// <summary>
    ///     实现了可以将<see cref="FixedMethodTraceCallStatus" />转换为自定义字符串的方法的接口
    /// </summary>
    public interface IReadFriendlyConverter
    {
        /// <summary>
        ///     默认的最大<see cref="Exception" />递归数查询设置器，背后数据应当以静态存储
        /// </summary>
        public int MAX_EXCEPTION_WARP_INDEX { get; set; }

        /// <summary>
        ///     此方法可以实现将<see cref="FixedMethodTraceCallStatus" />转为字符串
        /// </summary>
        /// <param name="me"></param>
        /// <param name="maxExceptionIndex"></param>
        /// <returns></returns>
        public string Convert(FixedMethodTraceCallStatus me, int? maxExceptionIndex = null);

        /// <summary>
        ///     <see cref="Convert" />的异步重载
        /// </summary>
        /// <param name="me"></param>
        /// <param name="maxExceptionIndex"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<string> ConvertAsync(FixedMethodTraceCallStatus me, int? maxExceptionIndex = null,
            CancellationToken? token = null);
    }
}