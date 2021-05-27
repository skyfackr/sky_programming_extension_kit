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

        public static MethodBase[] GetSummarizers()
        {
            return _summarizers.Keys.ToArray();
        }

        public static MethodBase[] GetSummarizer(Type obj)
        {
            if (!obj.IsClass) return Array.Empty<MethodBase>();
            var result = from summarizers in _summarizers
                where summarizers.Value.Contains(obj)
                select summarizers.Key;
            return result.ToArray();
        }

        public static MethodBase[] GetSummarizer(MethodBase obj)
        {
            if (!obj.IsDefined(typeof(MethodTraceCallStatusAttribute))) return Array.Empty<MethodBase>();
            var objFather = obj.DeclaringType;
            while (objFather is {IsClass: false}) objFather = objFather.DeclaringType;

            return objFather == null ? Array.Empty<MethodBase>() : GetSummarizer(objFather);
        }

        public static MethodBase[] GetSummarizer(PropertyInfo obj)
        {
            if (!obj.IsDefined(typeof(MethodTraceCallStatusAttribute))) return Array.Empty<MethodBase>();
            var objFather = obj.DeclaringType;
            while (objFather is {IsClass: false}) objFather = objFather.DeclaringType;

            return objFather == null ? Array.Empty<MethodBase>() : GetSummarizer(objFather);
        }

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