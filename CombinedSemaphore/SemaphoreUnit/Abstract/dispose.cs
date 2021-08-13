using System;

namespace SPEkit.CombinedSemaphore.Unit
{
    public abstract partial class SemaphoreUnit : IDisposable
    {
        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     dispose时执行
        /// </summary>
        /// <param name="disposing">若为true则代表从<see cref="IDisposable.Dispose()" />调用</param>
        protected abstract void Dispose(bool disposing);
    }
}