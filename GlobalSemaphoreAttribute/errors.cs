using System;

namespace SPEkit.SemaphoreSlimAttribute
{
    public sealed class NotInitializedException : Exception
    {
        internal NotInitializedException(object obj) : base($"Class {obj} not initialized.")
        {
        }
    }
}