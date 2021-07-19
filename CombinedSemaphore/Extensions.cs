using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SPEkit.CombinedSemaphore.Unit;

//todo code review
namespace SPEkit.CombinedSemaphore.MainClass
{
    /// <summary>
    ///     <see cref="MainClass.CombinedSemaphore" />的拓展方法
    /// </summary>
    public static class CombinedSemaphoreEx
    {
        /// <summary>
        ///     将多个<see cref="Semaphore" />组合为<see cref="CombinedSemaphore" />
        /// </summary>
        /// <param name="se"></param>
        /// <param name="semaphores"></param>
        /// <returns></returns>
        public static CombinedSemaphore Combine(this Semaphore se, IEnumerable<Semaphore> semaphores)
        {
            var ans = semaphores.ToList();
            ans.Add(se);
            return new CombinedSemaphore(ans);
        }

        /// <summary>
        ///     将多个<see cref="SemaphoreSlim" />组合为<see cref="CombinedSemaphore" />
        /// </summary>
        /// <param name="se"></param>
        /// <param name="semaphores"></param>
        /// <returns></returns>
        public static CombinedSemaphore Combine(this SemaphoreSlim se, IEnumerable<SemaphoreSlim> semaphores)
        {
            var ans = semaphores.ToList();
            ans.Add(se);
            return new CombinedSemaphore(ans);
        }

        /// <summary>
        ///     将多个<see cref="SemaphoreUnit" />组合为<see cref="CombinedSemaphore" />
        /// </summary>
        /// <param name="se"></param>
        /// <param name="semaphores"></param>
        /// <returns></returns>
        public static CombinedSemaphore Combine(this SemaphoreUnit se, IEnumerable<SemaphoreUnit> semaphores)
        {
            var ans = semaphores.ToList();
            ans.Add(se);
            return new CombinedSemaphore(ans);
        }

        /// <inheritdoc
        ///     cref="Combine(System.Collections.Generic.IEnumerable{SemaphoreUnit},System.Collections.Generic.IEnumerable{SemaphoreUnit})" />
        public static CombinedSemaphore Combine(this IEnumerable<SemaphoreUnit> se,
            IEnumerable<SemaphoreUnit> semaphores)
        {
            var ans = semaphores.Union(se);
            return new CombinedSemaphore(ans);
        }

        /// <inheritdoc
        ///     cref="Combine(System.Threading.Semaphore,System.Collections.Generic.IEnumerable{System.Threading.Semaphore})" />
        public static CombinedSemaphore Combine(this Semaphore se, params Semaphore[] semaphores)
        {
            return se.Combine(semaphores.AsEnumerable());
        }

        /// <inheritdoc
        ///     cref="Combine(System.Threading.SemaphoreSlim,System.Collections.Generic.IEnumerable{System.Threading.SemaphoreSlim})" />
        public static CombinedSemaphore Combine(this SemaphoreSlim se, params SemaphoreSlim[] semaphores)
        {
            return se.Combine(semaphores.AsEnumerable());
        }

        /// <inheritdoc
        ///     cref="Combine(System.Collections.Generic.IEnumerable{SemaphoreUnit},System.Collections.Generic.IEnumerable{SemaphoreUnit})" />
        public static CombinedSemaphore Combine(this SemaphoreUnit se, params SemaphoreUnit[] semaphores)
        {
            return se.Combine(semaphores.AsEnumerable());
        }

        /// <inheritdoc
        ///     cref="Combine(System.Collections.Generic.IEnumerable{SemaphoreUnit},System.Collections.Generic.IEnumerable{SemaphoreUnit})" />
        public static CombinedSemaphore Combine(this IEnumerable<SemaphoreUnit> se,
            params SemaphoreUnit[] semaphores)
        {
            return se.Combine(semaphores.AsEnumerable());
        }

        /// <summary>
        ///     将一个<see cref="CombinedSemaphore" />内容全部转移吸收并清空，遇到重复的自动忽略
        /// </summary>
        /// <param name="dest">目标对象</param>
        /// <param name="orig">被吸收对象</param>
        public static void Absorb(this CombinedSemaphore dest, CombinedSemaphore orig)
        {
            var temp = orig.ToList();
            orig.Clear();
            foreach (var unit in temp) dest.TryAdd(unit);
        }
    }
}