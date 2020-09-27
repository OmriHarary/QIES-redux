using System;
using System.Text.Json.Serialization;
using QIES.Common.Record.Json;

namespace QIES.Common.Record
{
    [JsonConverter(typeof(ServiceDateJsonConverter))]
    public record ServiceDate
    {
        internal const string EmptyValue = "0";
        public static readonly ServiceDate Empty = new ();

        public string Date { get; private init; }

        public ServiceDate(string date)
        {
            if (!IsValid(date))
            {
                throw new ArgumentException($"Invalid value: {date}", nameof(date));
            }
            Date = date;
        }

        private ServiceDate()
        {
            Date = EmptyValue;
        }

        public override string ToString() => Date;

        public static bool IsValid(string value)
        {
            if (value.Length != 8)
                return false;

            int y, m, d;
            var yParse = int.TryParse(value.Substring(0, 4), out y);
            var mParse = int.TryParse(value.Substring(4, 2), out m);
            var dParse = int.TryParse(value.Substring(6), out d);

            return (yParse && mParse && dParse)
                    && (y >= 1980 && y <= 2999)
                    && (m >= 1 && m <= 12)
                    && (d >= 1 && d <= 31);
        }
    }
}
