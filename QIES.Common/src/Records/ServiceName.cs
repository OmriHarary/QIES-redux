using System;
using System.Text.Json.Serialization;
using QIES.Common.Records.Json;

namespace QIES.Common.Records
{
    [JsonConverter(typeof(ServiceNameJsonConverter))]
    public record ServiceName
    {
        internal const string EmptyValue = "****";
        public static readonly ServiceName Empty = new ();

        public string Name { get; private init; }

        public ServiceName(string name)
        {
            if (!IsValid(name))
            {
                throw new ArgumentException($"Invalid value: {name}", nameof(name));
            }
            Name = name;
        }

        private ServiceName()
        {
            Name = EmptyValue;
        }

        public override string ToString() => Name;

        public static bool IsValid(string value)
        {
            if ((value.Length >= 3) && (value.Length <= 39))
            {
                if (!(value.StartsWith(' ') || value.EndsWith(' ')))
                {
                    foreach (var c in value)
                    {
                        if (!(char.IsLetterOrDigit(c) || (c == '\'') || (c == ' ')))
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
            return false;
        }
    }
}
