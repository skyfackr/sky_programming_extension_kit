using System.Diagnostics.CodeAnalysis;

namespace SPEkit.SemaphoreSlimAttribute
{
    [SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
    public abstract partial class AbstractSlot
    {
        /// <summary>
        ///     获取当前的等待时配置
        /// </summary>
        public WaitingOption Option { get; }
    }
}