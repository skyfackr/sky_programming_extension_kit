namespace SPEkit.SemaphoreSlimAttribute
{
    internal sealed class SessionsStatus
    {
        internal readonly bool IsEntered;

        internal SessionsStatus(bool isEntered)
        {
            IsEntered = isEntered;
        }
    }
}