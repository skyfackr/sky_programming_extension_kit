using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

// ReSharper disable once CheckNamespace
namespace SPEkit.DisposableSemaphoreSlim
{
    /// <summary>
    ///     在<see cref="DisposableSemaphoreSlim" />中进锁后返回的实现了<see cref="IDisposable" />的类
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [DebuggerDisplay("Released:{IsReleased},Entered:{IsEntered}")]
    public sealed class DisposableSemaphoreUnit : IDisposable
    {
        private readonly SemaphoreSlim _instance;
        private readonly object _releaseLock = new();

        internal DisposableSemaphoreUnit(SemaphoreSlim semaphoreInstance, DisposableSemaphoreSlim father,
            bool isEntered)
        {
            _instance = semaphoreInstance;
            Factory = father;
            IsEntered = isEntered;
            if (!isEntered) //进锁失败，直接标记为释放状态
                IsReleased = true;
            //_father.DeleteMe(this);
        }

        /// <summary>
        ///     用于获取父对象引用
        /// </summary>
        public DisposableSemaphoreSlim Factory { get; }

        /// <summary>
        ///     获取此次进锁是否成功，若不成功，将直接标记为已释放
        /// </summary>
        /// <remarks>
        ///     仅当超时下进锁失败时，会返回被标记为进锁失败的实例。在通过<see cref="CancellationToken" />阻断进锁时，将由<see cref="SemaphoreSlim" />实例抛出
        ///     <see cref="OperationCanceledException" />异常
        /// </remarks>
        public bool IsEntered { get; }

        /// <summary>
        ///     标记是否已经被释放
        /// </summary>
        public bool IsReleased { get; private set; }

        /// <summary>
        ///     与<see cref="Release" />等效
        /// </summary>
        public void Dispose()
        {
            Release();
        }

        /// <summary>
        ///     一个实例最多执行一次，可以为父对象的<see cref="SemaphoreSlim" />对象增加一个信号量并标记为已释放
        /// </summary>
        /// <remarks>此方法使用同步锁</remarks>
        public void Release()
        {
            lock (_releaseLock)
            {
                if (IsReleased) return;
                _instance.Release();
                IsReleased = true;
                Factory.DeleteMe(this);
            }
        }
    }
}