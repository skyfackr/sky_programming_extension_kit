using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        }

        public TraceStatus Status { get; private set; } = TraceStatus.NotStart;
        public List<CallSession> Sessions { get; private set; } = new List<CallSession>();
        public override void OnEntry(MethodExecutionArgs args)
        {
            base.OnEntry(args);
        }
    }
}
