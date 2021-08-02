using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SPEkit.CombinedSemaphore.error;
using SPEkit.CombinedSemaphore.Unit;
using SPEkit.CombinedSemaphore.Utils;

namespace SPEkit.CombinedSemaphore.MainClass
{
    public sealed partial class CombinedSemaphore
    {
        /// <summary>
        ///     创建一个<see cref="SemaphoreWin32Unit" />，相同的信号量对象会导致返回同一个对象
        /// </summary>
        /// <param name="semaphore"></param>
        /// <returns></returns>
        /// <remarks>应当避免传入disposed的对象，否则如果应用周期内有调用<see cref="CleanCreateUnitCache" />，可能会导致重复创建新对象</remarks>
        public static SemaphoreUnit CreateUnit(Semaphore semaphore)
        {
            using (s_win32DisposeCheckLock.ReaderLock())
            {
                using (SemaphoreSyncLock.Wait(semaphore))
                {
                    SemaphoreWin32Unit ans;
                    if (s_win32Cache.Keys.Contains(semaphore))
                    {
                        ans = s_win32Cache[semaphore];
                    }
                    else
                    {
                        ans = new SemaphoreWin32Unit(semaphore);
                        s_win32Cache[semaphore] = ans;
                    }

                    return ans;
                }
            }
        }

        /// <summary>
        ///     创建一个<see cref="SemaphoreSlimUnit" />，相同的信号量对象会导致返回同一个对象
        /// </summary>
        /// <param name="semaphore"></param>
        /// <returns></returns>
        /// <remarks>应当避免传入disposed的对象，否则如果应用周期内有调用<see cref="CleanCreateUnitCache" />，可能会导致重复创建新对象</remarks>
        public static SemaphoreUnit CreateUnit(SemaphoreSlim semaphore)
        {
            using (s_slimDisposeCheckLock.ReaderLock())
            {
                using (SemaphoreSyncLock.Wait(semaphore))
                {
                    SemaphoreSlimUnit ans;
                    if (s_slimCache.Keys.Contains(semaphore))
                    {
                        ans = s_slimCache[semaphore];
                    }
                    else
                    {
                        ans = new SemaphoreSlimUnit(semaphore);
                        s_slimCache[semaphore] = ans;
                    }

                    return ans;
                }
            }
        }

        /// <summary>
        ///     创建一个<see cref="SemaphoreUnit" />，相同的信号量对象会导致返回同一个对象，如本身为<see cref="SemaphoreUnit" />则返回自身
        /// </summary>
        /// <param name="semaphore"></param>
        /// <exception cref="TypeNotSupportedException"></exception>
        /// <returns></returns>
        /// <remarks>应当避免传入disposed的对象，否则如果应用周期内有调用<see cref="CleanCreateUnitCache" />，可能会导致重复创建新对象</remarks>
        public static SemaphoreUnit CreateUnit(object semaphore)
        {
            return semaphore switch
            {
                Semaphore se => CreateUnit(se),
                SemaphoreSlim ses => CreateUnit(ses),
                SemaphoreUnit seu => seu,
                _ => throw new TypeNotSupportedException(semaphore.GetType())
            };
        }

        /// <summary>
        ///     创建一堆<see cref="SemaphoreWin32Unit" />
        /// </summary>
        /// <param name="semaphores"></param>
        /// <returns></returns>
        /// <remarks>此函数返回延迟查询对象，将在使用时实际执行，如有异常将在实际执行时抛出</remarks>
        public static IEnumerable<SemaphoreUnit> CreateUnits(IEnumerable<Semaphore> semaphores)
        {
            return from se in semaphores
                select CreateUnit(se);
        }

        /// <summary>
        ///     创建一堆<see cref="SemaphoreSlimUnit" />
        /// </summary>
        /// <param name="semaphores"></param>
        /// <returns></returns>
        /// <remarks>此函数返回延迟查询对象，将在使用时实际执行，如有异常将在实际执行时抛出</remarks>
        public static IEnumerable<SemaphoreUnit> CreateUnits(IEnumerable<SemaphoreSlim> semaphores)
        {
            return from se in semaphores
                select CreateUnit(se);
        }

        /// <summary>
        ///     创建一堆<see cref="SemaphoreUnit" />
        /// </summary>
        /// <param name="semaphores"></param>
        /// <returns></returns>
        /// <exception cref="TypeNotSupportedException"></exception>
        /// <remarks>此函数返回延迟查询对象，将在使用时实际执行，如有异常将在实际执行时抛出</remarks>
        public static IEnumerable<SemaphoreUnit> CreateUnits(IEnumerable<object> semaphores)
        {
            return from se in semaphores
                select CreateUnit(se);
        }


        /// <summary>
        ///     创建一堆<see cref="SemaphoreWin32Unit" />，异步形式创建
        /// </summary>
        /// <param name="semaphores"></param>
        /// <returns></returns>
        /// <remarks>此函数返回延迟查询对象，将在使用时实际执行，如有异常将在实际执行时抛出</remarks>
        public static async IAsyncEnumerable<SemaphoreUnit> CreateUnitsAsync(IEnumerable<Semaphore> semaphores)
        {
            foreach (var semaphore in semaphores)
                yield return await Task.Run(() => CreateUnit(semaphore)).ConfigureAwait(false);
        }

        /// <summary>
        ///     创建一堆<see cref="SemaphoreSlimUnit" />，异步形式创建
        /// </summary>
        /// <param name="semaphores"></param>
        /// <returns></returns>
        /// <remarks>此函数返回延迟查询对象，将在使用时实际执行，如有异常将在实际执行时抛出</remarks>
        public static async IAsyncEnumerable<SemaphoreUnit> CreateUnitsAsync(IEnumerable<SemaphoreSlim> semaphores)
        {
            foreach (var semaphore in semaphores)
                yield return await Task.Run(() => CreateUnit(semaphore)).ConfigureAwait(false);
        }

        /// <summary>
        ///     创建一堆<see cref="SemaphoreUnit" />，异步形式创建
        /// </summary>
        /// <param name="semaphores"></param>
        /// <returns></returns>
        /// <exception cref="TypeNotSupportedException"></exception>
        /// <remarks>此函数返回延迟查询对象，将在使用时实际执行，如有异常将在实际执行时抛出</remarks>
        public static async IAsyncEnumerable<SemaphoreUnit> CreateUnitsAsync(IEnumerable<object> semaphores)
        {
            foreach (var semaphore in semaphores)
                yield return await Task.Run(() => CreateUnit(semaphore)).ConfigureAwait(false);
        }

        /// <inheritdoc cref="CreateUnitsAsync(System.Collections.Generic.IEnumerable{System.Threading.Semaphore})" />
        public static async IAsyncEnumerable<SemaphoreUnit> CreateUnitsAsync(IAsyncEnumerable<Semaphore> semaphores)
        {
            await foreach (var semaphore in semaphores.ConfigureAwait(false))
                yield return await Task.Run(() => CreateUnit(semaphore)).ConfigureAwait(false);
        }

        /// <inheritdoc cref="CreateUnitsAsync(System.Collections.Generic.IEnumerable{SemaphoreSlim})" />
        public static async IAsyncEnumerable<SemaphoreUnit> CreateUnitsAsync(IAsyncEnumerable<SemaphoreSlim> semaphores)
        {
            await foreach (var semaphore in semaphores.ConfigureAwait(false))
                yield return await Task.Run(() => CreateUnit(semaphore)).ConfigureAwait(false);
        }

        /// <inheritdoc cref="CreateUnitsAsync(System.Collections.Generic.IEnumerable{object})" />
        public static async IAsyncEnumerable<SemaphoreUnit> CreateUnitsAsync(IAsyncEnumerable<object> semaphores)
        {
            await foreach (var semaphore in semaphores.ConfigureAwait(false))
                yield return await Task.Run(() => CreateUnit(semaphore)).ConfigureAwait(false);
        }
    }
}