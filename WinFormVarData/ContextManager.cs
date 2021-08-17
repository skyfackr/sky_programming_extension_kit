using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Nito.AsyncEx;

namespace SPEkit.WinFormVarData
{
    public sealed partial class DataBinder<T>
    {
        private SynchronizationContext m_context = null;
        private Thread m_thread = null;
        private AsyncReaderWriterLock m_contextAndThread_lock = new();

        public SynchronizationContext CurrentContext
        {
            get
            {
                using (m_contextAndThread_lock.ReaderLock())
                {
                    return m_context;
                }
            }
        }

        public Thread CurrentThread
        {
            get
            {
                using (m_contextAndThread_lock.ReaderLock())
                {
                    return m_thread;
                }
            }
        }

        public void SetUIContext(Control ui)
        {
            using (m_contextAndThread_lock.WriterLock())
            {
                SynchronizationContext context;
                Thread thread;

                static (SynchronizationContext, Thread) Del() => (SynchronizationContext.Current, Thread.CurrentThread);

                (context, thread) = ((SynchronizationContext, Thread))ui.Invoke((Func<(SynchronizationContext, Thread)>) Del);
                m_context = context;
                m_thread = thread;
            }
        }

        public bool IsContextBinded => CurrentContext is not null;
    }
}
