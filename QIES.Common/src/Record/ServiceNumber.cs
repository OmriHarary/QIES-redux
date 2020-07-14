namespace QIES.Common.Record
{
    public class ServiceNumber
    {
        private const string Default = "00000";
        public string Number { get; private set; }

        public ServiceNumber(string number)
        {
            if (!IsValid(number))
            {
                throw new System.ArgumentException();
            }
            this.Number = number;
        }

        public ServiceNumber()
        {
            this.Number = Default;
        }

        public override string ToString() => Number;
        public override bool Equals(object? obj) => obj is ServiceNumber otherNumber && this.Number == otherNumber.Number;
        public override int GetHashCode() => System.HashCode.Combine(Number);

        public static bool operator ==(ServiceNumber lhs, ServiceNumber rhs) => lhs?.Equals(rhs) ?? rhs is null;
        public static bool operator !=(ServiceNumber lhs, ServiceNumber rhs) => !(lhs == rhs);

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
