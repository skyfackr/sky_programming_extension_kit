using System.Diagnostics.CodeAnalysis;

namespace SPEkit.SemaphoreSlimAttribute
{
    [SuppressMessage("ReSharper", "PossibleInvalidOperationException")]
    public sealed partial class SlotWaitAttribute
    {
        /// <inheritdoc />
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