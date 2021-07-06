namespace SPEkit.CombinedSemaphore.Unit
{
    public sealed partial class SemaphoreWin32Unit
    {
        public override bool Equals(SemaphoreUnit other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (other.GetType() != GetType()) return false;
            return m_semaphore.Equals(other.GetCurrentSemaphoreAsWin32());
        }
    }
}