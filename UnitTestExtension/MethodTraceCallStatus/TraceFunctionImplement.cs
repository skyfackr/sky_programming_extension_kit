using System;
using System.Diagnostics;
using System.Reflection;
using PostSharp.Aspects;

namespace SPEkit.UnitTestExtension
{
    public sealed partial class MethodTraceCallStatusAttribute
    {
        /// <inheritdoc />
        public override void OnEntry(MethodExecutionArgs args)
        {
            if (TestExSwitch.CheckOff()) return;


            base.OnEntry(args);
            var tag = new object();
            var session = new CallSession();

            while (!_sessions.TryAdd(tag, session)) tag = new object();


            args.MethodExecutionTag = tag;
            args.FlowBehavior = FlowBehavior.Default;
            //session._ThisEventExecutionStartTime=DateTime.UtcNow;
            session.StartTime = DateTime.UtcNow;
            session.Status = TraceStatus.Running;
            session.Arguments = args.Arguments.ToArray();
            session.Stack = new StackTrace(1, true);

            session._stopwatch.Reset();
            session._stopwatch.Start();
        }

        /// <inheritdoc />
        public override void OnExit(MethodExecutionArgs args)
        {
            if (TestExSwitch.CheckOff()) return;

            base.OnExit(args);
            //var tag = args.MethodExecutionTag;
            var session = GetSession(args.MethodExecutionTag);
            session.EndTime = DateTime.UtcNow;
            if (session._stopwatch.IsRunning) session._stopwatch.Stop();
            session.ExcuteTime = session._stopwatch.Elapsed;
            if (session.Status == TraceStatus.Pause)
                session.Status = TraceStatus.Success;
        }

        /// <inheritdoc />
        public override void OnSuccess(MethodExecutionArgs args)
        {
            if (TestExSwitch.CheckOff()) return;


            base.OnSuccess(args);
            var session = GetSession(args.MethodExecutionTag);
            session.Status = TraceStatus.Success;
            session.ReturnValue = args.ReturnValue;
        }

        /// <inheritdoc />
        public override void OnException(MethodExecutionArgs args)
        {
            if (TestExSwitch.CheckOff()) return;


            base.OnException(args);
            var session = GetSession(args.MethodExecutionTag);
            session.Status = TraceStatus.Fail;
            session.exce = args.Exception;
        }

        ///<inheritdoc />
        public override void OnYield(MethodExecutionArgs args)
        {
            if (TestExSwitch.CheckOff()) return;

            base.OnYield(args);
            var session = GetSession(args.MethodExecutionTag);
            session._stopwatch.Stop();
            session.Status = TraceStatus.Pause;
        }

        /// <inheritdoc />
        public override void OnResume(MethodExecutionArgs args)
        {
            if (TestExSwitch.CheckOff()) return;

            base.OnResume(args);
            var session = GetSession(args.MethodExecutionTag);
            session.Status = TraceStatus.Running;
            session._stopwatch.Start();
        }

        /// <inheritdoc />
        public override void RuntimeInitialize(MethodBase method)
        {
            base.RuntimeInitialize(method);
            Method = method;
            _registerMe();
        }
    }
}