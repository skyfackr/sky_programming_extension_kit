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
}