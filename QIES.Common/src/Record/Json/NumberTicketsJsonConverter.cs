using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace QIES.Common.Record.Json
{
    internal sealed class NumberTicketsJsonConverter : JsonConverter<NumberTickets>
    {
        public override NumberTickets Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var numberTickets = reader.GetInt32();
            if (numberTickets == NumberTickets.EmptyValue)
                return NumberTickets.Empty;
            if (NumberTickets.IsValid(numberTickets))
                return new NumberTickets(numberTickets);

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, NumberTickets value, JsonSerializerOptions options)
            => writer.WriteNumberValue(value.Number);
    }
}
