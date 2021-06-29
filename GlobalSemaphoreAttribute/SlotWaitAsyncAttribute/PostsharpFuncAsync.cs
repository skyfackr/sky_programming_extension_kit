using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using PostSharp.Aspects;

namespace SPEkit.SemaphoreSlimAttribute
{
    public sealed partial class SlotWaitAsyncAttribute
    {
        /// <inheritdoc />
        protected override bool TryEntry()
        {
            throw new WaitCancelledOrFailedException(GetAssignedMethodInternal(), CancelFlag.InternalError);
        }

        [SuppressMessage("ReSharper", "PossibleInvalidOperationException")]
        [SuppressMessage("CodeQuality", "IDE0079:请删除不必要的忽略", Justification = "<挂起>")]
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

        /// <inheritdoc />
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