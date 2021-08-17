using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SPEkit.WinFormVarData
{
    public sealed partial class DataBinder<T>
    {
        public bool IsDisposed { get; private set; } = false;
        private readonly object m_disposeLock = new();
        public void Dispose()
        {
            lock (m_disposeLock)
            {
                if (IsDisposed) return;
                IsDisposed = true;
                ClearAllNotify();
                (m_data as IDisposable)?.Dispose();
                m_data = default(T);
                //todo 未完成
            }

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AssertDisposed()
        {
            if (IsDisposed) throw new ObjectDisposedException(this.ToString());
        }

        public async ValueTask DisposeAsync()
        {
            if (IsDisposed) return;
            await Task.Run(Dispose).ConfigureAwait(false);
        }
    }
}
