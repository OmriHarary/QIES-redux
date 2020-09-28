using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace QIES.Common.Records.Json
{
    internal sealed class ServiceNumberJsonConverter : JsonConverter<ServiceNumber>
    {
        public override ServiceNumber Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var serviceNumber = reader.GetString();
            if (serviceNumber is not null)
            {
                if (serviceNumber == ServiceNumber.EmptyValue)
                    return ServiceNumber.Empty;
                if (ServiceNumber.IsValid(serviceNumber))
                    return new ServiceNumber(serviceNumber);
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, ServiceNumber value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.ToString());
    }
}
