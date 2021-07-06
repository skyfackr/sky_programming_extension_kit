using System;

namespace SPEkit.CombinedSemaphore.Unit
{
    public abstract partial class SemaphoreUnit : IEquatable<SemaphoreUnit>
    {
        public abstract bool Equals(SemaphoreUnit other);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SemaphoreUnit) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), GetCurrentSemaphore().GetHashCode());
        }

        public static bool operator ==(SemaphoreUnit left, SemaphoreUnit right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SemaphoreUnit left, SemaphoreUnit right)
        {
            return !Equals(left, right);
        }
    }
}