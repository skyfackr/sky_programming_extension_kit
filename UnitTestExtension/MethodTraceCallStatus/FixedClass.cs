using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

#nullable enable

namespace SPEkit.UnitTestExtension
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class FixedMethodTraceCallStatus
    {
        [JsonProperty]
        [JsonConverter(typeof(SessionsDictionaryConverter))]
        public readonly Dictionary<object, MethodTraceCallStatusAttribute.CallSession> Sessions;

        public readonly MethodBase Method;
        [JsonProperty]
        public readonly string Name;
        [JsonProperty]
        public readonly Attribute[] CustomAttributes;
        [JsonProperty(NullValueHandling =NullValueHandling.Ignore)]
        public readonly string? ParentTypeName=null;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public readonly string[]? ParametersTypeName = null;
        [JsonProperty]
        public readonly int MetaDataToken;

        [JsonProperty]
        public readonly string ReturnTypeName;

        public FixedMethodTraceCallStatus(MethodTraceCallStatusAttribute origin)
        {
            this.Sessions = origin.GetSessions().ToDictionary(pair => pair.Key, pair => pair.Value);
            this.Method = origin.Method;
            this.Name = this.Method.Name;
            this.CustomAttributes = this.Method.GetCustomAttributes().ToArray();
            this.ParentTypeName = this.Method.DeclaringType?.Name;
            this.MetaDataToken = this.Method.MetadataToken;
            this.ReturnTypeName = ((MethodInfo) this.Method).ReturnType.ToString();
            this.ParametersTypeName = (from param in this.Method.GetParameters()
                select param.ParameterType.ToString()).ToArray();
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
        
    }

    
}
