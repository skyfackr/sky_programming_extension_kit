using System;
using System.Runtime.CompilerServices;

namespace SPEkit.CombinedSemaphore.MainClass
{
    public sealed partial class CombinedSemaphore
    {
        private volatile bool m_isDisposed;

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