using System;

namespace SPEkit.UnitTestExtension
{
    public sealed partial class MethodTraceCallStatusAttribute : IEquatable<MethodTraceCallStatusAttribute>
    {
        /// <inheritdoc />
        public bool Equals(MethodTraceCallStatusAttribute other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && Equals(Method, other.Method);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is MethodTraceCallStatusAttribute other && Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), Method);
        }

        /// <summary>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(MethodTraceCallStatusAttribute left, MethodTraceCallStatusAttribute right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(MethodTraceCallStatusAttribute left, MethodTraceCallStatusAttribute right)
        {
            return !Equals(left, right);
        }
    }
}