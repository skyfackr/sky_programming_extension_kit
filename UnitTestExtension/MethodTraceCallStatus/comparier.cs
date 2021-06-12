using System;
using System.Diagnostics.CodeAnalysis;

namespace SPEkit.UnitTestExtension
{
    public sealed partial class MethodTraceCallStatusAttribute : IEquatable<MethodTraceCallStatusAttribute>
    {
        //private int? _hashCodeCache;
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
            //if (_hashCodeCache != null) return _hashCodeCache.Value;
            return Method != null ? HashCode.Combine(_baseHashCode, Method) : HashCode.Combine(_baseHashCode);

            //return _hashCodeCache.Value;
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

        public sealed partial class CallSession : IEquatable<CallSession>
        {
            /// <inheritdoc />
            public bool Equals(CallSession other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(_arguments, other._arguments) && Nullable.Equals(_endTime, other._endTime) &&
                       Equals(_exce, other._exce) && _excuteTime.Equals(other._excuteTime) &&
                       Equals(_returnValue, other._returnValue) && Equals(_stack, other._stack) &&
                       Nullable.Equals(_startTime, other._startTime) && _status == other._status &&
                       Equals(_stopwatch, other._stopwatch);
            }

            /// <inheritdoc />
            public override bool Equals(object obj)
            {
                return ReferenceEquals(this, obj) || obj is CallSession other && Equals(other);
            }

            /// <inheritdoc />
            public override int GetHashCode()
            {
                var hashCode = new HashCode();
                //hashCode.Add(_arguments);
                foreach (var argument in _arguments) hashCode.Add(argument);
                hashCode.Add(_endTime);
                hashCode.Add(_exce);
                hashCode.Add(_excuteTime);
                hashCode.Add(_returnValue);
                hashCode.Add(_stack);
                hashCode.Add(_startTime);
                hashCode.Add((int) _status);
                hashCode.Add(_stopwatch);
                return hashCode.ToHashCode();
            }

            /// <summary>
            /// </summary>
            /// <param name="left"></param>
            /// <param name="right"></param>
            /// <returns></returns>
            public static bool operator ==(CallSession left, CallSession right)
            {
                return Equals(left, right);
            }

            /// <summary>
            /// </summary>
            /// <param name="left"></param>
            /// <param name="right"></param>
            /// <returns></returns>
            public static bool operator !=(CallSession left, CallSession right)
            {
                return !Equals(left, right);
            }
        }
    }
}