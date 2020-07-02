namespace QIES.Backoffice.Parser.Record
{
    public class NumberTickets : RecordElement
    {
        public int Number { get; set; }
        private const int Default = 0;

        public NumberTickets(int number)
        {
            if (!IsValid(number))
            {
                throw new System.ArgumentException();
            }
            this.IsSet = true;
            this.Number = number;
        }

        public NumberTickets()
        {
        }

        public override string ToString()
        {
            if (IsSet)
            {
                return Number.ToString();
            }
            return Default.ToString();
        }

        public static bool IsValid(int value)
        {
            return value >= 1 && value <= 1000;
        }
    }
}
