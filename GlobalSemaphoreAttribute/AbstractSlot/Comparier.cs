using System;
using System.Diagnostics.CodeAnalysis;

namespace SPEkit.SemaphoreSlimAttribute
{
    public abstract partial class AbstractSlot : IEquatable<AbstractSlot>
    {
        /// <inheritdoc />
        public bool Equals(AbstractSlot other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && Equals(CurrentSemaphore, other.CurrentSemaphore) &&
                   Equals(m_method, other.m_method);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((AbstractSlot) obj);
        }

        /// <inheritdoc />
        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), CurrentSemaphore, m_method);
        }

        /// <summary>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(AbstractSlot left, AbstractSlot right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(AbstractSlot left, AbstractSlot right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        ///     仅对<see cref="CurrentSemaphore" />进行比较
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool SemaphoreEquals(AbstractSlot other)
        {
            return CurrentSemaphore.Equals(other.CurrentSemaphore);
        }

        /// <summary>
        ///     仅对所注册的方法进行比较
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool MethodEquals(AbstractSlot other)
        {
            return m_method.Equals(other.m_method);
        }
    }
}