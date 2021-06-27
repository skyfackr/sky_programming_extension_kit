namespace SPEkit.SemaphoreSlimAttribute
{
    public sealed partial class SlotWaitAttribute
    {
        protected override bool TryEntry()
        {
            var opt = Option.Clone();
            if (opt.Token == null && opt.WaitingTimePerWait == null)
            {
                Wait();
                return true;
            }

            if (opt.Token != null && opt.WaitingTimePerWait == null)
            {
                Wait(opt.Token.Value);
                return true;
            }

            if (opt.Token == null && opt.WaitingTimePerWait != null)
                return Wait(opt.WaitingTimePerWait.Value);
            return Wait(opt.WaitingTimePerWait.Value, opt.Token.Value);
        }
    }
}