using SPEkit.CombinedSemaphore.error;
namespace SPEkit.CombinedSemaphore.MainClass
{
    public sealed partial class CombinedSemaphore
    {
        /// <summary>
        /// 对所有信号量释放一次
        /// </summary>
        /// <returns>返回数组，各自代表每个信号量的剩余信号</returns>
        /// <exception cref="ReleaseFailedException"></exception>
        public int[] Release()
        {
            return ReleaseProcess(unit => unit.Release());
        }

        /// <summary>
        /// 对所有信号量释放<paramref name="releaseCount"/>次
        /// </summary>
        /// <returns>返回数组，各自代表每个信号量的剩余信号</returns>
        /// <exception cref="ReleaseFailedException"></exception>
        public int[] Release(int releaseCount)
        {
            return ReleaseProcess(unit => unit.Release(releaseCount));
        }
    }
}