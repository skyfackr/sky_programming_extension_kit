using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace SPEkit.SemaphoreSlimAttribute
{
    [SuppressMessage("ReSharper", "PossibleInvalidOperationException")]
    public sealed partial class SlotWaitAttribute
    {
        private static readonly Task<bool> s_trueTask = Task.FromResult(true);

        /// <inheritdoc />
        protected override Task<bool> TryEntry()

        {
            var opt = Option.Clone();
            if (opt.Token == null && opt.WaitingTimePerWait == null) //都没有
            {
                Wait();
                return s_trueTask;
            }

            if (opt.Token != null && opt.WaitingTimePerWait == null) //单token
            {
                Wait(opt.Token.Value);
                return s_trueTask;
            }

            if (opt.Token == null && opt.WaitingTimePerWait != null) //单timeout
                return Task.FromResult(Wait(opt.WaitingTimePerWait.Value));
            return Task.FromResult(Wait(opt.WaitingTimePerWait.Value, opt.Token.Value)); //都有
        }
    }
}