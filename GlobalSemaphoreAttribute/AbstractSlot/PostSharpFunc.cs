using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx.Synchronous;
using PostSharp.Aspects;

namespace SPEkit.SemaphoreSlimAttribute
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public abstract partial class AbstractSlot
    {
        /// <inheritdoc />
        public override void OnInvoke(MethodInterceptionArgs args)
        {
            //base.OnInvoke(args);
            AssertInitialized();
            var isEntered = false;
            try
            {
                //进入阶段
                try
                {
                    isEntered = TryEntry().WaitAndUnwrapException();
                    if (!isEntered)
                        throw new WaitCancelledOrFailedException(GetAssignedMethodInternal(), CancelFlag.Timeout);
                }
                catch (OperationCanceledException e)
                {
                    throw new WaitCancelledOrFailedException(GetAssignedMethodInternal(), CancelFlag.CancelledByToken,
                        e);
                }
                catch (WaitCancelledOrFailedException)
                {
                    throw;
                }
                catch (ObjectDisposedException)
                {
                }
                catch (Exception e)
                {
                    throw new WaitCancelledOrFailedException(GetAssignedMethodInternal(), CancelFlag.Unknown, e);
                }

                //执行阶段
                args.Proceed();
            }
            finally
            {
                //退出阶段
                if (isEntered)
                    try
                    {
                        Release();
                    }
                    catch (ObjectDisposedException)
                    {
                    }
                    catch (SemaphoreFullException e)
                    {
                        throw new WaitCancelledOrFailedException(GetAssignedMethodInternal(),
                            CancelFlag.MaxCountExceeded,
                            e, true);
                    }
                    catch (Exception e)
                    {
                        throw new WaitCancelledOrFailedException(GetAssignedMethodInternal(), CancelFlag.Unknown, e,
                            true);
                    }
            }
        }

        /// <inheritdoc />
        public override async Task OnInvokeAsync(MethodInterceptionArgs args)
        {
            //await base.OnInvokeAsync(args);
            AssertInitialized();
            var isEntered = false;
            try
            {
                //进入阶段
                try
                {
                    isEntered = await TryEntry();
                    if (!isEntered)
                        throw new WaitCancelledOrFailedException(GetAssignedMethodInternal(), CancelFlag.Timeout);
                }
                catch (OperationCanceledException e)
                {
                    throw new WaitCancelledOrFailedException(GetAssignedMethodInternal(), CancelFlag.CancelledByToken,
                        e);
                }
                catch (WaitCancelledOrFailedException)
                {
                    throw;
                }
                catch (ObjectDisposedException)
                {
                }
                catch (Exception e)
                {
                    throw new WaitCancelledOrFailedException(GetAssignedMethodInternal(), CancelFlag.Unknown, e);
                }

                //执行阶段
                await args.ProceedAsync();
            }
            finally
            {
                //退出阶段
                if (isEntered)
                    try
                    {
                        Release();
                    }
                    catch (ObjectDisposedException)
                    {
                    }
                    catch (SemaphoreFullException e)
                    {
                        throw new WaitCancelledOrFailedException(GetAssignedMethodInternal(),
                            CancelFlag.MaxCountExceeded,
                            e, true);
                    }
                    catch (Exception e)
                    {
                        throw new WaitCancelledOrFailedException(GetAssignedMethodInternal(), CancelFlag.Unknown, e,
                            true);
                    }
            }
        }

        /// <summary>
        ///     在默认代码中，通过此函数执行等待信号量逻辑
        /// </summary>
        /// <returns></returns>
        protected abstract Task<bool> TryEntry();
    }
}