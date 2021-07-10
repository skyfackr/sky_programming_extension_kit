namespace SPEkit.CombinedSemaphore.MainClass
{
    public sealed partial class CombinedSemaphore
    {
        public int[] Release()
        {
            return ReleaseProcess(unit => unit.Release());
        }

        public int[] Release(int releaseCount)
        {
            return ReleaseProcess(unit => unit.Release());
        }
    }
}