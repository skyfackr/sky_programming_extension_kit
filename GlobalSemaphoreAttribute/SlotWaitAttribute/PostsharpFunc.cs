using PostSharp.Aspects;

namespace SPEkit.SemaphoreSlimAttribute
{
    public sealed partial class SlotWaitAttribute
    {
        public override void OnEntry(MethodExecutionArgs args)
        {
            base.OnEntry(args);
        }

        public override void OnExit(MethodExecutionArgs args)
        {
            base.OnExit(args);
        }
    }
}