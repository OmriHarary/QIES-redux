namespace QIES.Common.Record
{
    public class ServiceName
    {
        private const string Default = "****";
        public string Name { get; private set; }

        public ServiceName(string name)
        {
            if (!IsValid(name))
            {
                throw new System.ArgumentException();
            }
            this.Name = name;
        }

        public ServiceName()
        {
            this.Name = Default;
        }

        public override string ToString() => Name;
        public override bool Equals(object? obj) => obj is ServiceName otherName && this.Name == otherName.Name;
        public override int GetHashCode() => System.HashCode.Combine(Name);

        public static bool operator ==(ServiceName lhs, ServiceName rhs) => lhs.Equals(rhs);
        public static bool operator !=(ServiceName lhs, ServiceName rhs) => !(lhs == rhs);

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
