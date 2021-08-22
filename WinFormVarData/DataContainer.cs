using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Nito.AsyncEx;
using PostSharp.Extensibility;
using INotifyPropertyChanged = System.ComponentModel.INotifyPropertyChanged;

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
                ClearDataSourceWithoutLock();
                var newData = DataChangedHook is null ? value : DataChangedHook(this, value);
                var oldData = m_data;
                m_data = newData;
                OnDataChanged?.BeginInvoke(this, oldData, value, IsDataChangeHooked, newData, null, null);
                Refresh();
            }
        }

        public async Task SetDataAsync(T value)
        {
            using (await m_dataLock.WriterLockAsync().ConfigureAwait(false))
            {
                ClearDataSourceWithoutLock();
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


        [SuppressMessage("ReSharper", "IdentifierTypo")]
        private void INPC_Update(object sender, PropertyChangedEventArgs args)
        {
            if (sender is not INotifyPropertyChanged inpc)
                throw new ArgumentException($"{nameof(sender)} cannot be null or not {nameof(INotifyPropertyChanged)}",
                    nameof(sender));
            if (!Equals(inpc, DataSource))
                throw new ArgumentException(
                    $"{nameof(sender)}:{sender} does not bind to this object",nameof(sender));
            if (args.PropertyName!=m_dataSourcePropertyName) return;
            using (m_dataLock.WriterLock())
            {
                var old = m_data;
                var newBeforeHook = GetValueFromSource(inpc, args.PropertyName);
                var newAfterHook = IsDataChangeHooked? DataChangedHook(this, newBeforeHook):newBeforeHook;
                if (!Equals(inpc, m_iNotifyPropertyChangedDataSource)) return;
                m_data = newAfterHook;
                OnDataChanged?.BeginInvoke(this, old, newBeforeHook, IsDataChangeHooked, newAfterHook,null,null);
                Refresh();
            }
            
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static T GetValueFromSource(INotifyPropertyChanged obj, string name)
        {
            if (obj is null) throw new ArgumentNullException(nameof(obj));
            if (name is null or "") throw new ArgumentNullException(nameof(name));
            var properties = obj.GetType().GetProperties();
            if (!properties.Select((info => info.Name)).Contains(name))
                throw new ArgumentException($"{obj.GetType()} does not contains the public property called {name}",
                    nameof(obj));
            // ReSharper disable once PossibleNullReferenceException
            var ans = obj.GetType().GetProperty(name).GetValue(obj);
            return ans switch
            {
                null when default(T) is null => default,
                null or not T => throw new ArgumentException($"{obj.GetType()} cannot be converted to {typeof(T)}",
                    nameof(obj)),
                _ => (T) ans
            };
        }

        

        private INotifyPropertyChanged m_iNotifyPropertyChangedDataSource = null;
        private string m_dataSourcePropertyName = null;

        public INotifyPropertyChanged GetDataSource()
        {
            using (m_dataLock.ReaderLock())
            {
                return m_iNotifyPropertyChangedDataSource;
            }
        }
        public void SetDataSource(INotifyPropertyChanged obj,string propertyName)
        {
            using (m_dataLock.WriterLock())
            {
                var ans = GetValueFromSource(obj, propertyName);
                var old = m_data;
                var ansAfterHook = IsDataChangeHooked ? DataChangedHook(this, ans) : ans;
                m_iNotifyPropertyChangedDataSource = obj;
                m_dataSourcePropertyName = propertyName;
                m_data = ansAfterHook;
                obj.PropertyChanged += INPC_Update;
                OnDataChanged?.BeginInvoke(this, old, ans, IsDataChangeHooked, ansAfterHook, null, null);
                Refresh();
            }
        }
        public async Task SetDataSourceAsync(INotifyPropertyChanged obj, string propertyName)
        {
            using (await m_dataLock.WriterLockAsync().ConfigureAwait(false))
            {
                var ans = await Task.Run((() => GetValueFromSource(obj, propertyName))).ConfigureAwait(false);
                var old = m_data;
                var ansAfterHook = await Task.Run((() => DataChangedHook(this, ans))).ConfigureAwait(false);
                m_iNotifyPropertyChangedDataSource = obj;
                m_dataSourcePropertyName = propertyName;
                m_data = ansAfterHook;
                obj.PropertyChanged += INPC_Update;
                OnDataChanged?.BeginInvoke(this, old, ans, IsDataChangeHooked, ansAfterHook, null, null);
                await RefreshAsync().ConfigureAwait(false);
            }
        }

        public void ClearDataSource()
        {
            using (m_dataLock.WriterLock())
            {
                ClearDataSourceWithoutLock();
            }
        }

        /// <summary>
        /// 必须在写锁中执行
        /// </summary>
        private void ClearDataSourceWithoutLock()
        {
            m_iNotifyPropertyChangedDataSource.PropertyChanged -= (PropertyChangedEventHandler)INPC_Update;
            m_iNotifyPropertyChangedDataSource = null;
            m_dataSourcePropertyName = null;
        }

        public INotifyPropertyChanged DataSource => GetDataSource();
        public bool IsDataFromINotifyPropertyChangedSource => DataSource is not null;
    }
}
