using System;
using System.Threading;
using PostSharp.Aspects;
using PostSharp.Aspects.Configuration;
using PostSharp.Aspects.Serialization;

namespace SPEkit.SemaphoreSlimAttribute
{
    //[AttributeUsage(
    //    AttributeTargets.Constructor | AttributeTargets.Delegate | AttributeTargets.Event | AttributeTargets.Field |
    //    AttributeTargets.Method | AttributeTargets.Module | AttributeTargets.Property, Inherited = false)]
    /// <summary>
    ///     使用此特性可以使函数进入时在指定<see cref="SemaphoreSlim" />上进行同步等待
    /// </summary>
    [OnMethodBoundaryAspectConfiguration(SerializerType = typeof(MsilAspectSerializer))]
    public sealed partial class SlotWaitAttribute : AbstractSlot
    {
    }


    //[AttributeUsage(
    //    AttributeTargets.Constructor | AttributeTargets.Delegate | AttributeTargets.Event | AttributeTargets.Field |
    //    AttributeTargets.Method | AttributeTargets.Module | AttributeTargets.Property, Inherited = false)]
    /// <summary>
    ///     使用此特性可以使函数进入时在指定<see cref="SemaphoreSlim" />上进行异步等待
    /// </summary>
    [OnMethodBoundaryAspectConfiguration(SerializerType = typeof(MsilAspectSerializer))]
    public sealed partial class SlotWaitAsyncAttribute : AbstractSlot
    {
    }

    /// <summary>
    ///     定义函数AOP式使用<see cref="SemaphoreSlim" />的基本代码
    /// </summary>
    /// <remarks>如继承此类实现自定义等待代码，可能需要<see cref="PostSharp" />库的许可证，更多问题、使用权请咨询相关公司</remarks>
    [AttributeUsage(
        AttributeTargets.Constructor | AttributeTargets.Delegate | AttributeTargets.Event | AttributeTargets.Field |
        AttributeTargets.Method | AttributeTargets.Module | AttributeTargets.Property)]
    public abstract partial class AbstractSlot : OnMethodBoundaryAspect
    {
    }
}