using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPEkit.WinFormVarData
{
    public sealed partial class DataBinder<T>
    {
        private Func<DataBinder<T>, T, T> m_dataChangeHook = null;

        public Func<DataBinder<T>, T, T> DataChangedHook
        {
            get => m_dataChangeHook;
            set
            {
                OnHookChanged.
            }
        }  //传入自身与尝试改数字，输出实际改数字
        public event Action<DataBinder<T>, T, T, bool, T> OnDataChanged;
        public event Action<DataBinder<T>, Func<DataBinder<T>, T, T>, Func<DataBinder<T>, T, T>> OnHookChanged;
    }
}
