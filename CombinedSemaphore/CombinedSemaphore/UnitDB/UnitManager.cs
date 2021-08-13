using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SPEkit.CombinedSemaphore.Unit;

namespace SPEkit.CombinedSemaphore.MainClass
{
    public sealed partial class CombinedSemaphore
    {
        private readonly IList<SemaphoreUnit> m_units;


        /// <summary>
        ///     尝试添加<see cref="SemaphoreUnit" />，如果已存在则添加并返回true，否则false
        /// </summary>
        /// <param name="semaphore"></param>
        /// <returns></returns>
        /// <remarks>此函数不是线程安全的</remarks>
        public bool TryAdd(SemaphoreUnit semaphore)
        {
            //AssertNotDisposed();
            if (Contains(semaphore)) return false;
            Add(semaphore);
            return true;
        }

        /// <summary>
        ///     对全部unit执行<see cref="SemaphoreUnit.GetWaitHandle()" />并返回一个延迟执行可迭代对象用于查询结果
        /// </summary>
        /// <returns></returns>
        public IEnumerable<WaitHandle> GetWaitHandles()
        {
            AssertNotDisposed();
            return (from unit in m_units
                select unit.GetWaitHandle()).ToArray();
        }

        /// <summary>
        ///     尝试生成并添加<see cref="SemaphoreUnit" />，如果已存在则添加并返回true，否则false
        /// </summary>
        /// <param name="semaphore"></param>
        /// <returns></returns>
        /// <remarks>此函数不是线程安全的</remarks>
        public bool TryAdd(SemaphoreSlim semaphore)
        {
            //AssertNotDisposed();
            return TryAdd(CreateUnit(semaphore));
        }

        /// <inheritdoc cref="TryAdd(SemaphoreSlim)" />
        public bool TryAdd(Semaphore semaphore)
        {
            //AssertNotDisposed();
            return TryAdd(CreateUnit(semaphore));
        }

        /// <inheritdoc cref="Contains(SemaphoreUnit)" />
        public bool Contains(SemaphoreSlim semaphore)
        {
            //AssertNotDisposed();
            return GetAllSemaphoreSlim().Contains(semaphore);
        }

        /// <inheritdoc cref="Contains(SemaphoreUnit)" />
        public bool Contains(Semaphore semaphore)
        {
            //AssertNotDisposed();
            return GetAllSemaphoreWin32().Contains(semaphore);
        }

        /// <summary>
        ///     获取当前存储用列表
        /// </summary>
        /// <returns></returns>
        public IList<SemaphoreUnit> GetUnitList()
        {
            AssertNotDisposed();
            return new List<SemaphoreUnit>(m_units);
        }

        /// <summary>
        ///     过滤出所有<see cref="SemaphoreWin32Unit" />并提取出<see cref="Semaphore" />
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Semaphore> GetAllSemaphoreWin32()
        {
            AssertNotDisposed();
            return (from unit in m_units
                where unit.GetCurrentSemaphore() is Semaphore
                select unit.GetCurrentSemaphoreAsWin32()).ToArray();
        }

        /// <summary>
        ///     过滤出所有<see cref="SemaphoreSlimUnit" />并提取出<see cref="SemaphoreSlim" />
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SemaphoreSlim> GetAllSemaphoreSlim()
        {
            AssertNotDisposed();
            return (from unit in m_units
                where unit.GetCurrentSemaphore() is SemaphoreSlim
                select unit.GetCurrentSemaphoreAsSlim()).ToArray();
        }

        /// <summary>
        ///     对所有unit检测一次，然后删除其中已经被disposed的对象
        /// </summary>
        /// <returns></returns>
        /// <remarks>此函数运行期间，并不会锁定其他函数，不能保证其他函数运行时此函数已经完成全部清理，这可能导致无法避免<see cref="ObjectDisposedException" /></remarks>
        public int RemoveAllDisposedUnit()
        {
            AssertNotDisposed();
            var ans = 0;
            //m_units.AsParallel().ForAll(unit =>
            //{

            //    var disposed = false;
            //    try
            //    {
            //        var handle = unit.GetWaitHandle().SafeWaitHandle;
            //        if (handle.IsInvalid || handle.IsClosed) disposed = true;
            //    }
            //    catch (ObjectDisposedException)
            //    {
            //        disposed = true;
            //    }

            //    if (!disposed) return;
            //    m_units.Remove(unit);
            //    Interlocked.Increment(ref ans);
            //});
            foreach (var unit in GetUnitList())
            {
                var disposed = false;
                try
                {
                    var handle = unit.GetWaitHandle().SafeWaitHandle;
                    if (handle.IsInvalid || handle.IsClosed) disposed = true;
                }
                catch (ObjectDisposedException)
                {
                    disposed = true;
                }

                if (!disposed) continue;
                m_units.Remove(unit);
                ans++;
            }

            return ans;
        }

        ///// <summary>
        /////     清空内置列表
        ///// </summary>
        //public void RemoveAll()
        //{
        //    foreach (var unit in m_units) m_units.Remove(unit);
        //}
    }
}