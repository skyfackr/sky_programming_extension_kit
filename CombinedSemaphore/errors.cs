using System;

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
}