using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace SPEkit.SemaphoreSlimAttribute
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public abstract partial class AbstractSlot
    {
        public static AbstractSlot TryGetAbstractSlotAttribute(MethodBase method)
        {
            return s_assignedMethod.GetValueOrDefault(method, null);
        }

        public static bool IsRegistered(MethodBase method)
        {
            return s_assignedMethod.ContainsKey(method);
        }

        public static AbstractSlot GetAbstractSlotAttribute(MethodBase method)
        {
            if (!IsRegistered(method))
                throw new MethodNotRegisteredException(method);
            return s_assignedMethod[method];
        }
    }

    public static class AbstractSlotEx
    {
        public static AbstractSlot GetAbstractSlotAttribute(this MethodBase method)
        {
            return AbstractSlot.GetAbstractSlotAttribute(method);
        }

        public static AbstractSlot TryGetAbstractSlotAttribute(this MethodBase method)
        {
            return AbstractSlot.TryGetAbstractSlotAttribute(method);
        }
    }
}