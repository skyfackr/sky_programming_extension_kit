using System;

namespace SPEkit.SemaphoreSlimAttribute
{
    [Flags]
    public enum CancelFlag
    {
        CancelledByToken,
        Timeout,
        Unknown,
        MaxCountExceeded,
        InternalError
    }
}