namespace SPEkit.SemaphoreSlimAttribute
{
    public abstract partial class AbstractSlot
    {
        /// <inheritdoc />
        public override string ToString()
        {
            if (!IsInitialized()) return  $"{base.ToString()}(hashcode:{GetHashCode()}) Not Initialized.";
            return $"{nameof(CurrentCount)}:{CurrentCount},CurrentMethod:{m_method}";
        }
    }
}