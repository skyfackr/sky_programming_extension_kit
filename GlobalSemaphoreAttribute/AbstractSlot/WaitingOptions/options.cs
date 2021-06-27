using System;
using System.Threading;
using Nito.AsyncEx;

namespace SPEkit.SemaphoreSlimAttribute
{
    public sealed class WaitingOption
    {
        private readonly AsyncReaderWriterLock m_tokenLock = new();
        private readonly AsyncReaderWriterLock m_waitTimeLock = new();
        private CancellationToken? m_token;
        private TimeSpan? m_waitingTimePerWait;

        public CancellationToken? Token
        {
            get
            {
                using (m_tokenLock.ReaderLock())
                {
                    return m_token;
                }
            }
            set
            {
                using (m_tokenLock.WriterLock())
                {
                    m_token = value;
                }
            }
        }

        public TimeSpan? WaitingTimePerWait
        {
            get
            {
                using (m_waitTimeLock.ReaderLock())
                {
                    return m_waitingTimePerWait;
                }
            }
            set
            {
                using (m_waitTimeLock.WriterLock())
                {
                    m_waitingTimePerWait = value;
                }
            }
        }

        public WaitingOption SetCancellationToken(CancellationToken? token)
        {
            Token = token;
            return this;
        }

        public WaitingOption SetTimeOut(TimeSpan? time)
        {
            WaitingTimePerWait = time;
            return this;
        }

        public static WaitingOption Create(CancellationToken? token = null, TimeSpan? time = null)
        {
            var ans = new WaitingOption {Token = token, WaitingTimePerWait = time};
            return ans;
        }

        public WaitingOption SetTimeOut(int ms)
        {
            WaitingTimePerWait = TimeSpan.FromMilliseconds(ms);
            return this;
        }

        public static WaitingOption FromCancellationToken(CancellationToken token)
        {
            return Create().SetCancellationToken(token);
        }

        public static WaitingOption FromMS(int ms)
        {
            return Create().SetTimeOut(ms);
        }

        public static WaitingOption FromTimeSpan(TimeSpan time)
        {
            return Create().SetTimeOut(time);
        }

        internal WaitingOption Clone()
        {
            return new() {Token = Token, WaitingTimePerWait = WaitingTimePerWait};
        }
    }
}