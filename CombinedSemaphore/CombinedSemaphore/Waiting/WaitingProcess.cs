using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Nito.AsyncEx.Synchronous;
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
            //foreach (var session in sessions) session.Recovery();
            sessions.AsParallel().ForAll(session => session.Recovery());
        }

        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        [SuppressMessage("ReSharper", "MethodHasAsyncOverload")]
        [SuppressMessage("ReSharper", "PossibleIntendedRethrow")]
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
                var exceptions = new ConcurrentBag<Exception>();
                result = Parallel.ForEach(units, (unit, state, index) =>
                {
                    var session = new WaitingSessions(unit);
                    sessions[index] = session;
                    try
                    {
                        session.Entered(act(unit).WaitAndUnwrapException());
                    }
                    catch (ObjectDisposedException e)
                    {
                        if (!option.CreateBinLikeClassSelectorUnit()
                            .Match((long) WaitActionFlag.IgnoreDisposed))
                        {
                            state.Stop();
                            exceptions.Add(e);
                            //throw;
                        }
                    }
                    catch (OperationCanceledException e)
                    {
                        state.Stop();
                        exceptions.Add(e);
                        //throw;
                    }
                    catch (Exception e)
                    {
                        state.Stop();
                        exceptions.Add(e);
                        //throw;
                    }

                    if (!session.IsEntered) state.Stop();
                });

                if (!exceptions.IsEmpty) throw new AggregateException(exceptions);
            }
            catch (AggregateException e)
            {
                e = e.Flatten();
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

                throw e;
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