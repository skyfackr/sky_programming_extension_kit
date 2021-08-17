using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nito.AsyncEx;
using PostSharp.Aspects.Advices;

namespace SPEkit.WinFormVarData
{
    [SuppressMessage("ReSharper", "EventNeverSubscribedTo.Global")]
    public sealed partial class DataBinder<T>
    {
        private Func<DataBinder<T>, T, T> m_dataChangeHook = null;
        private readonly AsyncReaderWriterLock m_dataChangeHookLock = new();
        public Func<DataBinder<T>, T, T> DataChangedHook
        {
            get
            {
                using (m_dataChangeHookLock.ReaderLock())
                {
                    return m_dataChangeHook;
                }
            }
            set
            {
                using (m_dataChangeHookLock.WriterLock())
                {
                    OnHookChanged?.BeginInvoke(this,m_dataChangeHook,value,null,null);
                    m_dataChangeHook = value;
                }
            }
        }  //传入自身与尝试改数字，输出实际改数字
        public event Action<DataBinder<T>, T, T, bool, T> OnDataChanged;//自身 原数据 新数据（未hook） 是否hook 新数据（hook后）
        public event Action<DataBinder<T>, Func<DataBinder<T>, T, T>, Func<DataBinder<T>, T, T>> OnHookChanged;//自身 原hook 新hook
        public bool IsDataChangeHooked => DataChangedHook is not null;
    }
}
