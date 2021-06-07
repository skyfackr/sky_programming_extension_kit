using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

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
        ///     删除指定的某个会话
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public (bool, CallSession) Remove(object tag)
        {
            var ans = _sessions.TryRemove(tag, out var deleted);
            return (ans, deleted);
        }

        /// <summary>
        ///     删除全部现有会话
        /// </summary>
        public void RemoveAll()
        {
            foreach (var session in _sessions) _sessions.TryRemove(session);
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

        private static Dictionary<MethodBase, MethodTraceCallStatusAttribute> _attributes = new();
        /// <summary>
        /// 检查此函数是否注册了<see cref="MethodTraceCallStatusAttribute"/>
        /// </summary>
        /// <remarks>以反射方式添加此属性或者查询此属性均可能无效</remarks>
        /// <param name="method"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsRegistered(MethodBase method)
        {
            return _attributes.ContainsKey(method);
        }

        /// <summary>
        /// 如果注册，则返回此函数拥有的<see cref="MethodTraceCallStatusAttribute"/>
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CanBeNull]
        public static MethodTraceCallStatusAttribute GetAttribute(MethodBase method)
        {
            return !IsRegistered(method) ? null : _attributes[method];
        }
    }
}