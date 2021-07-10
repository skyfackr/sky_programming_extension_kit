namespace SPEkit.CombinedSemaphore.Unit
{
    public sealed partial class SemaphoreSlimUnit
    {
        /// <inheritdoc />
        public override bool Equals(SemaphoreUnit other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.GetType() == GetType() && m_semaphoreSlim.Equals(other.GetCurrentSemaphoreAsSlim());
        }
    }
}