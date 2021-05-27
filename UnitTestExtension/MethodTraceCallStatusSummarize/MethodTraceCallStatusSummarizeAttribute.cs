using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SPEkit.UnitTestExtension
{
    public sealed partial class MethodTraceCallStatusSummarizeAttribute
    {
        private readonly bool _ignoreThisType;

        private readonly Type[] _includeExtraTypes;
        private MethodBase _method;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="includeExtraTypes">包含的其他的该方法可以分析的类</param>
        /// <param name="ignoreThisType">表明此方法是否不能分析自己所在类</param>
        public MethodTraceCallStatusSummarizeAttribute(Type[] includeExtraTypes = null, bool ignoreThisType = false)
        {
            _includeExtraTypes = includeExtraTypes ?? Array.Empty<Type>();
            _ignoreThisType = ignoreThisType;
        }

        /// <summary>
        /// 获取指定类的跟踪会话集合
        /// </summary>
        /// <param name="type">查询的类</param>
        /// <returns></returns>
        /// <exception cref="AttributeNotFoundException"></exception>
        public static Dictionary<MethodInfo, MethodTraceCallStatusAttribute> GetTraceMsgInSpecifiedClass(Type type)
        {
            var methods = _getRegisteredMethods(type);
            if (!methods.Any())
                throw new AttributeNotFoundException(typeof(MethodTraceCallStatusAttribute).ToString(), type);
            var ans = methods.ToDictionary(method => method,
                method => method.GetCustomAttributes<MethodTraceCallStatusAttribute>().First());
            return ans;
        }

        /// <summary>
        /// 查询此函数是否标记了<see cref="MethodTraceCallStatusSummarizeAttribute"/>
        /// </summary>
        /// <param name="method">查询的函数</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsRegistered(MethodBase method)
        {
            return method.IsDefined(typeof(MethodTraceCallStatusSummarizeAttribute));
        }

        /// <summary>
        /// 获取调用函数所注册的类的跟踪会话列表
        /// </summary>
        /// <returns></returns>
        /// <exception cref="AttributeNotFoundException"></exception>
        /// <exception cref="AttributeNotRegisterException"></exception>
        
        public static Dictionary<MethodInfo, MethodTraceCallStatusAttribute> GetTraceMsg()
        {
            var callMethod = new StackTrace().GetFrame(1).GetMethod();
            if (callMethod == null || IsRegistered(callMethod))
                throw new AttributeNotRegisterException(typeof(MethodTraceCallStatusSummarizeAttribute).ToString());

            var attr = callMethod.GetCustomAttributes<MethodTraceCallStatusSummarizeAttribute>().First();
            var callClass = callMethod.DeclaringType;
            if (callClass == null)
                throw new AttributeNotFoundException(typeof(MethodTraceCallStatusAttribute).ToString(), typeof(object));
            var methods = _getRegisteredMethods(callClass);
            if (!methods.Any())
                throw new AttributeNotFoundException(typeof(MethodTraceCallStatusAttribute).ToString(), callClass);
            return methods.ToDictionary(method => method,
                method => method.GetCustomAttributes<MethodTraceCallStatusAttribute>().First());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IOrderedEnumerable<MethodInfo> _getRegisteredMethods(Type type)
        {
            return from method in type.GetMethods()
                where IsDefined(method, typeof(MethodTraceCallStatusAttribute))
                orderby method.Name
                select method;
        }

        public static Dictionary<Type, Dictionary<MethodInfo, MethodTraceCallStatusAttribute>> GetTraceMsgs()
        {
            var callMethod = new StackTrace().GetFrame(1).GetMethod();
            if (callMethod == null || IsRegistered(callMethod))
                throw new AttributeNotRegisterException(typeof(MethodTraceCallStatusSummarizeAttribute).ToString());

            var attr = callMethod.GetCustomAttributes<MethodTraceCallStatusSummarizeAttribute>().First();
            var callClass = callMethod.DeclaringType;
            if (callClass == null)
                throw new AttributeNotFoundException(typeof(MethodTraceCallStatusAttribute).ToString(), typeof(object));
            var types = attr._includeExtraTypes.AsEnumerable();
            if (!attr._ignoreThisType)
                types = types.Append(callClass);
            if (!types.Any()) throw new AttributeConfigurationNullException();
            return types.ToDictionary(
                type => type, GetTraceMsgInSpecifiedClass);
        }

        public override bool IsDefaultAttribute()
        {
            return _ignoreThisType == false && _includeExtraTypes.Length == 0;
        }

        public override void RuntimeInitialize(MethodBase method)
        {
            base.RuntimeInitialize(method);
            _method = method;
            _registerThis();
        }

        public MethodBase GetDeclaringMethod()
        {
            return _method;
        }
    }
}