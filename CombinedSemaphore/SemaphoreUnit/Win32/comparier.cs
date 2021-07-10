namespace SPEkit.CombinedSemaphore.Unit
{
    public sealed partial class SemaphoreWin32Unit
    {
        /// <inheritdoc />
        public override bool Equals(SemaphoreUnit other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.GetType() == GetType() && m_semaphore.Equals(other.GetCurrentSemaphoreAsWin32());
        }
    }
}