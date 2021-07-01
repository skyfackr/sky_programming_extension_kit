using System;
using System.Threading;
using PostSharp.Aspects;

namespace SPEkit.SemaphoreSlimAttribute
{
    public abstract partial class AbstractSlot
    {
        /// <exception cref="WaitCancelledOrFailedException"></exception>
        /// <inheritdoc />
        public override void OnEntry(MethodExecutionArgs args)
        {
            base.OnEntry(args);
            AssertInitialized();
            args.MethodExecutionTag = new SessionsStatus(false);
            if (CheckDisposed()) return;

            try
            {
                var tag = new SessionsStatus(TryEntry());
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

        /// <exception cref="WaitCancelledOrFailedException"></exception>
        /// <inheritdoc />
        public override void OnExit(MethodExecutionArgs args)
        {
            base.OnExit(args);

            if (args.MethodExecutionTag is not SessionsStatus session)
                throw new WaitCancelledOrFailedException(GetAssignedMethodInternal(), CancelFlag.InternalError);
            if (CheckDisposed()) return;

            if (!session.IsEntered) return;
            try
            {
                Release();
            }
            catch (ObjectDisposedException)
            {
            }
            catch (SemaphoreFullException e)
            {
                throw new WaitCancelledOrFailedException(GetAssignedMethodInternal(), CancelFlag.MaxCountExceeded,
                    e);
            }
            catch (Exception e)
            {
                throw new WaitCancelledOrFailedException(GetAssignedMethodInternal(), CancelFlag.Unknown, e);
            }
        }

        /// <summary>
        ///     在默认<see cref="OnEntry" />代码中，通过此函数执行等待信号量逻辑
        /// </summary>
        /// <returns></returns>
        protected abstract bool TryEntry();
    }
}