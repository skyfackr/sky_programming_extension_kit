using System.Threading;
using SPEkit.CombinedSemaphore.error;
using SPEkit.CombinedSemaphore.Unit;

namespace SPEkit.CombinedSemaphore.MainClass
{
    public sealed partial class CombinedSemaphore
    {
        public static SemaphoreUnit CreateUnit(Semaphore semaphore)
        {
            return new SemaphoreWin32Unit(semaphore);
        }

        public static SemaphoreUnit CreateUnit(SemaphoreSlim semaphore)
        {
            return new SemaphoreSlimUnit(semaphore);
        }

        public static SemaphoreUnit CreateUnit(object semaphore)
        {
            return semaphore switch
            {
                Semaphore se => CreateUnit(se),
                SemaphoreSlim ses => CreateUnit(ses),
                _ => throw new TypeNotSupportedException(semaphore.GetType())
            };
        }
    }
}