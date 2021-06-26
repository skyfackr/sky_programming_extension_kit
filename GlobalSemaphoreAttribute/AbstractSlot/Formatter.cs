namespace SPEkit.SemaphoreSlimAttribute
{
    public abstract partial class AbstractSlot
    {
        public override string ToString()
        {
            if (!IsInitialized()) return base.ToString() + " Not Initialized.";
            return $"{nameof(CurrentSemaphore)}: {CurrentSemaphore},{nameof(m_method)}:{m_method}";
        }
    }
}