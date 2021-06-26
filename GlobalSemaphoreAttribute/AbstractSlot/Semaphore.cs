using System;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace SPEkit.SemaphoreSlimAttribute
{
    public abstract partial class AbstractSlot
    {
        private readonly AsyncReaderWriterLock m_currentSemaphoreLock = new();
        private SemaphoreSlim m_currentSemaphore;
        private volatile bool m_Initialized;


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


        protected void Initialize(SemaphoreSlim semaphore)
        {
            CurrentSemaphore = semaphore;
            m_Initialized = true;
        }

        protected void AssertInitialized()
        {
            if (!IsInitialized())
                throw new NotInitializedException(this);
        }

        protected SemaphoreSlim GetSemaphore()
        {
            return CurrentSemaphore;
        }

        public bool IsInitialized()
        {
            return m_Initialized && m_postSharpInit;
        }

        protected virtual SemaphoreSlim CheckInitializedAndReturnSemaphore()
        {
            AssertInitialized();
            return GetSemaphore();
        }

        public virtual void SetSemaphore(SemaphoreSlim semaphore)
        {
            Initialize(semaphore);
        }

        [Obsolete("不安全行为，不建议直接获取内部对象")]
        public SemaphoreSlim UnsafeGetCurrentSemaphore()
        {
            return GetSemaphore();
        }

        #region Semaphore功能

        public int CurrentCount => CurrentSemaphore.CurrentCount;

        public int Release()
        {
            return CurrentSemaphore.Release();
        }

        public int Release(int releaseCount)
        {
            return CurrentSemaphore.Release(releaseCount);
        }

        public void Wait()
        {
            CurrentSemaphore.Wait();
        }

        public bool Wait(int millisecondsTimeout)
        {
            return CurrentSemaphore.Wait(millisecondsTimeout);
        }

        public bool Wait(int millisecondsTimeout, CancellationToken cancellationToken)
        {
            return CurrentSemaphore.Wait(millisecondsTimeout, cancellationToken);
        }

        public void Wait(CancellationToken cancellationToken)
        {
            CurrentSemaphore.Wait(cancellationToken);
        }

        public bool Wait(TimeSpan timeout)
        {
            return CurrentSemaphore.Wait(timeout);
        }

        public bool Wait(TimeSpan timeout, CancellationToken cancellationToken)
        {
            return CurrentSemaphore.Wait(timeout, cancellationToken);
        }

        public Task WaitAsync()
        {
            return CurrentSemaphore.WaitAsync();
        }

        public Task<bool> WaitAsync(int millisecondsTimeout)
        {
            return CurrentSemaphore.WaitAsync(millisecondsTimeout);
        }

        public Task<bool> WaitAsync(int millisecondsTimeout, CancellationToken cancellationToken)
        {
            return CurrentSemaphore.WaitAsync(millisecondsTimeout, cancellationToken);
        }

        public Task WaitAsync(CancellationToken cancellationToken)
        {
            return CurrentSemaphore.WaitAsync(cancellationToken);
        }

        public Task<bool> WaitAsync(TimeSpan timeout)
        {
            return CurrentSemaphore.WaitAsync(timeout);
        }

        public Task<bool> WaitAsync(TimeSpan timeout, CancellationToken cancellationToken)
        {
            return CurrentSemaphore.WaitAsync(timeout, cancellationToken);
        }

        #endregion
    }
}