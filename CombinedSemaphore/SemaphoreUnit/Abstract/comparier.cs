using System;

namespace SPEkit.CombinedSemaphore.Unit
{
    public abstract partial class SemaphoreUnit 
    {
        /// <inheritdoc />
        public abstract bool Equals(SemaphoreUnit other);

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == this.GetType() && Equals((SemaphoreUnit) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), GetCurrentSemaphore().GetHashCode());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(SemaphoreUnit left, SemaphoreUnit right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(SemaphoreUnit left, SemaphoreUnit right)
        {
            return !Equals(left, right);
        }
    }
}