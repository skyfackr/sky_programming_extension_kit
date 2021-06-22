using System;
using PostSharp.Aspects;
using PostSharp.Aspects.Configuration;
using PostSharp.Aspects.Serialization;

namespace SPEkit.SemaphoreSlimAttribute
{
    [OnMethodBoundaryAspectConfiguration(SerializerType = typeof(MsilAspectSerializer))]
    [AttributeUsage(
        AttributeTargets.Constructor | AttributeTargets.Delegate | AttributeTargets.Event | AttributeTargets.Field |
        AttributeTargets.Method | AttributeTargets.Module | AttributeTargets.Property, Inherited = false)]
    public sealed class SlotWaitAttribute : AbstractSlot
    {
    }

    [OnMethodBoundaryAspectConfiguration(SerializerType = typeof(MsilAspectSerializer))]
    [AttributeUsage(
        AttributeTargets.Constructor | AttributeTargets.Delegate | AttributeTargets.Event | AttributeTargets.Field |
        AttributeTargets.Method | AttributeTargets.Module | AttributeTargets.Property, Inherited = false)]
    public sealed class SlotWaitAsyncAttribute : AbstractSlot
    {
    }

    public abstract partial class AbstractSlot : OnMethodBoundaryAspect
    {
    }
}