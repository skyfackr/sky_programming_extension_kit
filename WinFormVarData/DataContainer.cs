using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace SPEkit.WinFormVarData
{
    public sealed partial class DataBinder<T>
    {
        private T m_data;
        private readonly AsyncReaderWriterLock m_dataLock = new();

        public T GetData()
        {
            using (m_dataLock.ReaderLock())
            {
                return m_data;
            }
        }

        public async Task<T> GetDataAsync()
        {
            using (await m_dataLock.ReaderLockAsync().ConfigureAwait(false))
            {
                return m_data;
            }
        }

        public void SetData(T value)
        {
            using (m_dataLock.WriterLock())
            {
                var newData = DataChangedHook is null ? value : DataChangedHook(this, value);
                var oldData = m_data;
                m_data = newData;
                OnDataChanged?.BeginInvoke(this,oldData,value,IsDataChangeHooked,newData,null,null);
                Refresh();
            }
        }

        public async Task SetDataAsync(T value)
        {
            using (await m_dataLock.WriterLockAsync().ConfigureAwait(false))
            {
                var newData = DataChangedHook is null ? value : await Task.Run((() => DataChangedHook(this, value))).ConfigureAwait(false);
                var oldData = m_data;
                m_data = newData;
                OnDataChanged?.BeginInvoke(this, oldData, value, IsDataChangeHooked, newData, null, null);
                await RefreshAsync().ConfigureAwait(false);
            }
        }

        public T Data
        {
            get => GetData();
            set => SetData(value);
        }

        public const string DataPropertyName = nameof(Data);

        private void TrySubscription(T data)
        {

        }
    }
}
