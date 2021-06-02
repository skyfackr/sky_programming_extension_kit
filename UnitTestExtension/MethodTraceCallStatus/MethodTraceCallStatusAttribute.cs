using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SPEkit.UnitTestExtension
{
    public sealed partial class MethodTraceCallStatusAttribute
    {
        /// <summary>
        ///     跟踪状态标识
        /// </summary>
        [Flags]
        public enum TraceStatus
        {
            /// <summary>
            ///     还没跑
            /// </summary>
            NotStart,

            /// <summary>
            ///     跑着
            /// </summary>
            Running,

            /// <summary>
            ///     跑成了
            /// </summary>
            Success,

            /// <summary>
            ///     跑路了
            /// </summary>
            Fail,

            /// <summary>
            ///     饮茶先啦（resume状态）
            /// </summary>
            Pause
        }

        //private readonly object _sessionsAddLock = new();

        //public TraceStatus Status { get; private set; } = TraceStatus.NotStart;
        private readonly ConcurrentDictionary<object, CallSession> _sessions = new();

        /// <summary>
        ///     代表设定了这个属性的方法
        /// </summary>
        public MethodBase Method { get; private set; }

        /// <summary>
        ///     获取指定跟踪会话
        /// </summary>
        /// <param name="tag">会话标识符</param>
        /// <returns>单个会话信息</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CallSession GetSession(object tag)
        {
            if (TestExSwitch.CheckOff()) return null;
            return !_sessions.ContainsKey(tag) ? null : _sessions[tag];
        }

        /// <summary>
        ///     获取会话列表不可变字典<see cref="ImmutableDictionary{TKey,TValue}" />
        /// </summary>
        /// <returns>会话列表</returns>
        public ImmutableDictionary<object, CallSession> GetSessions()
        {
            return TestExSwitch.CheckOff()
                ? ImmutableDictionary<object, CallSession>.Empty
                : _sessions.ToImmutableDictionary();
        }

        /// <summary>
        ///     转化为<see cref="FixedMethodTraceCallStatus" />
        /// </summary>
        /// <returns></returns>
        public FixedMethodTraceCallStatus ToFixed()
        {
            return new(this);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{nameof(Method)}: {Method}";
        }

        /// <summary>
        ///     会话列表定义
        /// </summary>
        [Serializable]
        public sealed class CallSession
        {
            private volatile object[] _arguments = { };
            private DateTime? _endTime;
            private volatile Exception _exce;

            private TimeSpan _excuteTime = new(0, 0, 0);
            private volatile object _returnValue;
            private volatile StackTrace _stack;
            private DateTime? _startTime;
            private volatile TraceStatus _status = TraceStatus.NotStart;

            //internal DateTime? _ThisEventExecutionStartTime = null;
            internal volatile Stopwatch _stopwatch = new();

            internal CallSession()
            {
            }

            /// <summary>
            ///     获取当前会话状态
            /// </summary>
            public TraceStatus Status
            {
                get => _status;
                internal set => _status = value;
            }

            /// <summary>
            ///     获取当前会话所传入参数
            /// </summary>
            public object[] Arguments
            {
                get => _arguments;
                internal set => _arguments = value;
            }

            /// <summary>
            ///     获取当前会话返回值
            /// </summary>
            public object ReturnValue
            {
                get => _returnValue;
                internal set => _returnValue = value;
            }

            /// <summary>
            ///     获取当前会话的异常
            /// </summary>
            public Exception exce
            {
                get => _exce;
                internal set => _exce = value;
            }

            /// <summary>
            ///     获取当前调用时间
            /// </summary>
            /// <remarks>注意如果当前处于<see cref="TraceStatus.Running" />则当前运行期间时间不计入</remarks>
            public TimeSpan ExcuteTime
            {
                get => _excuteTime;
                internal set => _excuteTime = value;
            }


            /// <summary>
            ///     开始的utc时间
            /// </summary>
            public DateTime? StartTime
            {
                get => _startTime;
                internal set => _startTime = value;
            }

            /// <summary>
            ///     结束的utc时间
            /// </summary>
            public DateTime? EndTime
            {
                get => _endTime;
                internal set => _endTime = value;
            }

            /// <summary>
            ///     函数调用栈
            /// </summary>
            public StackTrace Stack
            {
                get => _stack;
                internal set => _stack = value;
            }
        }
    }
}