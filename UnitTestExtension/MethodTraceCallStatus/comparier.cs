using System;
using System.Diagnostics.CodeAnalysis;

namespace SPEkit.UnitTestExtension
{
    public sealed partial class MethodTraceCallStatusAttribute : IEquatable<MethodTraceCallStatusAttribute>
    {
        private int? _hashCodeCache;
        private int _baseHashCode => base.GetHashCode();

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
        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            if (_hashCodeCache != null) return _hashCodeCache.Value;
            _hashCodeCache = Method != null ? HashCode.Combine(_baseHashCode, Method) : HashCode.Combine(_baseHashCode);

            return _hashCodeCache.Value;
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