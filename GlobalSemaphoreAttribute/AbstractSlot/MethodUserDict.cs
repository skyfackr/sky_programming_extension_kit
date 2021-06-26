using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SPEkit.SemaphoreSlimAttribute
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public abstract partial class AbstractSlot
    {
        private static readonly Dictionary<MethodBase, AbstractSlot> s_assignedMethod = new();
        private MethodBase m_method;
        private bool m_postSharpInit;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected MethodBase GetAssignedMethodInternal()
        {
            return m_method;
        }

        public virtual MethodBase GetAssignedMethod()
        {
            return GetAssignedMethodInternal();
        }

        protected virtual void CustomRuntimeInitialize(MethodBase method)
        {
        }

        private void RegisterMe(MethodBase method)
        {
            if (!s_assignedMethod.TryAdd(method, this))
                throw new AmbiguityAssignedException(method);
            m_method = method;
        }

        /// <inheritdoc />
        public sealed override void RuntimeInitialize(MethodBase method)
        {
            base.RuntimeInitialize(method);
            CustomRuntimeInitialize(method);
            RegisterMe(method);
            m_postSharpInit = true;
        }
    }
}