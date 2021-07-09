using SPEkit.CombinedSemaphore.Utils;

namespace SPEkit.CombinedSemaphore.MainClass
{
    public sealed partial class CombinedSemaphore
    {
        private volatile WaitActionFlag m_option;

        public WaitActionFlag Option
        {
            get => m_option;
            set => m_option = value;
        }
    }
}