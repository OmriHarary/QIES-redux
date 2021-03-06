using System;
using System.Text.Json.Serialization;
using QIES.Common.Records.Json;

namespace QIES.Common.Records
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

            var yParse = int.TryParse(value[0..4],  out int y);
            var mParse = int.TryParse(value[4..6],  out int m);
            var dParse = int.TryParse(value[6..],   out int d);

            return (yParse && mParse && dParse)
                    && (y >= 1980 && y <= 2999)
                    && (m >= 1 && m <= 12)
                    && (d >= 1 && d <= 31);
        }
    }
}
