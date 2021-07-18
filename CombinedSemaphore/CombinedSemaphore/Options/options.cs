using SPEkit.CombinedSemaphore.Utils;

namespace SPEkit.CombinedSemaphore.MainClass
{
    public sealed partial class CombinedSemaphore
    {
        private volatile WaitActionFlag m_option;

        /// <summary>
        ///     当前等待设定
        /// </summary>
        public WaitActionFlag Option
        {
            get => m_option;
            set => m_option = value;
        }
    }
}