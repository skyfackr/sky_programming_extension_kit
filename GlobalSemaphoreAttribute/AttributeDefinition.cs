using System;
using PostSharp.Aspects;
using PostSharp.Aspects.Configuration;
using PostSharp.Aspects.Serialization;

namespace SPEkit.SemaphoreSlimAttribute
{
    //[AttributeUsage(
    //    AttributeTargets.Constructor | AttributeTargets.Delegate | AttributeTargets.Event | AttributeTargets.Field |
    //    AttributeTargets.Method | AttributeTargets.Module | AttributeTargets.Property, Inherited = false)]
    [OnMethodBoundaryAspectConfiguration(SerializerType = typeof(MsilAspectSerializer))]
    public sealed partial class SlotWaitAttribute : AbstractSlot
    {
    }


    //[AttributeUsage(
    //    AttributeTargets.Constructor | AttributeTargets.Delegate | AttributeTargets.Event | AttributeTargets.Field |
    //    AttributeTargets.Method | AttributeTargets.Module | AttributeTargets.Property, Inherited = false)]
    [OnMethodBoundaryAspectConfiguration(SerializerType = typeof(MsilAspectSerializer))]
    public sealed partial class SlotWaitAsyncAttribute : AbstractSlot
    {
    }

    [AttributeUsage(
        AttributeTargets.Constructor | AttributeTargets.Delegate | AttributeTargets.Event | AttributeTargets.Field |
        AttributeTargets.Method | AttributeTargets.Module | AttributeTargets.Property)]
    public abstract partial class AbstractSlot : OnMethodBoundaryAspect
    {
    }
}