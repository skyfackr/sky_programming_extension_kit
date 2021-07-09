using System;
using System.Threading;
using System.Threading.Tasks;

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
                    await Task.Delay(waitTime, token);
                    if (token.IsCancellationRequested) return;
                }
            });
        }

        internal void Stop()
        {
            if (m_tks.IsCancellationRequested) return;
            m_tks.Cancel();
        }

        ~CleanerCirculation()
        {
            Stop();
        }
    }
}