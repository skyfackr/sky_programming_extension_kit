using System;
using System.Collections.Generic;
using SPEkit.CombinedSemaphore.Unit;

namespace SPEkit.CombinedSemaphore.MainClass
{
    public sealed partial class CombinedSemaphore : IList<SemaphoreUnit>, IDisposable
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