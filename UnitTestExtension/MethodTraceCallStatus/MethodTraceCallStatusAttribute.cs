using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using PostSharp.Aspects;
using PostSharp.Constraints;

namespace SPEkit.UnitTestExtension
{
    
    public sealed partial class MethodTraceCallStatusAttribute
    {
        public MethodTraceCallStatusAttribute():base(){}
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
            public TraceStatus Status { get; internal set; } = TraceStatus.NotStart;
            public object[] Arguments { get; internal set; } = { };
            public object ReturnValue { get; internal set; } = null;
            public Exception exce { get; internal set; } = null;

            public TimeSpan ExcuteTime
            {
                get;
                internal set;
            } = new(0, 0, 0);

            //internal DateTime? _ThisEventExecutionStartTime = null;
            internal Stopwatch _stopwatch = new Stopwatch();
            public DateTime? StartTime { get; internal set; } = null;
            public DateTime? EndTime { get; internal set; } = null;
            public StackTrace Stack { get; internal set; } = null;
        }

        //public TraceStatus Status { get; private set; } = TraceStatus.NotStart;
        private Dictionary<object, CallSession> Sessions  = new Dictionary<object, CallSession>();
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
    }
}
