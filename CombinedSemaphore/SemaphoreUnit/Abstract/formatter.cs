using System.Runtime.CompilerServices;

namespace SPEkit.CombinedSemaphore.Unit
{
    public abstract partial class SemaphoreUnit
    {
        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
        {
            return $"CurrentType:{GetCurrentSemaphore().GetType()}";
        }
    }
}