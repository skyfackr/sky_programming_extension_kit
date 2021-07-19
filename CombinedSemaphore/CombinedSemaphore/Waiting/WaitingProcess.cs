using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using SPEkit.BinLikeClassSelectors;
using SPEkit.CombinedSemaphore.Unit;
using SPEkit.CombinedSemaphore.Utils;

namespace SPEkit.CombinedSemaphore.MainClass
{
    public sealed partial class CombinedSemaphore
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void RecoveryAll(IEnumerable<WaitingSessions> sessions)
        {
            sessions = from session in sessions
                where session != null
                select session;
            foreach (var session in sessions) session.Recovery();
        }

        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        [SuppressMessage("ReSharper", "MethodHasAsyncOverload")]
        private bool WaitingProcess(Func<SemaphoreUnit, Task<bool>> act)
        {
            AssertNotDisposed();
            var option = m_option.IgnoreConflictFlag();
            var units = m_units.ToList();
            var sessions = new WaitingSessions[units.Count];
            ParallelLoopResult? result = null;
            IEnumerable<bool> sessionResults;
            try
            {
                result = Parallel.ForEach(units, async (unit, state, index) =>
                {
                    var session = new WaitingSessions(unit);
                    sessions[index] = session;
                    try
                    {
                        session.Entered(await act(unit).ConfigureAwait(false));
                    }
                    catch (ObjectDisposedException)
                    {
                        if (!option.CreateBinLikeClassSelectorUnit()
                            .Match((long) WaitActionFlag.IgnoreDisposed))
                        {
                            state.Stop();
                            throw;
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        state.Stop();
                        throw;
                    }
                    catch (Exception)
                    {
                        state.Stop();
                        throw;
                    }

                    if (!session.IsEntered) state.Stop();
                });
            }
            catch (AggregateException e)
            {
                var countMaker = from ex in e.InnerExceptions
                    group ex by ex.GetType();
                //var a=e.InnerExceptions.Distinct()
                var count = countMaker as IGrouping<Type, Exception>[] ?? countMaker.ToArray();
                if (count.Length > 1) throw;
                var onlyEx = count.First().Key;
                if (onlyEx == typeof(ObjectDisposedException))
                    throw new ObjectDisposedException("Some semaphore disposed.");

                if (onlyEx == typeof(OperationCanceledException))
                    throw new OperationCanceledException("Some waiting cancelled");

                throw;
            }
            finally
            {
                sessionResults = from session in sessions
                    select session.IsEntered;
                if (result == null)
                {
                    RecoveryAll(sessions);
                }
                else
                {
                    if (!result.Value.IsCompleted || sessionResults.Contains(false)) RecoveryAll(sessions);
                }
            }

            return result.Value.IsCompleted && !sessionResults.Contains(false);
        }
    }
}