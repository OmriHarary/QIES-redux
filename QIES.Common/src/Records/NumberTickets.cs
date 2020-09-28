using System;
using System.Text.Json.Serialization;
using QIES.Common.Records.Json;

namespace QIES.Common.Records
{
    [JsonConverter(typeof(NumberTicketsJsonConverter))]
    public record NumberTickets
    {
        internal const int EmptyValue = 0;
        public static readonly NumberTickets Empty = new ();

        public int Number { get; private init; }

        public NumberTickets(int number)
        {
            if (!IsValid(number))
            {
                throw new ArgumentException($"Invalid value: {number}", nameof(number));
            }
            Number = number;
        }

        private NumberTickets()
        {
            Number = EmptyValue;
        }

        public override string ToString() => Number.ToString();

        public static bool IsValid(int value) => value >= 1 && value <= 1000;
    }
}
