﻿using System;

namespace SPEkit.UnitTestExtension
{
    public sealed partial class
        MethodTraceCallStatusSummarizeAttribute : IEquatable<MethodTraceCallStatusSummarizeAttribute>
    {
        /// <inheritdoc />
        public bool Equals(MethodTraceCallStatusSummarizeAttribute other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && _ignoreThisType == other._ignoreThisType &&
                   Equals(_includeExtraTypes, other._includeExtraTypes) && Equals(_method, other._method);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is MethodTraceCallStatusSummarizeAttribute other && Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            var baseHash = HashCode.Combine(base.GetHashCode(), _ignoreThisType, _method);
            var hash = new HashCode();
            hash.Add(baseHash);
            foreach (var type in _includeExtraTypes) hash.Add(type);

            return hash.ToHashCode();
        }

        /// <summary>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(MethodTraceCallStatusSummarizeAttribute left,
            MethodTraceCallStatusSummarizeAttribute right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(MethodTraceCallStatusSummarizeAttribute left,
            MethodTraceCallStatusSummarizeAttribute right)
        {
            return !Equals(left, right);
        }
    }
}