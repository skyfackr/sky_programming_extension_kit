using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SPEkit.CombinedSemaphore.error;
using SPEkit.CombinedSemaphore.Unit;

namespace SPEkit.CombinedSemaphore.MainClass
{
    public sealed partial class CombinedSemaphore
    {
        public static SemaphoreUnit CreateUnit(Semaphore semaphore)
        {
            using (s_win32DisposeCheckLock.ReaderLock())
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

        public static SemaphoreUnit CreateUnit(SemaphoreSlim semaphore)
        {
            using (s_slimDisposeCheckLock.ReaderLock())
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

        public static IEnumerable<SemaphoreUnit> CreateUnits(IEnumerable<Semaphore> semaphores)
        {
            return from se in semaphores
                select CreateUnit(se);
        }

        public static IEnumerable<SemaphoreUnit> CreateUnits(IEnumerable<SemaphoreSlim> semaphores)
        {
            return from se in semaphores
                select CreateUnit(se);
        }

        public static IEnumerable<SemaphoreUnit> CreateUnits(IEnumerable<object> semaphores)
        {
            return from se in semaphores
                select CreateUnit(se);
        }
    }
}