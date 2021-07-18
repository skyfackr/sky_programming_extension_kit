using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SPEkit.BinLikeClassSelectors;
using SPEkit.CombinedSemaphore.error;
using SPEkit.CombinedSemaphore.Unit;
using SPEkit.CombinedSemaphore.Utils;

namespace SPEkit.CombinedSemaphore.MainClass
{
    public sealed partial class CombinedSemaphore
    {
        /// <summary>
        ///     当一个<see cref="ReleaseRecoverySession" />全部执行完成时启动事件，传入一个<see cref="ReleaseRecoverySession" />和一个
        ///     <see cref="Exception" />
        ///     仅当还原时出错，<see cref="Exception" />才不为null
        /// </summary>
        [SuppressMessage("ReSharper", "EventNeverSubscribedTo.Global")]
        public event Action<ReleaseRecoverySession, Exception> AllRecoveryCompleteEvent;

        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        private ReleaseRecoverySession RecoveryAll(IEnumerable<ReleasingSessions> sessions)
        {
            var tks = new CancellationTokenSource();
            var units = from session in sessions
                select session.Unit;
            var recoverySession = new ReleaseRecoverySession(units, tks);
            Task.Run(() =>
            {
                Exception ex = null;
                try
                {
                    Parallel.ForEach(sessions, session => { session.Recovery(tks.Token); });
                }
                catch (Exception e)
                {
                    ex = e;
                }
                finally
                {
                    recoverySession.IsRecoveryCompleted = true;
                    AllRecoveryCompleteEvent?.Invoke(recoverySession, ex);
                }
            }, CancellationToken.None);
            return recoverySession;
        }

        private void ReleaseFailedHelper(AggregateException ex, IEnumerable<ReleasingSessions> sessions)
        {
            var option = m_option.IgnoreConflictFlag().CreateBinLikeClassSelectorUnit();
            var exs = ex.InnerExceptions as IEnumerable<Exception>;
            if (option.Match((long) WaitActionFlag.IgnoreDisposed))
                exs = from oneEx in exs
                    where oneEx.GetType() != typeof(ObjectDisposedException)
                    select oneEx;
            if (option.Match((long) WaitActionFlag.ContinueAndIgnoreWhenReleaseExceeded))
                exs = from oneEx in exs
                    where oneEx.GetType() != typeof(SemaphoreFullException)
                    select oneEx;
            if (!exs.Any()) return;
            sessions = from session in sessions
                where session != null
                select session;
            //if (!sessions.Any()) return;
            throw new ReleaseFailedException(ex, RecoveryAll(sessions));
        }

        [SuppressMessage("ReSharper", "NotAccessedVariable")]
        private int[] ReleaseProcess(Func<SemaphoreUnit, int> act)
        {
            AssertNotDisposed();
            var units = m_units.ToList();
            var sessions = new ReleasingSessions[units.Count];
            var returns = new int[units.Count];
            var options = m_option.IgnoreConflictFlag().CreateBinLikeClassSelectorUnit();
            ParallelLoopResult? result = null;
            try
            {
                result = Parallel.ForEach(units, (unit, state, index) =>
                {
                    var session = new ReleasingSessions(unit);
                    sessions[index] = session;
                    try
                    {
                        returns[index] = act(unit);
                        session.Released(true);
                    }
                    catch (ObjectDisposedException)
                    {
                        if (options.Match((long) WaitActionFlag.ThrowWhenDisposed))
                        {
                            state.Stop();
                            throw;
                        }
                    }
                    catch (SemaphoreFullException)
                    {
                        if (options.Match((long) WaitActionFlag.RecoveryAndThrowWhenReleaseExceeded))
                        {
                            state.Stop();
                            throw;
                        }
                    }
                    catch (Exception)
                    {
                        state.Stop();
                        throw;
                    }
                });
            }
            catch (AggregateException e)
            {
                ReleaseFailedHelper(e, sessions);
            }

            return returns;
        }
    }
}