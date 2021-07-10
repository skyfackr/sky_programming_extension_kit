using System;
using System.Collections.Generic;
using System.Threading;
using SPEkit.CombinedSemaphore.Unit;

namespace SPEkit.CombinedSemaphore.MainClass
{
    /// <summary>
    /// 支持针对<see cref="Semaphore"/>和<see cref="SemaphoreSlim"/>通过与<see cref="SemaphoreSlim"/>同样的方法进行统一并行等待释放
    /// </summary>
    public sealed partial class CombinedSemaphore : IList<SemaphoreUnit>, IDisposable
    {
    }
}

namespace SPEkit.CombinedSemaphore.Unit
{
    /// <summary>
    /// 基本抽象类，可用于统一两种信号量的类声明
    /// </summary>
    public abstract partial class SemaphoreUnit : IEquatable<SemaphoreUnit>
    {
    }

    /// <summary>
    /// <see cref="Semaphore"/>的<see cref="SemaphoreUnit"/>实现
    /// </summary>
    public sealed partial class SemaphoreWin32Unit : SemaphoreUnit
    {
    }

    /// <summary>
    /// <see cref="SemaphoreSlim"/>的<see cref="SemaphoreUnit"/>实现
    /// </summary>
    public sealed partial class SemaphoreSlimUnit : SemaphoreUnit
    {
    }
}