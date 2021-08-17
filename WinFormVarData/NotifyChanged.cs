using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace SPEkit.WinFormVarData
{
    public sealed partial class DataBinder<T>
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void Refresh()
        {
            if (!IsContextBinded)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(DataPropertyName));
                return;
            }
            CurrentContext.Send((SendOrPostCallback)InvokePropertyChanged,null);
        }

        public async Task RefreshAsync()
        {
            if (!IsContextBinded)
            {
                await Task.Run((() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(DataPropertyName))))
                    .ConfigureAwait(false);
                return;
            }

            await CurrentContext.PostAsync((() => InvokePropertyChanged(null))).ConfigureAwait(false);
        }

        private void InvokePropertyChanged(object state)
        {
            PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(DataPropertyName));
        }

        public void ClearAllNotify()
        {
            PropertyChanged = null;
        }
    }
}
