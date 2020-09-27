using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace QIES.Common.Record.Json
{
    internal sealed class ServiceNameJsonConverter : JsonConverter<ServiceName>
    {
        public override ServiceName Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var serviceName = reader.GetString();
            if (serviceName is not null)
            {
                if (serviceName == ServiceName.EmptyValue)
                    return ServiceName.Empty;
                if (ServiceName.IsValid(serviceName))
                    return new ServiceName(serviceName);
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, ServiceName value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.ToString());
    }
}
