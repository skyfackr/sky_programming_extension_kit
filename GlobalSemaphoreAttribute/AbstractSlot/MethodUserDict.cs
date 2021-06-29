using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using PostSharp.Aspects;

namespace SPEkit.SemaphoreSlimAttribute
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public abstract partial class AbstractSlot
    {
        private static readonly Dictionary<MethodBase, AbstractSlot> s_assignedMethod = new();
        private MethodBase m_method;
        private bool m_postSharpInit;

        /// <summary>
        ///     内部方法，可以直接获取此对象所注册函数
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected MethodBase GetAssignedMethodInternal()
        {
            return m_method;
        }

        /// <summary>
        ///     获取此对象所注册函数
        /// </summary>
        /// <returns></returns>
        /// <remarks>在派生类中，可以通过重写实现自定义功能</remarks>
        public virtual MethodBase GetAssignedMethod()
        {
            return GetAssignedMethodInternal();
        }

        /// <summary>
        ///     在派生类中，通过重写此函数以实现自定义初始化代码。此代码将在<see cref="OnMethodBoundaryAspect" />的标准初始化过程中被<see cref="RuntimeInitialize" />执行
        /// </summary>
        /// <param name="method"></param>
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