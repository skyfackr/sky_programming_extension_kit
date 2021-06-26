using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace SPEkit.SemaphoreSlimAttribute
{
    public sealed class NotInitializedException : Exception
    {
        internal NotInitializedException(object obj) : base($"Class {obj} not initialized.")
        {
        }
    }

    public sealed class AmbiguityAssignedException : Exception
    {
        internal AmbiguityAssignedException(MethodBase method) : base(
            $"method {method} is already assigned one attribute inherited from {nameof(AbstractSlot)}")
        {
        }
    }

    public sealed class MethodNotRegisteredException : Exception
    {
        internal MethodNotRegisteredException(MethodBase method)
            : base($"method {method} have not registered any attribute inherited from {nameof(AbstractSlot)}")
        {
        }
    }

    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class WaitCancelledOrFailedException : Exception
    {
        internal WaitCancelledOrFailedException(MethodBase method, CancelFlag reason, Exception inner = null)
            : base($"Semaphore waiting of method {method} interrupted because:{Enum.GetName(reason)}", inner)
        {
            Reasons = reason;
        }

        public CancelFlag Reasons { get; }
    }
}