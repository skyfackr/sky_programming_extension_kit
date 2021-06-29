using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace SPEkit.SemaphoreSlimAttribute
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public abstract partial class AbstractSlot
    {
        /// <summary>
        ///     尝试查找<paramref name="method" />是否存在<see cref="AbstractSlot" />衍生特性并返回，如无，返回null
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        /// <remarks>以动态形式添加相关特性可能导致无法注册及生效</remarks>
        public static AbstractSlot TryGetAbstractSlotAttribute(MethodBase method)
        {
            return s_assignedMethod.GetValueOrDefault(method, null);
        }

        /// <summary>
        ///     检查<paramref name="method" />是否有注册<see cref="AbstractSlot" />衍生特性
        ///     <para>默认检查特性是否定义的相关函数在此特性上将不可用</para>
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        /// <remarks>以动态形式添加相关特性可能导致无法注册及生效</remarks>
        public static bool IsRegistered(MethodBase method)
        {
            return s_assignedMethod.ContainsKey(method);
        }

        /// <summary>
        ///     获取<paramref name="method" />所定义的<see cref="AbstractSlot" />衍生特性
        /// </summary>
        /// <param name="method"></param>
        /// <exception cref="MethodNotRegisteredException"></exception>
        /// <returns></returns>
        /// <remarks>以动态形式添加相关特性可能导致无法注册及生效</remarks>
        public static AbstractSlot GetAbstractSlotAttribute(MethodBase method)
        {
            if (!IsRegistered(method))
                throw new MethodNotRegisteredException(method);
            return s_assignedMethod[method];
        }
    }

    /// <summary>
    ///     用于向<see cref="MethodBase" />注入扩展方法
    /// </summary>
    public static class AbstractSlotEx
    {
        /// <inheritdoc cref="AbstractSlot.GetAbstractSlotAttribute" />
        public static AbstractSlot GetAbstractSlotAttribute(this MethodBase method)
        {
            return AbstractSlot.GetAbstractSlotAttribute(method);
        }

        /// <inheritdoc cref="AbstractSlot.TryGetAbstractSlotAttribute" />
        public static AbstractSlot TryGetAbstractSlotAttribute(this MethodBase method)
        {
            return AbstractSlot.TryGetAbstractSlotAttribute(method);
        }
    }
}