namespace SPEkit.CombinedSemaphore.MainClass
{
    public sealed partial class CombinedSemaphore
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