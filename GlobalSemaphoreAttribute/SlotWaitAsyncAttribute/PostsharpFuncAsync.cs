using System;
using System.Threading.Tasks;
using PostSharp.Aspects;

namespace SPEkit.SemaphoreSlimAttribute
{
    public sealed partial class SlotWaitAsyncAttribute
    {
        protected override bool TryEntry()
        {
            throw new WaitCancelledOrFailedException(GetAssignedMethodInternal(), CancelFlag.InternalError);
        }

        private async Task<bool> TryEntryAsync()
        {
            var opt = Option.Clone();
            if (opt.Token == null && opt.WaitingTimePerWait == null)
            {
                await WaitAsync();
                return true;
            }

            if (opt.Token != null && opt.WaitingTimePerWait == null)
            {
                await WaitAsync(opt.Token.Value);
                return true;
            }

            if (opt.Token == null && opt.WaitingTimePerWait != null)
                return await WaitAsync(opt.WaitingTimePerWait.Value);
            return await WaitAsync(opt.WaitingTimePerWait.Value, opt.Token.Value);
        }

        public override async void OnEntry(MethodExecutionArgs args)
        {
            AssertInitialized();
            args.MethodExecutionTag = new SessionsStatus(false);
            if (CheckDisposed()) return;

            try
            {
                var tag = new SessionsStatus(await TryEntryAsync());
                args.MethodExecutionTag = tag;
                if (!tag.IsEntered)
                    throw new WaitCancelledOrFailedException(GetAssignedMethodInternal(), CancelFlag.Timeout);
            }
            catch (OperationCanceledException e)
            {
                throw new WaitCancelledOrFailedException(GetAssignedMethodInternal(), CancelFlag.CancelledByToken, e);
            }
            catch (ObjectDisposedException)
            {
            }
            catch (Exception e)
            {
                throw new WaitCancelledOrFailedException(GetAssignedMethodInternal(), CancelFlag.Unknown, e);
            }
        }
    }
}