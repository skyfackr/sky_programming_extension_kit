#nullable enable
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SPEkit.UnitTestExtension
{
    internal class
        SessionsDictionaryConverter : JsonConverter<Dictionary<object, MethodTraceCallStatusAttribute.CallSession>>
    {
        public override bool CanRead => false;


        public override void WriteJson(JsonWriter writer,
            Dictionary<object, MethodTraceCallStatusAttribute.CallSession>? value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var resolver = serializer.ContractResolver as DefaultContractResolver;
            writer.WriteStartObject();
            foreach (var sessionKey in value.Keys)
            {
                var session = value[sessionKey];
                writer.WritePropertyName(ResolveName(sessionKey.GetHashCode().ToString(), resolver));
                writer.WriteStartObject();

                #region 单个session格式化部分

                #region Arguments

                writer.WritePropertyName(ResolveName("Arguments", resolver));
                writer.WriteStartArray();
                foreach (var argument in session.Arguments)
                {
                    writer.WriteStartObject();
                    if (argument.GetType().IsDefined(typeof(SerializableAttribute)))
                    {
                        serializer.Serialize(writer, argument);
                    }
                    else
                    {
                        writer.WritePropertyName(ResolveName("HashCode", resolver));
                        writer.WriteValue(argument.GetHashCode());
                        writer.WritePropertyName(ResolveName("StringValue", resolver));
                        writer.WriteValue(argument.ToString());
                        writer.WritePropertyName(ResolveName("Type", resolver));
                        writer.WriteValue(argument.GetType().ToString());
                    }

                    writer.WriteEndObject();
                }

                writer.WriteEndArray();

                #endregion

                #region EndTime

                writer.WritePropertyName(ResolveName("EndTime", resolver));
                if (session.EndTime == null)
                    writer.WriteNull();
                else
                    serializer.Serialize(writer, session.EndTime.Value.ToLocalTime());

                #endregion

                #region exce

                writer.WritePropertyName(ResolveName("Exception", resolver));
                if (session.exce == null)
                    writer.WriteNull();
                else
                    serializer.Serialize(writer, session.exce);

                #endregion

                #region ExcuteTime

                writer.WritePropertyName(ResolveName("ExcuteTime", resolver));
                serializer.Serialize(writer, session.ExcuteTime);

                #endregion

                #region ReturnValue

                writer.WritePropertyName(ResolveName("ReturnValue", resolver));
                if (session.ReturnValue == null)
                {
                    writer.WriteNull();
                }
                else
                {
                    var argument = session.ReturnValue;

                    if (argument.GetType().IsDefined(typeof(SerializableAttribute)))
                    {
                        serializer.Serialize(writer, argument);
                    }
                    else
                    {
                        writer.WriteStartObject();
                        writer.WritePropertyName(ResolveName("HashCode", resolver));
                        writer.WriteValue(argument.GetHashCode());
                        writer.WritePropertyName(ResolveName("StringValue", resolver));
                        writer.WriteValue(argument.ToString());
                        writer.WritePropertyName(ResolveName("Type", resolver));
                        writer.WriteValue(argument.GetType().ToString());
                        writer.WriteEndObject();
                    }
                }

                #endregion

                #region Stack

                writer.WritePropertyName(ResolveName("Stack", resolver));
                if (session.Stack == null)
                    writer.WriteNull();
                else
                    writer.WriteValue(session.Stack.ToString());

                #endregion

                #region StartTime

                writer.WritePropertyName(ResolveName("StartTime", resolver));
                if (session.StartTime == null)
                    writer.WriteNull();
                else
                    serializer.Serialize(writer, session.StartTime.Value.ToLocalTime());

                #endregion

                #region Status

                writer.WritePropertyName(ResolveName("Status", resolver));
                writer.WriteValue(Enum.GetName(session.Status));

                #endregion

                #endregion

                writer.WriteEndObject();
            }

            writer.WriteEndObject();
        }

        public override Dictionary<object, MethodTraceCallStatusAttribute.CallSession>? ReadJson(JsonReader reader,
            Type objectType, Dictionary<object, MethodTraceCallStatusAttribute.CallSession>? existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string ResolveName(string name, DefaultContractResolver? resolver)
        {
            return resolver == null ? name : resolver.GetResolvedPropertyName(name);
        }
    }
}