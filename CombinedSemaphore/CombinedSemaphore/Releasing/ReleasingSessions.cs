using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SPEkit.CombinedSemaphore.Unit;

namespace SPEkit.CombinedSemaphore.Utils
{
    internal class ReleasingSessions
    {
        internal readonly SemaphoreUnit Unit;

        internal ReleasingSessions(SemaphoreUnit unit)
        {
            Unit = unit;
        }

        internal bool IsReleased { get; private set; }

        internal void Released(bool isReleased)
        {
            IsReleased = isReleased;
        }

        internal void Recovery(CancellationToken token)
        {
            if (!IsReleased) return;
            try
            {
                Unit.Wait(token);
            }
            catch (OperationCanceledException)
            {
            }
            catch (ObjectDisposedException)
            {
            }
        }
    }

    /// <summary>
    ///     如果释放时出现未被忽略的错误则会启动全部还原操作，此类的实例可以用于一次还原会话的查询与取消
    /// </summary>
    public class ReleaseRecoverySession
    {
        private readonly ManualResetEvent m_completeEvent = new(false);
        private readonly CancellationTokenSource m_tks;

        /// <summary>
        ///     本次会话操作的全部<see cref="SemaphoreUnit" />对象
        /// </summary>
        public readonly SemaphoreUnit[] Units;

        private bool m_isRecoveryCompleted;

        internal ReleaseRecoverySession(IEnumerable<SemaphoreUnit> units, CancellationTokenSource tks)
        {
            Units = units.ToArray();
            m_tks = tks;
        }

        /// <summary>
        ///     表明是否还原完毕
        /// </summary>
        public bool IsRecoveryCompleted
        {
            get => m_isRecoveryCompleted;
            internal set
            {
                if (value) m_completeEvent.Set();
                else m_completeEvent.Reset();
                m_isRecoveryCompleted = value;
            }
        }

        /// <summary>
        ///     表明还原是否被取消
        /// </summary>
        public bool IsRecoveryCancelled => m_tks.IsCancellationRequested;

        /// <summary>
        ///     取消本次还原会话
        /// </summary>
        /// <remarks>
        ///     注意，此函数会对所有还原操作的等待信号方法发送取消通知，无法预知哪些被取消哪些运行成功，使用后可能导致不可预知的状态
        ///     取消完成后<see cref="MainClass.CombinedSemaphore.AllRecoveryCompleteEvent" />将被立刻执行
        /// </remarks>
        public void Cancel()
        {
            m_tks.Cancel();
        }

        /// <summary>
        ///     等待完成
        /// </summary>
        /// <returns></returns>
        public Task WaitAsync()
        {
            return Task.Run(m_completeEvent.WaitOne);
        }
    }
}