using System;

namespace SPEkit.SemaphoreSlimAttribute
{
    public abstract partial class AbstractSlot : IEquatable<AbstractSlot>
    {
        public bool Equals(AbstractSlot other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && Equals(CurrentSemaphore, other.CurrentSemaphore);
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((AbstractSlot) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), CurrentSemaphore);
        }

        public static bool operator ==(AbstractSlot left, AbstractSlot right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AbstractSlot left, AbstractSlot right)
        {
            return !Equals(left, right);
        }
    }
}