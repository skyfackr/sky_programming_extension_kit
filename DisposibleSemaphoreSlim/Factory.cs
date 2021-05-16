using System;
using System.Collections.Generic;
using System.Threading;
using Nito.AsyncEx;

// ReSharper disable once CheckNamespace
namespace SPEkit.DisposableSemaphoreSlim
{
    /// <summary>
    ///     实现了<see cref="IDisposable" />的<see cref="SemaphoreSlim" />替代品，可以通过using语法糖方便使用
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed partial class DisposableSemaphoreSlim : IDisposable
    {
        private readonly object _deleteMeLock = new();
        private readonly AsyncReaderWriterLock _disposeLock = new();
        private readonly SemaphoreSlim _instance;
        private readonly List<DisposableSemaphoreUnit> _unreleasedUnit = new();
        private bool _isDisposed;

        /// <summary>
        ///     将所有生成的未标记为释放的<see cref="DisposableSemaphoreUnit" />释放，然后释放全部资源
        /// </summary>
        public void Dispose()
        {
            using (_disposeLock.WriterLock())
            {
                if (_isDisposed) return;
                ReleaseAll();

                _instance?.Dispose();
                _isDisposed = true;
            }
        }

        internal void DeleteMe(DisposableSemaphoreUnit me)
        {
            lock (_deleteMeLock)
            {
                if (_unreleasedUnit.Contains(me)) _unreleasedUnit.Remove(me);
            }
        }

        private void ReleaseAll()
        {
            foreach (var child in _unreleasedUnit.ToArray()) child.Dispose();

            _unreleasedUnit.RemoveAll(_ => true);
        }

        private DisposableSemaphoreUnit _createUnit(bool isEntered)
        {
            var instance = new DisposableSemaphoreUnit(_instance, this, isEntered);
            if (isEntered) _unreleasedUnit.Add(instance); //进锁成功才进入缓存列表
            return instance;
        }

        /// <summary>
        ///     直接返回内部的<see cref="SemaphoreSlim" />引用
        /// </summary>
        /// <returns></returns>
        [Obsolete("风险操作，不建议使用")]
        public SemaphoreSlim DangerousGetSemaphoreSlimInstance()
        {
            return _instance;
        }

        private void _assertNotDisposed()
        {
            if (_isDisposed) throw new ObjectDisposedException(ToString());
        }

        #region 构造函数

        /// <summary>
        ///     默认允许一个信号并初始即可使用
        /// </summary>
        public DisposableSemaphoreSlim()
        {
            _instance = new SemaphoreSlim(1);
        }

        /// <summary>
        ///     允许<paramref name="initialCount" />个信号并初始可使用
        /// </summary>
        /// <param name="initialCount">允许信号量</param>
        public DisposableSemaphoreSlim(int initialCount)
        {
            _instance = new SemaphoreSlim(initialCount);
        }

        /// <summary>
        ///     初始允许<paramref name="initialCount" />个信号量，并最大允许<paramref name="maxCount" />个信号量
        /// </summary>
        /// <param name="initialCount">初始允许信号量</param>
        /// <param name="maxCount">最大允许信号量</param>
        public DisposableSemaphoreSlim(int initialCount, int maxCount)
        {
            _instance = new SemaphoreSlim(initialCount, maxCount);
        }

        #endregion

        #region 原必要功能实现转换

        /// <inheritdoc cref="SemaphoreSlim.CurrentCount" />
        public int CurrentCount => _instance.CurrentCount;

        /// <inheritdoc cref="SemaphoreSlim.Release()" />
        public int Release()
        {
            return _instance.Release();
        }

        /// <inheritdoc cref="SemaphoreSlim.Release(int)" />
        public int Release(int releaseCount)
        {
            return _instance.Release(releaseCount);
        }

        #endregion
    }
}