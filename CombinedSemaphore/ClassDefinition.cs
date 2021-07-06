namespace SPEkit.CombinedSemaphore
{
    public sealed class CombinedSemaphore
    {
    }
}

namespace SPEkit.CombinedSemaphore.Unit
{
    public abstract partial class SemaphoreUnit
    {
    }

    public sealed partial class SemaphoreWin32Unit : SemaphoreUnit
    {
    }

    public sealed partial class SemaphoreSlimUnit : SemaphoreUnit
    {
    }
}