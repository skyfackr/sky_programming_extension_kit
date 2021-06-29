using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace SPEkit.SemaphoreSlimAttribute
{
    [SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public abstract partial class AbstractSlot
    {
        private readonly AsyncReaderWriterLock m_currentSemaphoreLock = new();
        private SemaphoreSlim m_currentSemaphore;
        private volatile bool m_initialized;


        private SemaphoreSlim CurrentSemaphore
        {
            get
            {
                using (m_currentSemaphoreLock.ReaderLock())
                {
                    return m_currentSemaphore;
                }
            }
            set
            {
                using (m_currentSemaphoreLock.WriterLock())
                {
                    m_currentSemaphore = value;
                }
            }
        }


        /// <summary>
        ///     设置<see cref="CurrentSemaphore" />同时初始化，任何修改<see cref="CurrentSemaphore" />为非null值的行为均应当经过此函数
        /// </summary>
        /// <param name="semaphore"></param>
        protected void Initialize(SemaphoreSlim semaphore)
        {
            CurrentSemaphore = semaphore;
            m_initialized = true;
        }

        /// <summary>
        ///     检查是否初始化，没有的话丢异常
        /// </summary>
        /// <exception cref="NotInitializedException"></exception>
        protected void AssertInitialized()
        {
            if (!IsInitialized())
                throw new NotInitializedException(this);
        }

        /// <summary>
        ///     内部方法，获取<see cref="CurrentSemaphore" />
        /// </summary>
        /// <returns></returns>
        protected SemaphoreSlim GetSemaphore()
        {
            return CurrentSemaphore;
        }

        /// <summary>
        ///     检查此对象是否处于已完成初始化状态
        /// </summary>
        /// <returns></returns>
        public bool IsInitialized()
        {
            return m_initialized && m_postSharpInit;
        }

        /// <summary>
        ///     内部方法，可快速实现检查是否初始化，没初始化丢异常，初始化了返回<see cref="CurrentSemaphore" />
        /// </summary>
        /// <returns></returns>
        protected virtual SemaphoreSlim CheckInitializedAndReturnSemaphore()
        {
            AssertInitialized();
            return GetSemaphore();
        }

        /// <summary>
        ///     设置<see cref="CurrentSemaphore" />
        /// </summary>
        /// <param name="semaphore">待设置对象</param>
        public virtual void SetSemaphore(SemaphoreSlim semaphore)
        {
            Initialize(semaphore);
        }

        /// <summary>
        ///     直接获取当前设置的<see cref="CurrentSemaphore" />实例
        /// </summary>
        /// <returns></returns>
        [Obsolete("不安全行为，不建议直接获取内部对象")]
        public SemaphoreSlim UnsafeGetCurrentSemaphore()
        {
            return GetSemaphore();
        }

        #region Semaphore功能

        /// <inheritdoc cref="SemaphoreSlim.CurrentCount" />
        public int CurrentCount => CurrentSemaphore.CurrentCount;

        /// <inheritdoc cref="SemaphoreSlim.Release()" />
        public int Release()
        {
            return CurrentSemaphore.Release();
        }

        /// <inheritdoc cref="SemaphoreSlim.Release(int)" />
        public int Release(int releaseCount)
        {
            return CurrentSemaphore.Release(releaseCount);
        }

        /// <inheritdoc cref="SemaphoreSlim.Wait()" />
        public void Wait()
        {
            CurrentSemaphore.Wait();
        }

        /// <inheritdoc cref="SemaphoreSlim.Wait(int)" />
        public bool Wait(int millisecondsTimeout)
        {
            return CurrentSemaphore.Wait(millisecondsTimeout);
        }

        /// <inheritdoc cref="SemaphoreSlim.Wait(int,CancellationToken)" />
        public bool Wait(int millisecondsTimeout, CancellationToken cancellationToken)
        {
            return CurrentSemaphore.Wait(millisecondsTimeout, cancellationToken);
        }

        /// <inheritdoc cref="SemaphoreSlim.Wait(CancellationToken)" />
        public void Wait(CancellationToken cancellationToken)
        {
            CurrentSemaphore.Wait(cancellationToken);
        }

        /// <inheritdoc cref="SemaphoreSlim.Wait(TimeSpan)" />
        public bool Wait(TimeSpan timeout)
        {
            return CurrentSemaphore.Wait(timeout);
        }

        /// <inheritdoc cref="SemaphoreSlim.Wait(TimeSpan,CancellationToken)" />
        public bool Wait(TimeSpan timeout, CancellationToken cancellationToken)
        {
            return CurrentSemaphore.Wait(timeout, cancellationToken);
        }

        /// <inheritdoc cref="SemaphoreSlim.WaitAsync()" />
        public Task WaitAsync()
        {
            return CurrentSemaphore.WaitAsync();
        }

        /// <inheritdoc cref="SemaphoreSlim.WaitAsync(int)" />
        public Task<bool> WaitAsync(int millisecondsTimeout)
        {
            return CurrentSemaphore.WaitAsync(millisecondsTimeout);
        }

        /// <inheritdoc cref="SemaphoreSlim.WaitAsync(int,CancellationToken)" />
        public Task<bool> WaitAsync(int millisecondsTimeout, CancellationToken cancellationToken)
        {
            return CurrentSemaphore.WaitAsync(millisecondsTimeout, cancellationToken);
        }

        /// <inheritdoc cref="SemaphoreSlim.WaitAsync(CancellationToken)" />
        public Task WaitAsync(CancellationToken cancellationToken)
        {
            return CurrentSemaphore.WaitAsync(cancellationToken);
        }

        /// <inheritdoc cref="SemaphoreSlim.WaitAsync(TimeSpan)" />
        public Task<bool> WaitAsync(TimeSpan timeout)
        {
            return CurrentSemaphore.WaitAsync(timeout);
        }

        /// <inheritdoc cref="SemaphoreSlim.WaitAsync(TimeSpan,CancellationToken)" />
        public Task<bool> WaitAsync(TimeSpan timeout, CancellationToken cancellationToken)
        {
            return CurrentSemaphore.WaitAsync(timeout, cancellationToken);
        }

        #endregion
    }
}