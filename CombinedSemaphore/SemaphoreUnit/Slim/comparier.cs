namespace SPEkit.CombinedSemaphore.Unit
{
    public sealed partial class SemaphoreSlimUnit
    {
        public override bool Equals(SemaphoreUnit other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (other.GetType() != GetType()) return false;
            return m_semaphoreSlim.Equals(other.GetCurrentSemaphoreAsSlim());
        }
    }
}