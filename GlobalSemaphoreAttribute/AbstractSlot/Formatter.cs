namespace SPEkit.SemaphoreSlimAttribute
{
    public abstract partial class AbstractSlot
    {
        public override string ToString()
        {
            if (!m_Initialized) return base.ToString() + " Not Initialized.";
            return $"{nameof(CurrentSemaphore)}: {CurrentSemaphore}";
        }
    }
}