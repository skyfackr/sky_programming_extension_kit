using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using PostSharp.Aspects;
using PostSharp.Constraints;

namespace SPEkit.UnitTestExtension
{
    
    public sealed partial class MethodTraceCallStatusAttribute
    {
        public MethodBase Method { get; private set; } = null;
        public MethodTraceCallStatusAttribute():base(){}
        [Flags]
        public enum TraceStatus
        {
            NotStart,
            Running,
            Success,
            Fail,
            Pause
        }
        [Serializable]
        public sealed class CallSession
        {
            public TraceStatus Status
            {
                get => _status;
                internal set => _status = value;
            }

            public object[] Arguments
            {
                get => _arguments;
                internal set => _arguments = value;
            }

            public object ReturnValue
            {
                get => _returnValue;
                internal set => _returnValue = value;
            }

            public Exception exce
            {
                get => _exce;
                internal set => _exce = value;
            }

            public TimeSpan ExcuteTime
            {
                get => _excuteTime;
                internal set => _excuteTime = value;
            }

            //internal DateTime? _ThisEventExecutionStartTime = null;
            internal volatile Stopwatch _stopwatch = new Stopwatch();
            private volatile TraceStatus _status = TraceStatus.NotStart;
            private volatile object[] _arguments = { };
            private volatile object _returnValue = null;
            private volatile Exception _exce = null;
            
            private TimeSpan _excuteTime = new(0, 0, 0);
            private DateTime? _startTime = null;
            private DateTime? _endTime = null;
            private volatile StackTrace _stack = null;

            
            public DateTime? StartTime
            {
                get => _startTime;
                internal set => _startTime = value;
            }

            public DateTime? EndTime
            {
                get => _endTime;
                internal set => _endTime = value;
            }

            public StackTrace Stack
            {
                get => _stack;
                internal set => _stack = value;
            }
        }

        //public TraceStatus Status { get; private set; } = TraceStatus.NotStart;
        private volatile Dictionary<object, CallSession> Sessions  = new Dictionary<object, CallSession>();
        private readonly object _sessionsAddLock = new();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CallSession GetSession(object tag)
        {
            return Sessions[tag];
            
        }

        public ImmutableDictionary<object, CallSession> GetSessions()
        {
            return Sessions.ToImmutableDictionary();
        }

        public FixedMethodTraceCallStatus ToFixed()
        {
            return new FixedMethodTraceCallStatus(this);
        }
        public override void OnEntry(MethodExecutionArgs args)
        {
            base.OnEntry(args);
            var tag = new object();
            var session = new CallSession();
            lock (_sessionsAddLock)
            {
                while (!Sessions.TryAdd(tag, session))
                {
                    tag = new object();
                }
            }
            args.MethodExecutionTag = tag;
            args.FlowBehavior = FlowBehavior.Default;
            //session._ThisEventExecutionStartTime=DateTime.UtcNow;
            session.StartTime = DateTime.UtcNow;
            session.Status = TraceStatus.Running;
            session.Arguments = args.Arguments.ToArray();
            session.Stack = new StackTrace();
            
            session._stopwatch.Reset();
            session._stopwatch.Start();
        }

        public override void OnExit(MethodExecutionArgs args)
        {
            base.OnExit(args);
            //var tag = args.MethodExecutionTag;
            var session = GetSession(args.MethodExecutionTag);
            session.EndTime=DateTime.UtcNow;
            if (session._stopwatch.IsRunning) session._stopwatch.Stop();
            session.ExcuteTime = session._stopwatch.Elapsed;
            if (session.Status == TraceStatus.Pause)
                session.Status = TraceStatus.Success;
        }

        public override void OnSuccess(MethodExecutionArgs args)
        {
            base.OnSuccess(args);
            var session = GetSession(args.MethodExecutionTag);
            session.Status = TraceStatus.Success;
            session.ReturnValue = args.ReturnValue;
        }

        public override void OnException(MethodExecutionArgs args)
        {
            base.OnException(args);
            var session = GetSession(args.MethodExecutionTag);
            session.Status = TraceStatus.Fail;
            session.exce = args.Exception;
        }

        public override void OnYield(MethodExecutionArgs args)
        {
            base.OnYield(args);
            var session = GetSession(args.MethodExecutionTag);
            session._stopwatch.Stop();
            session.Status = TraceStatus.Pause;
        }

        public override void OnResume(MethodExecutionArgs args)
        {
            base.OnResume(args);
            var session = GetSession(args.MethodExecutionTag);
            session.Status = TraceStatus.Running;
            session._stopwatch.Start();
        }

        public override void RuntimeInitialize(MethodBase method)
        {
            base.RuntimeInitialize(method);
            this.Method = method;
        }
    }
}
