using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace SPEkit.SemaphoreSlimAttribute
{
    public sealed partial class SlotWaitAsyncAttribute
    {
        /// <inheritdoc />
        [SuppressMessage("ReSharper", "PossibleInvalidOperationException")]
        protected override async Task<bool> TryEntry()
        {
            var opt = Option.Clone();
            if (opt.Token == null && opt.WaitingTimePerWait == null) //none
            {
                await WaitAsync().ConfigureAwait(false);
                return true;
            }

            if (opt.Token != null && opt.WaitingTimePerWait == null) //token
            {
                await WaitAsync(opt.Token.Value).ConfigureAwait(false);
                return true;
            }

            if (opt.Token == null && opt.WaitingTimePerWait != null) //timeout
                return await WaitAsync(opt.WaitingTimePerWait.Value).ConfigureAwait(false);
            return await WaitAsync(opt.WaitingTimePerWait.Value, opt.Token.Value).ConfigureAwait(false); //both
        }
    }
}