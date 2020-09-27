using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace QIES.Common.Record.Json
{
    internal sealed class ServiceDateJsonConverter : JsonConverter<ServiceDate>
    {
        public override ServiceDate Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var serviceDate = reader.GetString();
            if (serviceDate is not null && ServiceDate.IsValid(serviceDate))
                return new ServiceDate(serviceDate);

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, ServiceDate value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.ToString());
    }
}
