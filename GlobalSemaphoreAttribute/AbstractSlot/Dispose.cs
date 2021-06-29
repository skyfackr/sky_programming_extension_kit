using System;
using System.Threading;
using System.Threading.Tasks;

namespace SPEkit.SemaphoreSlimAttribute
{
    public abstract partial class AbstractSlot : IDisposable, IAsyncDisposable
    {
        /// <inheritdoc />
        /// <remarks>
        ///     考虑到此类使用场景，其生命周期理应同等于应用，因此其dispose状态是可以重置的，此行为不符合规范，使用时应当注意。
        ///     在dispose状态下函数请求不会被<see cref="SemaphoreSlim" />等待，因此此类会处于无效状态。所有dispose请求也均为针对<see cref="CurrentSemaphore" />的请求
        /// </remarks>
        public async ValueTask DisposeAsync()
        {
            await Task.Run(Dispose);
        }

        /// <inheritdoc />
        /// <remarks>
        ///     考虑到此类使用场景，其生命周期理应同等于应用，因此其dispose状态是可以重置的，此行为不符合规范，使用时应当注意。
        ///     在dispose状态下函数请求不会被<see cref="SemaphoreSlim" />等待，因此此类会处于无效状态。所有dispose请求也均为针对<see cref="CurrentSemaphore" />的请求
        /// </remarks>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     此函数将被<see cref="IDisposable.Dispose" />调用
        /// </summary>
        /// <param name="disposing">若为true则代表由<see cref="IDisposable.Dispose" />调用，否则是由垃圾回收器进行析构</param>
        /// <remarks>由于本类并无非托管资源，因此未重写析构器，如有需要，请同时重写此函数及析构器</remarks>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing) CurrentSemaphore?.Dispose();
        }

        /// <summary>
        ///     检查<see cref="CurrentSemaphore" />是否被dispose
        /// </summary>
        /// <returns></returns>
        protected bool CheckDisposed()
        {
            try
            {
                _ = CurrentSemaphore.AvailableWaitHandle;
                return false;
            }
            catch (ObjectDisposedException)
            {
                return true;
            }
        }

        /// <summary>
        ///     检查此特性是否处于dispose状态
        ///     <para>
        ///         此类dispose状态由<see cref="CurrentSemaphore" />的dispose状态决定，
        ///         如果<see cref="CurrentSemaphore" />被重设为一个未dispose的
        ///         对象，此对象也将解除dispose。同理，设置被dispose的对象也会改此对象状态
        ///     </para>
        ///     <para>如果此对象处于dispose状态，所有对被注册函数的调用请求将无等待放行</para>
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        ///     考虑到此类使用场景，其生命周期理应同等于应用，因此其dispose状态是可以重置的，此行为不符合规范，使用时应当注意。
        ///     在dispose状态下函数请求不会被<see cref="SemaphoreSlim" />等待，因此此类会处于无效状态。所有dispose请求也均为针对<see cref="CurrentSemaphore" />的请求
        /// </remarks>
        public virtual bool IsDisposed()
        {
            return CheckDisposed();
        }
    }
}