#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SPEkit.UnitTestExtension
{
    public sealed partial class MethodTraceCallStatusSummarizeAttribute
    {
        private static readonly Dictionary<MethodBase, Type[]> _summarizers = new(); //<summarizer,tracees[]>

        /// <summary>
        ///     获取当前代码所有注册了<see cref="MethodTraceCallStatusSummarizeAttribute" />的方法实例
        /// </summary>
        /// <returns></returns>
        public static MethodBase[] GetSummarizers()
        {
            return _summarizers.Keys.ToArray();
        }

        /// <summary>
        ///     返回一个数组，其中包含所有可以处理<paramref name="obj" />的方法实例
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static MethodBase[] GetSummarizer(Type obj)
        {
            if (!obj.IsClass) return Array.Empty<MethodBase>();
            var result = from summarizers in _summarizers
                where summarizers.Value.Contains(obj)
                select summarizers.Key;
            return result.ToArray();
        }

        /// <inheritdoc cref="GetSummarizer(System.Type)" />
        public static MethodBase[] GetSummarizer(MethodBase obj)
        {
            if (!obj.IsDefined(typeof(MethodTraceCallStatusAttribute))) return Array.Empty<MethodBase>();
            var objFather = obj.DeclaringType;
            while (objFather is {IsClass: false}) objFather = objFather.DeclaringType;

            return objFather == null ? Array.Empty<MethodBase>() : GetSummarizer(objFather);
        }

        /// <inheritdoc cref="GetSummarizer(System.Type)" />
        public static MethodBase[] GetSummarizer(PropertyInfo obj)
        {
            if (!obj.IsDefined(typeof(MethodTraceCallStatusAttribute))) return Array.Empty<MethodBase>();
            var objFather = obj.DeclaringType;
            while (objFather is {IsClass: false}) objFather = objFather.DeclaringType;

            return objFather == null ? Array.Empty<MethodBase>() : GetSummarizer(objFather);
        }

        /// <summary>
        ///     返回方法中注册的<see cref="MethodTraceCallStatusSummarizeAttribute" />已声明的可处理类型
        /// </summary>
        /// <param name="summarizer"></param>
        /// <returns></returns>
        /// <remarks>如果传入方法未注册<see cref="MethodTraceCallStatusSummarizeAttribute" />则直接返回空数组</remarks>
        public static Type[] GetTracees(MethodBase summarizer)
        {
            if (!IsRegistered(summarizer)) return Array.Empty<Type>();
            return _summarizers[summarizer];
        }

        private void _registerThis()
        {
            var mytracees = _includeExtraTypes;
            if (!_ignoreThisType && _method.DeclaringType != null)
                mytracees = mytracees.Append(_method.DeclaringType).ToArray();
            //var me = new KeyValuePair<MethodBase, Type[]>(this._method, mytracees);
            _summarizers.Add(_method, mytracees);
        }
    }
}