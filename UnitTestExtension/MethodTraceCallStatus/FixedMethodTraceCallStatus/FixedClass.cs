using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

#nullable enable

namespace SPEkit.UnitTestExtension
{
    /// <summary>
    ///     由 <see cref="MethodTraceCallStatusAttribute" /> 转化的固定且可Json化的
    ///     <see
    ///         cref="SerializableAttribute" />
    ///     类
    ///     <para>调用 <see cref="ToJson()" /> 可转化为Json</para>
    /// </summary>
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public sealed partial class FixedMethodTraceCallStatus
    {
        /// <summary>
        ///     被定义的属性
        /// </summary>
        [JsonProperty] public readonly Attribute[]? CustomAttributes;


        /// <summary>
        ///     元数据标识符
        /// </summary>
        [JsonProperty] public readonly int MetaDataToken;

        /// <summary>
        ///     代表方法的<see cref="MethodBase" />
        /// </summary>
        public readonly MethodBase? Method;

        /// <summary>
        ///     方法名
        /// </summary>
        [JsonProperty] public readonly string? Name;

        /// <summary>
        ///     一个数组，各自代表方法参数类型的string（如果存在）
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public readonly string[]? ParametersTypeName;

        /// <summary>
        ///     代表定义此方法的类的类型的string（如果存在）
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public readonly string? ParentTypeName;

        /// <summary>
        ///     返回类型的string
        /// </summary>
        [JsonProperty] public readonly string? ReturnTypeName;

        /// <summary>
        ///     所有的跟踪会话的保存信息，其元素在该类创建后将不变，但各个会话状态可能由于程序运行而改变
        /// </summary>
        [JsonProperty] [JsonConverter(typeof(SessionsDictionaryConverter))]
        public readonly Dictionary<object, MethodTraceCallStatusAttribute.CallSession> Sessions;

        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="origin"></param>
        public FixedMethodTraceCallStatus(MethodTraceCallStatusAttribute origin)
        {
            Sessions = origin.GetSessions().ToDictionary(pair => pair.Key, pair => pair.Value);
            Method = origin.Method;
            if (Method == null) return;
            Name = Method.Name;
            CustomAttributes = Method.GetCustomAttributes().ToArray();
            ParentTypeName = Method.DeclaringType?.Name;
            MetaDataToken = Method.MetadataToken;
            ReturnTypeName = ((MethodInfo) Method).ReturnType.ToString();
            ParametersTypeName = (from param in Method.GetParameters()
                select param.ParameterType.ToString()).ToArray();
        }

        /// <summary>
        ///     json化
        /// </summary>
        /// <returns>json格式字符串</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        ///     json化
        /// </summary>
        /// <param name="formatting">格式定义</param>
        /// <returns>json格式字符串</returns>
        public string ToJson(Formatting formatting)
        {
            return JsonConvert.SerializeObject(this, formatting);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{nameof(MetaDataToken)}: {MetaDataToken}, {nameof(Method)}: {Method}";
        }
    }
}