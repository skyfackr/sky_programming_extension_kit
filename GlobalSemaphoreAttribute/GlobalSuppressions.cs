// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly:
    SuppressMessage("Usage", "CA2219:不要在 finally 子句中引发异常", Justification = "<挂起>", Scope = "member",
        Target = "~M:SPEkit.SemaphoreSlimAttribute.AbstractSlot.OnInvoke(PostSharp.Aspects.MethodInterceptionArgs)")]
[assembly:
    SuppressMessage("CodeQuality", "IDE0079:请删除不必要的忽略", Justification = "<挂起>", Scope = "member",
        Target =
            "~M:SPEkit.SemaphoreSlimAttribute.SlotWaitAsyncAttribute.TryEntry~System.Threading.Tasks.Task{System.Boolean}")]
[assembly:
    SuppressMessage("ReSharper", "PossibleInvalidOperationException", Justification = "<挂起>", Scope = "member",
        Target =
            "~M:SPEkit.SemaphoreSlimAttribute.SlotWaitAsyncAttribute.TryEntry~System.Threading.Tasks.Task{System.Boolean}")]
[assembly:
    SuppressMessage("Usage", "CA2219:不要在 finally 子句中引发异常", Justification = "<挂起>", Scope = "member",
        Target =
            "~M:SPEkit.SemaphoreSlimAttribute.AbstractSlot.OnInvokeAsync(PostSharp.Aspects.MethodInterceptionArgs)~System.Threading.Tasks.Task")]