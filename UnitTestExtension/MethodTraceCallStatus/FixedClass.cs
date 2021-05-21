using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SPEkit.UnitTestExtension
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class FixedMethodTraceCallStatus
    {
        [JsonProperty]
        [JsonConverter(typeof(SessionsDictionaryConverter))]
        public readonly Dictionary<object, MethodTraceCallStatusAttribute.CallSession> Sessions;
    }

    internal class
        SessionsDictionaryConverter : JsonConverter<Dictionary<object, MethodTraceCallStatusAttribute.CallSession>>
    {
        public override void WriteJson(JsonWriter writer, Dictionary<object, MethodTraceCallStatusAttribute.CallSession>? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override Dictionary<object, MethodTraceCallStatusAttribute.CallSession>? ReadJson(JsonReader reader, Type objectType, Dictionary<object, MethodTraceCallStatusAttribute.CallSession>? existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        
        
    }
}
