using System;
using SPEkit.CombinedSemaphore.Utils;

namespace SPEkit.CombinedSemaphore.error
{
    public sealed class TypeCannotConvertException : Exception
    {
        internal TypeCannotConvertException(Type type)
            : base($"This object cannot output as {type}")
        {
        }
    }

    public sealed class TypeNotSupportedException : Exception
    {
        internal TypeNotSupportedException(Type type)
            : base($"Type {type} is not supported.")
        {
        }
    }

    public sealed class ReleaseFailedException : AggregateException
    {
        public readonly ReleaseRecoverySession RecoverySession;


        internal ReleaseFailedException(AggregateException ex, ReleaseRecoverySession session) : base(ex.Message,
            ex.InnerExceptions)
        {
            RecoverySession = session;
        }
    }
}