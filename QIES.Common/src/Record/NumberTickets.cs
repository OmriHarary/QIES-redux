namespace QIES.Common.Record
{
    public class NumberTickets
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

        public override string ToString() => Number.ToString();

        public static bool IsValid(int value) => value >= 1 && value <= 1000;
    }
}
