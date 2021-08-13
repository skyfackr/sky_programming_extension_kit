using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx.Synchronous;

namespace SPEkit.CombinedSemaphore.Utils
{
    internal class CleanerCirculation
    {
        private readonly Task m_task;
        private readonly CancellationTokenSource m_tks = new();

        internal CleanerCirculation(Action<CancellationToken> cleaner, TimeSpan waitTime)
        {
            m_task = Task.Run(async () =>
            {
                var token = m_tks.Token;
                while (true)
                {
                    cleaner(token);
                    //todo delete
                    Trace.WriteLine("cleaner ok");
                    try
                    {
                        await Task.Delay(waitTime, token).ConfigureAwait(false);
                    }
                    catch (TaskCanceledException )
                    {
                        break;
                    }
                    if (token.IsCancellationRequested) break;
                }
            }, m_tks.Token);
        }

        internal void Stop(bool wait = true)
        {
            if (m_tks.IsCancellationRequested) return;
            m_tks.Cancel();
            //todo delete
            Trace.WriteLine("cancelled");
            if (wait) m_task.WaitAndUnwrapException();
            //todo delete
            Trace.WriteLine("waited");
            //m_task.Wait()
        }

        ~CleanerCirculation()
        {
            Stop(false);
        }
    }
}