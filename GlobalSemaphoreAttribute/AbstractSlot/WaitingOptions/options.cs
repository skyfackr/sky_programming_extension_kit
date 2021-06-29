using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Nito.AsyncEx;

namespace SPEkit.SemaphoreSlimAttribute
{
    /// <summary>
    ///     当<see cref="AbstractSlot" />对象执行等待时的退出动作设定
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class WaitingOption
    {
        private readonly AsyncReaderWriterLock m_tokenLock = new();
        private readonly AsyncReaderWriterLock m_waitTimeLock = new();
        private CancellationToken? m_token;
        private TimeSpan? m_waitingTimePerWait;

        /// <summary>
        ///     指定等待时传入的<see cref="CancellationToken" />，如为null则忽略
        /// </summary>
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

        /// <summary>
        ///     指定等待的超时时间，如为null则为无限
        /// </summary>
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

        /// <summary>
        ///     设置<see cref="Token" />
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public WaitingOption SetCancellationToken(CancellationToken? token)
        {
            Token = token;
            return this;
        }

        /// <summary>
        ///     设置<see cref="WaitingTimePerWait" />
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public WaitingOption SetTimeOut(TimeSpan? time)
        {
            WaitingTimePerWait = time;
            return this;
        }

        /// <summary>
        ///     创建实例
        /// </summary>
        /// <param name="token"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static WaitingOption Create(CancellationToken? token = null, TimeSpan? time = null)
        {
            var ans = new WaitingOption {Token = token, WaitingTimePerWait = time};
            return ans;
        }

        /// <summary>
        ///     指定超时
        /// </summary>
        /// <param name="ms"></param>
        /// <returns></returns>
        public WaitingOption SetTimeOut(int ms)
        {
            WaitingTimePerWait = TimeSpan.FromMilliseconds(ms);
            return this;
        }

        /// <summary>
        ///     指定<see cref="CancellationToken" />并返回实例
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static WaitingOption FromCancellationToken(CancellationToken token)
        {
            return Create().SetCancellationToken(token);
        }

        /// <summary>
        ///     指定超时毫秒数并返回实例
        /// </summary>
        /// <param name="ms"></param>
        /// <returns></returns>
        public static WaitingOption FromMS(int ms)
        {
            return Create().SetTimeOut(ms);
        }

        /// <summary>
        ///     指定超时<see cref="TimeSpan" />并返回实例
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
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