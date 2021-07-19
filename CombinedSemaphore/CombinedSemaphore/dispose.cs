using System;
using System.Runtime.CompilerServices;
using System.Threading;
using SPEkit.CombinedSemaphore.Unit;

namespace SPEkit.CombinedSemaphore.MainClass
{
    public sealed partial class CombinedSemaphore
    {
        private volatile bool m_isDisposed;

        /// <inheritdoc />
        /// <remarks>
        ///     警告：执行dispose会对当前此实例中存储的所有<see cref="SemaphoreUnit" />执行<see cref="IDisposable.Dispose()" />，这会使全部绑定的
        ///     <see cref="Semaphore" />或者<see cref="SemaphoreSlim" />被dispose，在部分信号量实例被外部使用的场景下，此操作可能导致意外的
        ///     <see cref="ObjectDisposedException" />，如需避免进行dispose，请先执行<see cref="Clear" />
        /// </remarks>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
            m_isDisposed = true;
        }


        private void Dispose(bool disposing)
        {
            if (!disposing) return;
            foreach (var unit in m_units)
                unit?.Dispose();
            //m_units.Remove(unit);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AssertNotDisposed()
        {
            if (m_isDisposed)
                throw new ObjectDisposedException(GetType().ToString());
        }
    }
}