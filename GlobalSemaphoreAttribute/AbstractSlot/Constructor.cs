using System.Threading;

namespace SPEkit.SemaphoreSlimAttribute
{
    public abstract partial class AbstractSlot
    {
        /// <summary>
        ///     通过指定<see cref="SemaphoreSlim" />对象进行初始化
        /// </summary>
        /// <param name="semaphore">指定的对象</param>
        /// <param name="option">等待时设定</param>
        /// <remarks>
        ///     注意如果不传入<see cref="SemaphoreSlim" />对象，会使特性对象维持在未初始化状态。在未初始化状态下调用被注册函数将引发<see cref="NotInitializedException" />
        /// </remarks>
        protected AbstractSlot(SemaphoreSlim semaphore = null, WaitingOption option = null) : this(option)
        {
            if (semaphore != null) Initialize(semaphore);
        }

        /// <summary>
        ///     直接新建<see cref="SemaphoreSlim" />对象进行初始化
        /// </summary>
        /// <param name="initialCount">初始信号量</param>
        /// <param name="maxCount">最大信号量</param>
        /// <param name="option">等待时设定</param>
        protected AbstractSlot(int initialCount, int? maxCount = null, WaitingOption option = null) : this(option)
        {
            Initialize(maxCount == null
                ? new SemaphoreSlim(initialCount)
                : new SemaphoreSlim(initialCount, maxCount.Value));
        }

        private AbstractSlot(WaitingOption option)
        {
            Option = option ?? new WaitingOption();
        }
    }
}