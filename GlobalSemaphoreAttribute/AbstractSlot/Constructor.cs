using System.Threading;

namespace SPEkit.SemaphoreSlimAttribute
{
    public abstract partial class AbstractSlot
    {
        /// <summary>
        ///     创建一个没有初始化的对象
        /// </summary>
        /// <remarks>
        ///     在未初始化状态下调用被注册函数将引发<see cref="NotInitializedException" />
        /// </remarks>
        protected AbstractSlot() : this(null)
        {
            //if (semaphore != null) Initialize(semaphore);
        }

        /// <summary>
        ///     直接新建<see cref="SemaphoreSlim" />对象进行初始化
        /// </summary>
        /// <param name="initialCount">初始信号量</param>
        /// <param name="maxCount">最大信号量，如为负数则视为不存在</param>
        protected AbstractSlot(int initialCount, int maxCount = -1) : this(null)
        {
            Initialize(maxCount < 0
                ? new SemaphoreSlim(initialCount)
                : new SemaphoreSlim(initialCount, maxCount));
        }

        private AbstractSlot(WaitingOption option)
        {
            Option = option ?? new WaitingOption();
        }
    }
}