using System;
using System.Text.Json.Serialization;
using QIES.Common.Records.Json;

namespace QIES.Common.Records
{
    [JsonConverter(typeof(ServiceNumberJsonConverter))]
    public record ServiceNumber
    {
        internal const string EmptyValue = "00000";
        public static readonly ServiceNumber Empty = new ();

        public string Number { get; private init; }

        public ServiceNumber(string number)
        {
            if (!IsValid(number))
            {
                throw new ArgumentException($"Invalid value: {number}", nameof(number));
            }
            Number = number;
        }

        private ServiceNumber()
        {
            Number = EmptyValue;
        }

        public override string ToString() => Number;

        public static bool IsValid(string value)
        {
            if ((value.Length == 5) && !value.StartsWith('0'))
            {
                foreach (var c in value)
                {
                    if (!char.IsDigit(c))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
    }
}
