using System;

namespace SPEkit.SemaphoreSlimAttribute
{
    public abstract partial class AbstractSlot : IEquatable<AbstractSlot>
    {
        public bool Equals(AbstractSlot other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && Equals(m_currentSemaphore, other.m_currentSemaphore) &&
                   Equals(m_method, other.m_method);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((AbstractSlot) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), m_currentSemaphore, m_method);
        }

        public static bool operator ==(AbstractSlot left, AbstractSlot right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AbstractSlot left, AbstractSlot right)
        {
            return !Equals(left, right);
        }

        public bool SemaphoreEquals(AbstractSlot other)
        {
            return m_currentSemaphore.Equals(other.m_currentSemaphore);
        }

        public bool MethodEquals(AbstractSlot other)
        {
            return m_method.Equals(other.m_method);
        }
    }
}