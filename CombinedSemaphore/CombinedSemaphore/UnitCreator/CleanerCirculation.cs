using System;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx.Synchronous;

namespace SPEkit.CombinedSemaphore.Utils
{
    internal class CleanerCirculation
    {
        private readonly Task m_task;
        private readonly CancellationTokenSource m_tks = new();

        internal CleanerCirculation(Action cleaner, TimeSpan waitTime)
        {
            m_task = Task.Run(async () =>
            {
                var token = m_tks.Token;
                while (true)
                {
                    cleaner();
                    await Task.Delay(waitTime, token).ConfigureAwait(false);
                    if (token.IsCancellationRequested) return;
                }
            },m_tks.Token);
        }

        internal void Stop(bool wait=true)
        {
            if (m_tks.IsCancellationRequested) return;
            m_tks.Cancel();
            if (wait) m_task.WaitAndUnwrapException();
            //m_task.Wait()
        }

        ~CleanerCirculation()
        {
            Stop(false);
        }
    }
}