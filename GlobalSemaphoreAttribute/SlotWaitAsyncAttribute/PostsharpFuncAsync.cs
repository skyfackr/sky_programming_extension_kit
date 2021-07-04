using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Nito.AsyncEx.Synchronous;
using PostSharp.Aspects;

namespace SPEkit.SemaphoreSlimAttribute
{
    public sealed partial class SlotWaitAsyncAttribute
    {
        /// <inheritdoc />
        protected override bool TryEntry()
        {
            //throw new WaitCancelledOrFailedException(GetAssignedMethodInternal(), CancelFlag.InternalError);
            //try
            //{
                return Task.Run(TryEntryAsync).ConfigureAwait(false).GetAwaiter().GetResult();
            //}
            //catch (AggregateException e)
            //{
            //    throw e.GetBaseException();
            //}

        }

        [SuppressMessage("ReSharper", "PossibleInvalidOperationException")]
        [SuppressMessage("CodeQuality", "IDE0079:请删除不必要的忽略", Justification = "<挂起>")]
        private async Task<bool> TryEntryAsync()
        {
            var opt = Option.Clone();
            if (opt.Token == null && opt.WaitingTimePerWait == null)//none
            {
                await WaitAsync().ConfigureAwait(false);
                return true;
            }

            if (opt.Token != null && opt.WaitingTimePerWait == null)//token
            {
                await WaitAsync(opt.Token.Value).ConfigureAwait(false);
                return true;
            }

            if (opt.Token == null && opt.WaitingTimePerWait != null)//timeout
                return await WaitAsync(opt.WaitingTimePerWait.Value).ConfigureAwait(false);
            return await WaitAsync(opt.WaitingTimePerWait.Value, opt.Token.Value).ConfigureAwait(false);//both
        }

        /// <inheritdoc />
        public override async void OnEntry(MethodExecutionArgs args)
        {
            //Trace.WriteLine("enter in");
            await Task.Run((() =>
            {
                base.OnEntry(args);
            }));
            //Trace.WriteLine("enter exit");
        }

        private async Task OnEntryAsync(MethodExecutionArgs args)
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
            catch (WaitCancelledOrFailedException)
            {
                throw;
            }
            catch (Exception e) 
            {
                throw new WaitCancelledOrFailedException(GetAssignedMethodInternal(), CancelFlag.Unknown, e);
            }
        }

        /// <inheritdoc />
        public override async void OnExit(MethodExecutionArgs args)
        {
            //Trace.WriteLine("exit in");
            await Task.Run((() =>
            {
                base.OnExit(args);
            }));
            //Trace.WriteLine("exit out");
        }
    }
}