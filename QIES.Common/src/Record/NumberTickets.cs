using System;

namespace QIES.Common.Record
{
    public class NumberTickets : IEquatable<NumberTickets>
    {
        private const int Default = 0;
        public int Number { get; private set; }

        public NumberTickets(int number)
        {
            if (!IsValid(number))
            {
                throw new System.ArgumentException();
            }
            this.Number = number;
        }

        public NumberTickets()
        {
            this.Number = Default;
        }

        public bool Equals(NumberTickets? other) => Number == other?.Number;
        public override bool Equals(object? obj) => obj is NumberTickets otherNum && Equals(otherNum);
        public override string ToString() => Number.ToString();
        public override int GetHashCode() => HashCode.Combine(Number);

        public static bool operator ==(NumberTickets lhs, NumberTickets rhs) => lhs?.Equals(rhs) ?? rhs is null;
        public static bool operator !=(NumberTickets lhs, NumberTickets rhs) => !(lhs == rhs);

        public static bool IsValid(int value) => value >= 1 && value <= 1000;

    }
}
