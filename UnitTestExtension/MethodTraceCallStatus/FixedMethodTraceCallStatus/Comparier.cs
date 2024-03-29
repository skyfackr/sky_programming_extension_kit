﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SPEkit.UnitTestExtension
{
    public sealed partial class FixedMethodTraceCallStatus : IComparable<FixedMethodTraceCallStatus>, IComparable,
        IEquatable<FixedMethodTraceCallStatus>
    {
        /// <inheritdoc />
        public int CompareTo(object obj)
        {
            if (ReferenceEquals(null, obj)) return 1;
            if (ReferenceEquals(this, obj)) return 0;
            return obj is FixedMethodTraceCallStatus other
                ? CompareTo(other)
                : throw new ArgumentException($"Object must be of type {nameof(FixedMethodTraceCallStatus)}");
        }

        /// <inheritdoc />
        public int CompareTo(FixedMethodTraceCallStatus other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            var nameComparison = string.Compare(Name, other.Name, StringComparison.CurrentCultureIgnoreCase);
            if (nameComparison != 0) return nameComparison;
            return MetaDataToken.CompareTo(other.MetaDataToken);
        }

        /// <inheritdoc />
        public bool Equals(FixedMethodTraceCallStatus other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return MetaDataToken == other.MetaDataToken && MethodEquals(Method, other.Method) &&
                   SessionsEquals(other);
        }

        private static bool MethodEquals(MethodBase a, MethodBase b)
        {
            if (a == null && b == null) return true;
            // ReSharper disable twice ConditionIsAlwaysTrueOrFalse
            if (a == null && b != null || a != null && b == null) return false;
            return a.Equals(b);
        }

        private bool SessionsEquals(FixedMethodTraceCallStatus others)
        {
            var otherSessions = others.Sessions;
            return Sessions.Count == otherSessions.Count && Sessions.All(session =>
                otherSessions.ContainsKey(session.Key) && session.Value.Equals(otherSessions[session.Key]));
        }

        /// <summary>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator <(FixedMethodTraceCallStatus left, FixedMethodTraceCallStatus right)
        {
            return Comparer<FixedMethodTraceCallStatus>.Default.Compare(left, right) < 0;
        }

        /// <summary>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator >(FixedMethodTraceCallStatus left, FixedMethodTraceCallStatus right)
        {
            return Comparer<FixedMethodTraceCallStatus>.Default.Compare(left, right) > 0;
        }

        /// <summary>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator <=(FixedMethodTraceCallStatus left, FixedMethodTraceCallStatus right)
        {
            return Comparer<FixedMethodTraceCallStatus>.Default.Compare(left, right) <= 0;
        }

        /// <summary>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator >=(FixedMethodTraceCallStatus left, FixedMethodTraceCallStatus right)
        {
            return Comparer<FixedMethodTraceCallStatus>.Default.Compare(left, right) >= 0;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is FixedMethodTraceCallStatus other && Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(MetaDataToken, Method, Sessions);
        }

        /// <summary>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(FixedMethodTraceCallStatus left, FixedMethodTraceCallStatus right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(FixedMethodTraceCallStatus left, FixedMethodTraceCallStatus right)
        {
            return !Equals(left, right);
        }
    }
}